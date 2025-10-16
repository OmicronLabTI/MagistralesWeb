// <summary>
// <copyright file="InvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Invoices.Impl
{
    /// <summary>
    /// Class for Invoice Service.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IServiceLayerClient serviceLayerClient;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service Layer Client.</param>
        /// <param name="logger">Logger.</param>
        public InvoiceService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateInvoiceByRemissions(List<int> deliveryIds)
        {
            var query = $"{ServiceQuerysConstants.QryDeliveryNotes}?$filter={string.Join(" or ", deliveryIds.Select(id => $"(DocNum eq {id})"))}";
            var result = await this.serviceLayerClient.GetAsync(query);

            if (result.Success)
            {
                var deliveryNoteResponse = JsonConvert.DeserializeObject<ServiceLayerGenericMultipleResultDto<DeliveryNoteCreatedDto>>(result.Response.ToString());
                var first = deliveryNoteResponse.Value.FirstOrDefault() ?? new DeliveryNoteCreatedDto() { CustomerCode = string.Empty };

                var createInvoice = new CreateInvoiceDto
                {
                    CardCode = first.CustomerCode,
                    DocumentLines = new List<CreateInvoiceLineDto>(),
                };
                deliveryNoteResponse.Value.ForEach(delivery =>
                {
                    createInvoice.DocumentLines.AddRange(delivery.DocumentLines.Select(line => new CreateInvoiceLineDto
                    {
                        BaseType = 15,
                        BaseEntry = delivery.DocEntry,
                        BaseLine = line.LineNum,
                    }));
                });

                await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryInvoiceDocument, JsonConvert.SerializeObject(createInvoice));
            }

            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateInvoiceTrackingInfo(int invoiceId, TrackingInformationDto packageInformationSend)
        {
            var dictionaryResult = new Dictionary<string, string>();
            try
            {
                var responseInvoice = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryInvoiceDocumentByDocEntry, invoiceId));
                if (!responseInvoice.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.UpdateTrackingInvoiceNotFoundError, invoiceId));
                    dictionaryResult.Add(
                        string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, invoiceId),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            responseInvoice.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, responseInvoice.UserError)));
                    return ServiceUtils.CreateResult(true, 200, responseInvoice.UserError, dictionaryResult, null, null);
                }

                var resultInvoice = JsonConvert.DeserializeObject<InvoiceDto>(responseInvoice.Response.ToString());
                var resultShippingTypes = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryGetShippingTypesByName, packageInformationSend.TransportMode.ToUpper()));
                var responseServiceLayer = JsonConvert.DeserializeObject<ServiceLayerResponseDto>(resultShippingTypes.Response.ToString());
                var responseShippingTypes = JsonConvert.DeserializeObject<List<ShippingTypesResponseDto>>(responseServiceLayer.Value.ToString());
                if (responseShippingTypes.Any())
                {
                    var shippingDetail = responseShippingTypes.First();
                    resultInvoice.TransportationCode = shippingDetail.TransportCode;
                    (string sapTrackingNumber, string sapExtendedTrackingNumbers) = UpdateSapTracking(
                        packageInformationSend.PackageId,
                        packageInformationSend.TrackingNumber,
                        resultInvoice.TrackingNumber,
                        resultInvoice.ExtendedTrackingNumbers);

                    resultInvoice.TrackingNumber = sapTrackingNumber;
                    resultInvoice.ExtendedTrackingNumbers = sapExtendedTrackingNumbers;
                }

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Prod")
                {
                    var responseUpdate = await this.serviceLayerClient.PatchAsync(string.Format(ServiceQuerysConstants.QryInvoiceDocumentByDocEntry, invoiceId), JsonConvert.SerializeObject(resultInvoice));
                    if (!responseUpdate.Success)
                    {
                        this.logger.Error(string.Format(ServiceConstants.UpdateTrackingUpdateInvoiceError, invoiceId, responseUpdate.UserError, JsonConvert.SerializeObject(packageInformationSend)));
                        dictionaryResult.Add(string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, invoiceId), ServiceUtils.GetDictionaryValueString(ServiceConstants.RelationMessagesServiceLayer, responseUpdate.UserError, string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, responseUpdate.UserError)));
                        return ServiceUtils.CreateResult(true, 200, responseUpdate.UserError, dictionaryResult, null, null);
                    }
                }

                this.logger.Information(string.Format(ServiceConstants.UpdateTrackingInvoiceUpdatedLog, invoiceId));
                dictionaryResult.Add(
                        string.Format(ServiceConstants.DictionaryKeyOkGenericFormat, invoiceId),
                        ServiceConstants.OkLabelResponse);
                return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
            }
            catch (Exception ex)
            {
                this.logger.Error(string.Format(
                    ServiceConstants.UpdateTrackingInvoiceError, invoiceId, JsonConvert.SerializeObject(packageInformationSend), ex.Message, ex.StackTrace));
                dictionaryResult.Add(string.Format(ServiceConstants.ServiceLayerErrorHandled, invoiceId), $"{ex.Message}");
                return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
            }
        }

        private static (string sapTrackingNumber, string sapExtendedTrackingNumbers) UpdateSapTracking(
        int packageId,
        string trackingNumber,
        string sapTrackingNumber,
        string sapExtendedTrackingNumbers)
        {
            string newKey = packageId.ToString();
            string newValue = trackingNumber;

            var combinedTrackingEntriesFromSap = $"{sapTrackingNumber},{sapExtendedTrackingNumbers}"
                .Split(ServiceConstants.CommaChar, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => e.Contains(ServiceConstants.HypenChar))
                .Select(e =>
                {
                    var parts = e.Split(ServiceConstants.HypenChar, 2);
                    return (Key: parts[0], Value: parts[1]);
                })
                .GroupBy(p => p.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value);

            combinedTrackingEntriesFromSap[newKey] = newValue;

            var orderedPairs = combinedTrackingEntriesFromSap
                .Select(kvp => $"{kvp.Key}-{kvp.Value}")
                .ToList();

            var (trackingBuilder, extendedBuilder) = orderedPairs.Aggregate(
                (trackingBuilder: new StringBuilder(), extendedBuilder: new StringBuilder()),
                (acc, entry) =>
                {
                    int projectedLength = acc.trackingBuilder.Length + entry.Length + (acc.trackingBuilder.Length > 0 ? 1 : 0);

                    if (projectedLength <= ServiceConstants.MaxSapTrackingLength)
                    {
                        AppendWithComma(acc.trackingBuilder, entry);
                    }
                    else
                    {
                        AppendWithComma(acc.extendedBuilder, entry);
                    }

                    return acc;
                });

            return (trackingBuilder.ToString(), extendedBuilder.ToString());
        }

        private static void AppendWithComma(StringBuilder sb, string value)
        {
            if (sb.Length > 0)
            {
                sb.Append(ServiceConstants.CommaChar);
            }

            sb.Append(value);
        }
    }
}
