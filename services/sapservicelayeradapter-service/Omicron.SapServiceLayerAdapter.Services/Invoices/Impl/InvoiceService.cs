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

        /// <inheritdoc/>
        public async Task<ResultModel> CreateInvoice(CreateInvoiceDocumentDto createInvoiceDocumentInfo)
        {
            var logBase = string.Format(LogsConstants.CreateInvoiceInSapLogBase, createInvoiceDocumentInfo.ProcessId);
            try
            {
                this.logger.Information(LogsConstants.GetDeliveriesForInvoice, logBase, JsonConvert.SerializeObject(createInvoiceDocumentInfo));
                var (documentLines, comment) = await this.GetDocumentLinesByDeliveries(createInvoiceDocumentInfo.IdDeliveries);
                var invoiceRequest = new CreateInvoiceDto
                {
                    CardCode = createInvoiceDocumentInfo.CardCode,
                    DocumentDate = DateTime.Now,
                    DocumentDueDate = DateTime.Now,
                    DocumentTaxDate = DateTime.Now,
                    CfdiDriverVersion = createInvoiceDocumentInfo.CfdiDriverVersion,
                    Comments = comment,
                    InvoiceDocumentLines = documentLines,
                    InvoiceId = createInvoiceDocumentInfo.InvoiceId,
                };

                this.logger.Information(LogsConstants.CreateInvoiceOnSap, logBase, JsonConvert.SerializeObject(invoiceRequest));
                var invoiceResponse = await this.serviceLayerClient.PostAsync(
                        ServiceQuerysConstants.QryInvoiceDocument, JsonConvert.SerializeObject(invoiceRequest));

                if (!invoiceResponse.Success)
                {
                    this.logger.Error(LogsConstants.InvoiceServiceLayerError, logBase, invoiceResponse.UserError);
                    return ServiceUtils.CreateResult(false, 500, invoiceResponse.UserError, null, null);
                }

                var createdInvoice = JsonConvert.DeserializeObject<InvoiceDto>(invoiceResponse.Response.ToString());
                await this.ConfigurePaymentMethod(createdInvoice.DocumentEntry);
                this.logger.Information(LogsConstants.InvoiceCreatedSuccessfully, logBase);
                return ServiceUtils.CreateResult(true, 200, null, createdInvoice.DocumentNumber, null);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, LogsConstants.ServiceLayerErrorToCreateInvoice, logBase);
                return ServiceUtils.CreateResult(false, 500, ex.Message, null, null);
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

        private async Task<(List<CreateInvoiceDocumentLinesDto>, string)> GetDocumentLinesByDeliveries(List<int> idDeliveries)
        {
            var query = ServiceUtils.BuildFilteredQueryByIds(
                ServiceQuerysConstants.QryDeliveryNotes,
                idDeliveries,
                ServiceConstants.DocNumFieldName,
                ServiceConstants.OperatorOr);

            var deliveries = await this.serviceLayerClient.GetAsync(query);
            var deliveryNoteResponse = JsonConvert.DeserializeObject<ServiceLayerGenericMultipleResultDto<DeliveryNoteDto>>(deliveries.Response.ToString());

            var deliveriesById = idDeliveries
                .Select(id => (Id: id, Delivery: deliveryNoteResponse.Value.First(x => x.DocEntry == id)))
                .ToList();

            var documentLines = deliveriesById
                .SelectMany(r => r.Delivery.DeliveryNoteLines.Select(line => new CreateInvoiceDocumentLinesDto
                {
                    DocumentBaseType = ServiceConstants.DeliveryBaseTypeForInvoice,
                    DocumentBaseEntry = r.Id,
                    DocumentBaseLine = line.LineNumber,
                    Container = line.Container,
                    Label = line.Label,
                }))
                .ToList();

            var baseComment = string.Format(
                ServiceConstants.CommentForCreatedInvoice,
                string.Join(ServiceConstants.Comma, deliveriesById.Select(r => r.Id)));

            var orderComment = await this.GetOldestOrderCommentFromDeliveries(deliveriesById);

            var finalComment = string.IsNullOrWhiteSpace(orderComment)
                ? baseComment
                : $"{baseComment}, {orderComment}";

            this.logger.Information($"Invoice comment: {finalComment}");
            return (documentLines, finalComment);
        }

        private async Task ConfigurePaymentMethod(int invoiceId)
        {
            try
            {
                var invoiceResponse = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryInvoiceDocumentByDocEntry, invoiceId));

                if (!invoiceResponse.Success)
                {
                    this.logger.Warning($"The invoice could not be read {invoiceId} error: {invoiceResponse.UserError}");
                    return;
                }

                var invoice = JsonConvert.DeserializeObject<InvoicePaymentInfoDto>(invoiceResponse.Response.ToString());

                var paymentMethod = invoice.FormaPago33 ?? string.Empty;
                var method = paymentMethod.Trim() == ServiceConstants.PaymentFormat ? ServiceConstants.PaymentPPD : ServiceConstants.PaymentPUE;
                var updateRequest = new { U_BXP_METPAGO33 = method };
                var updateResponse = await this.serviceLayerClient.PatchAsync(
                    string.Format(ServiceQuerysConstants.QryInvoiceDocumentByDocEntry, invoiceId),
                    JsonConvert.SerializeObject(updateRequest));

                if (!updateResponse.Success)
                {
                    this.logger.Error($"Error update U_BXP_METPAGO33 invoice {invoiceId}: {updateResponse.UserError}");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"Error calculate payment method for invoice {invoiceId}");
            }
        }

        private async Task<string> GetOldestOrderCommentFromDeliveries(List<(int Id, DeliveryNoteDto Delivery)> deliveriesById)
        {
            try
            {
                var orderIds = deliveriesById
                    .SelectMany(d => d.Delivery.DeliveryNoteLines)
                    .Where(l => l.BaseEntry > 0 && l.BaseType == ServiceConstants.BaseTypeOrder)
                    .Select(l => l.BaseEntry.Value)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                if (!orderIds.Any())
                {
                    return null;
                }

                this.logger.Information($"Fetching comments from {orderIds.Count} orders");

                var allOrders = new List<OrderCommentDto>();

                foreach (var batch in orderIds.Chunk(ServiceConstants.OrdersBatchSize))
                {
                    var filter = string.Join(" or ", batch.Select(id => $"DocEntry eq {id}"));
                    var query = string.Format(ServiceConstants.QryOrdersWithComments, filter);

                    var response = await this.serviceLayerClient.GetAsync(query);
                    if (response.Success)
                    {
                        var result = JsonConvert.DeserializeObject<ServiceLayerGenericMultipleResultDto<OrderCommentDto>>(
                            response.Response.ToString());
                        if (result?.Value != null)
                        {
                            allOrders.AddRange(result.Value);
                        }
                    }
                }

                var comment = allOrders
                    .Where(o => !string.IsNullOrWhiteSpace(o.Comments))
                    .OrderBy(o => o.DocEntry)
                    .FirstOrDefault()
                    ?.Comments?.Trim();

                if (comment != null)
                {
                    this.logger.Information($"Found comment: {comment}");
                }

                return comment;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Error getting order comments");
                return null;
            }
        }
    }
}
