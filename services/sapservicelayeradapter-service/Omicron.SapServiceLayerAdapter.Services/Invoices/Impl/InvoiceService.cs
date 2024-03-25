// <summary>
// <copyright file="InvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Axity.Commons.Utils;

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
                    return ServiceUtils.CreateResult(false, 400, responseInvoice.UserError, dictionaryResult, null, null);
                }

                var resultInvoice = JsonConvert.DeserializeObject<InvoiceDto>(responseInvoice.Response.ToString());
                var resultShippingTypes = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryGetShippingTypesByName, packageInformationSend.TransportMode.ToUpper()));
                var responseServiceLayer = JsonConvert.DeserializeObject<ServiceLayerResponseDto>(resultShippingTypes.Response.ToString());
                var responseShippingTypes = JsonConvert.DeserializeObject<List<ShippingTypesResponseDto>>(responseServiceLayer.Value.ToString());
                if (responseShippingTypes.Any())
                {
                    var shippingDetail = responseShippingTypes.First();
                    resultInvoice.TransportationCode = shippingDetail.TransportCode;
                    resultInvoice.TrackingNumber = packageInformationSend.TrackingNumber;
                }

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Prod")
                {
                    var responseUpdate = await this.serviceLayerClient.PatchAsync(string.Format(ServiceQuerysConstants.QryInvoiceDocumentByDocEntry, invoiceId), JsonConvert.SerializeObject(resultInvoice));
                    if (!responseUpdate.Success)
                    {
                        this.logger.Error(string.Format(ServiceConstants.UpdateTrackingUpdateInvoiceError, invoiceId, responseUpdate.UserError, JsonConvert.SerializeObject(packageInformationSend)));
                        dictionaryResult.Add(string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, invoiceId), ServiceUtils.GetDictionaryValueString(ServiceConstants.RelationMessagesServiceLayer, responseUpdate.UserError, string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, responseUpdate.UserError)));
                        return ServiceUtils.CreateResult(false, 400, responseUpdate.UserError, dictionaryResult, null, null);
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
                dictionaryResult.Add(string.Format(ServiceConstants.UpdateTrackingInvoiceErrorHandled, invoiceId), $"{ex.Message}");
                return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
            }
        }
    }
}
