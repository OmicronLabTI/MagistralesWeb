// <summary>
// <copyright file="OrderService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Orders.Impl
{
    /// <summary>
    /// Class Orders Service.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IServiceLayerClient serviceLayerClient;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly ISapFileService sapFileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service layer client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="sapFileService">SapFile Service.</param>
        public OrderService(IServiceLayerClient serviceLayerClient, ILogger logger, IConfiguration configuration, ISapFileService sapFileService)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.configuration = configuration;
            this.sapFileService = sapFileService;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CloseSampleOrders(List<CloseSampleOrderDto> sampleOrders)
        {
            var dictionaryResult = new Dictionary<string, string>();
            string dictionaryKey;
            string dictionaryValue;
            this.logger.Information(string.Format(ServiceConstants.CloseSampleOrderLogInfo, JsonConvert.SerializeObject(sampleOrders)));
            foreach (var sampleOrder in sampleOrders)
            {
                (dictionaryKey, dictionaryValue) = await this.CloseSampleOrder(sampleOrder);
                dictionaryResult.Add(dictionaryKey, dictionaryValue);
            }

            return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetLastGeneratedOrder()
        {
            var result = await this.serviceLayerClient.GetAsync(ServiceQuerysConstants.QryGetLastGeneratedOrder);

            if (!result.Success)
            {
                result.Response = JsonConvert.DeserializeObject<ServiceLayerErrorResponseDto>(result.Response.ToString());
                return result;
            }

            var response = JsonConvert.DeserializeObject<ServiceLayerResponseDto>(result.Response.ToString());
            var order = JsonConvert.DeserializeObject<List<OrderDto>>(response.Value.ToString());
            return ServiceUtils.CreateResult(
                result.Success,
                result.Code,
                result.UserError,
                order,
                result.ExceptionMessage,
                result.Comments?.ToString());
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateSaleOrder(CreateSaleOrderDto saleOrderModel)
        {
            try
            {
                this.logger.Error($"Sap Service Layer Adapter - LOG  - Order to create {JsonConvert.SerializeObject(saleOrderModel)}");
                var prescription = await this.DownloadRecipeOnServer(saleOrderModel.PrescriptionUrl);
                int? attachmentId = null;
                string messageError = string.Empty;
                if (!string.IsNullOrEmpty(prescription))
                {
                    this.logger.Error($"Sap Service Layer Adapter - LOG  - Order to create PRESCRIPTION {prescription}");
                    (attachmentId, messageError) = await this.CreateAttachment(prescription);
                    this.logger.Error($"Sap Service Layer Adapter - LOG  - Order to create ATTACHMENT {prescription}");
                    if (attachmentId == null)
                    {
                        var temporalMessage = $"Error: {messageError} - {prescription} - {attachmentId}";

                        // return ServiceUtils.CreateResult(false, 400, "The attachment could not be created", "The attachment could not be created", null);
                        return ServiceUtils.CreateResult(false, 400, temporalMessage, temporalMessage, null);
                    }
                }

                var order = new CreateOrderDto();
                order.CardCode = saleOrderModel.CardCode;
                order.DocumentDate = DateTime.Now;
                order.DueDate = DateTime.Now.AddDays(10);
                order.ShippingCode = saleOrderModel.ShippinAddress;
                order.PayToCode = saleOrderModel.BillingAddress;
                order.ReferenceNumber = saleOrderModel.ProfecionalLicense;
                order.OrderLines = new List<CreateOrderLineDto>();

                if (!string.IsNullOrEmpty(saleOrderModel.UserRfc))
                {
                    order.TaxId = saleOrderModel.UserRfc;
                }

                order.DiscountPercent = Convert.ToDouble(saleOrderModel.DiscountSpecial);
                order.DxpOrder = saleOrderModel.TransactionId;
                order.EcommerceComments = saleOrderModel.IsNamePrinted == 1 ? $"Nombre del paciente: {saleOrderModel.PatientName}" : string.Empty;
                order.BXPPaymentMethod = saleOrderModel.PaymentMethodSapCode;
                order.BXPWayToPay = saleOrderModel.WayToPaySapCode;
                order.OrderPackage = saleOrderModel.IsPackage ? ServiceConstants.IsPackage : ServiceConstants.IsNotPackage;
                order.DXPNeedsShipCost = saleOrderModel.ShippingCost;
                order.SampleOrder = saleOrderModel.IsSample ? "Si" : "No";
                order.CFDIProvisional = saleOrderModel.CfdiValue;

                if (saleOrderModel.IsOmigenomicsOrder != null)
                {
                    order.IsOmigenomics = (bool)saleOrderModel.IsOmigenomicsOrder ? "Y" : "N";
                }

                if (saleOrderModel.SlpCode != null)
                {
                    order.SalesPersonCode = (int)saleOrderModel.SlpCode;
                }

                if (saleOrderModel.EmployeeId != null)
                {
                    order.DocumentsOwner = (int)saleOrderModel.EmployeeId;
                }

                if (attachmentId != null)
                {
                    order.AttachmentEntry = (int)attachmentId;
                }

                for (var i = 0; i < saleOrderModel.Items.Count; i++)
                {
                    var orderLine = new CreateOrderLineDto();
                    orderLine.ItemCode = saleOrderModel.Items[i].ItemCode;
                    orderLine.Quantity = saleOrderModel.Items[i].Quantity;
                    orderLine.UnitPrice = saleOrderModel.Items[i].CostPerPiece;
                    orderLine.DiscountPercent = saleOrderModel.Items[i].DiscountPercentage;
                    orderLine.Container = saleOrderModel.Items[i].Container;
                    orderLine.Label = saleOrderModel.Items[i].Label;
                    orderLine.Prescription = saleOrderModel.Items[i].NeedRecipe == "Y" ? "Si" : "No";
                    order.OrderLines.Add(orderLine);
                }

                var propertyMappings = new Dictionary<string, string>
                {
                    { ServiceConstants.OrdersCFDIProperty, this.configuration[ServiceConstants.CustomPropertyNameCFDI] },
                };

                var body = ServiceUtils.SerializeWithCustomProperties<CreateOrderDto>(propertyMappings, order);
                this.logger.Information($"Sap Service Layer Adapter - Order to create on service layer - {body}");
                var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryPostOrders, body);
                if (!result.Success)
                {
                    this.logger.Error($"The sale order was tried to be created: {result.Code} - {result.UserError} - {JsonConvert.SerializeObject(saleOrderModel)}");
                    return ServiceUtils.CreateResult(false, 400, result.UserError, result.UserError, null);
                }

                var createdOrder = ServiceUtils.DeserializeWithCustomProperties<OrderDto>(propertyMappings, result.Response.ToString());
                return ServiceUtils.CreateResult(true, 200, null, createdOrder, null);
            }
            catch (Exception ex)
            {
                this.logger.Error($"There was an error while creating the sale order {ex.Message} - {ex.StackTrace} - {JsonConvert.SerializeObject(saleOrderModel)}");
                return ServiceUtils.CreateResult(false, 400, null, ex.Message, null);
            }
        }

        private static List<BatchNumbersDto> CreateBatchLine(OrderLineDto orderLine, List<CreateDeliveryDto> itemsList)
        {
            var batchNumbers = new List<BatchNumbersDto>();
            var product = itemsList.FirstOrDefault(x => x.ItemCode.Equals(orderLine.ItemCode));
            product ??= new CreateDeliveryDto { OrderType = ServiceConstants.Magistral };
            if (product.OrderType == ServiceConstants.Magistral)
            {
                return batchNumbers;
            }

            var batchNumber = new BatchNumbersDto();
            foreach (var b in product.Batches)
            {
                batchNumber.Quantity = (double)b.BatchQty;
                batchNumber.BatchNumber = b.BatchNumber;
                batchNumbers.Add(batchNumber);
            }

            return batchNumbers;
        }

        private static List<InventoryGenExitLineDto> CreateInventoryGenExitLines(List<OrderLineDto> orderLines, List<CreateDeliveryDto> itemsList)
        {
            var inventoryGenExitLines = new List<InventoryGenExitLineDto>();
            InventoryGenExitLineDto inventoryGenExitLine;
            foreach (var line in orderLines)
            {
                inventoryGenExitLine = new InventoryGenExitLineDto
                {
                    ItemCode = line.ItemCode,
                    BaseType = -1,
                    BaseLine = line.LineNum,
                    Quantity = line.Quantity,
                    WarehouseCode = "PT",
                    AccountCode = "6213001",
                    BatchNumbers = CreateBatchLine(line, itemsList),
                };

                inventoryGenExitLines.Add(inventoryGenExitLine);
            }

            return inventoryGenExitLines;
        }

        private async Task<(int?, string)> CreateAttachment(string pathFile)
        {
            var attachment = new CreateAttachmentDto();
            var attachmentLine = new AttachmentDto();

            attachmentLine.FileName = Path.GetFileNameWithoutExtension(pathFile);
            attachmentLine.FileExtension = Path.GetExtension(pathFile).Substring(1);
            attachmentLine.SourcePath = Path.GetDirectoryName(pathFile);
            attachmentLine.Override = "tYES";

            attachment.AttachmentLines = new List<AttachmentDto>() { attachmentLine };

            this.logger.Error($"Sap Service Layer Adapter - LOG - The attached document will try to create {JsonConvert.SerializeObject(attachment)}");
            var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryAttachments2, JsonConvert.SerializeObject(attachment));
            if (!result.Success)
            {
                this.logger.Error($"Sap Service Layer Adapter - The attachement could not be saved {result.Code} - {result.ExceptionMessage}");
                var messageError = $"{result.UserError} - {result.ExceptionMessage} - {result.Response}";
                return (null, messageError);
            }

            var attachmentCreated = JsonConvert.DeserializeObject<CreateAttachmentResponseDto>(result.Response.ToString());
            return (attachmentCreated.AbsoluteEntry, string.Empty);
        }

        private async Task<(string, string)> CloseSampleOrder(CloseSampleOrderDto sampleOrder)
        {
            try
            {
                var responseOrder = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, sampleOrder.SaleOrderId));
                if (!responseOrder.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.CloseSampleOrderNotFoundError, sampleOrder.SaleOrderId));
                    return (
                        string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, sampleOrder.SaleOrderId),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            responseOrder.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, responseOrder.UserError)));
                }

                var resultOrder = JsonConvert.DeserializeObject<OrderDto>(responseOrder.Response.ToString());
                var inventoryGenExitRequest = new InventoryGenExitDto
                {
                    Comments = string.Format(ServiceConstants.CloseSampleOrderComment, sampleOrder.SaleOrderId),
                    InventoryGenExitLines = CreateInventoryGenExitLines(
                        resultOrder.OrderLines.Where(ol => ol.ItemCode != ServiceConstants.ShippingCostItemCode).ToList(),
                        sampleOrder.ItemsList),
                };

                var inventoryGenExitResponse = await this.serviceLayerClient.PostAsync(
                    ServiceQuerysConstants.QryPostInventoryGenExists, JsonConvert.SerializeObject(inventoryGenExitRequest));

                if (!inventoryGenExitResponse.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.CloseSampleOrderErrorToCreateInventoryGenExit, inventoryGenExitResponse.UserError));
                    return (
                        string.Format(ServiceConstants.CloseSampleOrderInventoryError, sampleOrder.SaleOrderId),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            inventoryGenExitResponse.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, inventoryGenExitResponse.UserError)));
                }

                var closeOrderResponse = await this.serviceLayerClient.PostAsync(
                    string.Format(ServiceQuerysConstants.QryPostCloseOrderById, sampleOrder.SaleOrderId), string.Empty);

                if (!closeOrderResponse.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.CloseSampleOrderErrorToCloseOrder, sampleOrder.SaleOrderId, closeOrderResponse.UserError));
                    return (
                        string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, sampleOrder.SaleOrderId),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            closeOrderResponse.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, closeOrderResponse.UserError)));
                }

                return (string.Format(ServiceConstants.DictionaryKeyOkGenericFormat, sampleOrder.SaleOrderId), ServiceConstants.OkLabelResponse);
            }
            catch (Exception ex)
            {
                this.logger.Error(string.Format(
                    ServiceConstants.CloseSampleOrderAnInventoryError, sampleOrder.SaleOrderId, ex.Message, ex.StackTrace));
                return (string.Format(ServiceConstants.ServiceLayerErrorHandled, sampleOrder.SaleOrderId), $"{ex.Message}");
            }
        }

        private async Task<string> DownloadRecipeOnServer(string urlPrescription)
        {
            var serverPrescriptionPath = string.Empty;

            if (!string.IsNullOrEmpty(urlPrescription))
            {
                var resultSapFile = await this.sapFileService.PostAsync(
                                new List<PrescriptionServerRequestDto>
                                {
                                    new () { AzurePrescriptionUrl = urlPrescription, },
                                },
                                ServiceConstants.SavePrescriptionToServer);
                var result = JsonConvert.DeserializeObject<List<PrescriptionServerResponseDto>>(resultSapFile.Response.ToString());
                serverPrescriptionPath = result.First(ts => ts.AzurePrescriptionUrl.Equals(urlPrescription)).ServerPrescriptionUrl;
            }

            return serverPrescriptionPath;
        }
    }
}
