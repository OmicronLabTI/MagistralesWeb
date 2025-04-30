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
                var attachmentId = await this.GetPrescriptionId(saleOrderModel);

                var order = new CreateOrderDto
                {
                    CardCode = saleOrderModel.CardCode,
                    DocumentDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(10),
                    ShippingCode = saleOrderModel.ShippinAddress,
                    PayToCode = saleOrderModel.BillingAddress,
                    ReferenceNumber = saleOrderModel.ProfecionalLicense,
                    OrderLines = new List<CreateOrderLineDto>(),
                    TaxId = saleOrderModel.UserRfc,
                    DiscountPercent = Convert.ToDouble(saleOrderModel.DiscountSpecial),
                    DxpOrder = saleOrderModel.TransactionId,
                    EcommerceComments = ServiceUtils.CalculateTernary(saleOrderModel.IsNamePrinted == 1, $"Nombre del paciente: {saleOrderModel.PatientName}", string.Empty),
                    BXPPaymentMethod = saleOrderModel.PaymentMethodSapCode,
                    BXPWayToPay = saleOrderModel.WayToPaySapCode,
                    OrderPackage = ServiceUtils.CalculateTernary(saleOrderModel.IsPackage, ServiceConstants.IsPackage, ServiceConstants.IsNotPackage),
                    DXPNeedsShipCost = saleOrderModel.ShippingCost,
                    SampleOrder = ServiceUtils.CalculateTernary(saleOrderModel.IsSample, "Si", "No"),
                    CFDIProvisional = saleOrderModel.CfdiValue,
                    ContactPersonCode = saleOrderModel.TypeClientOrder == ServiceConstants.ClientTypeInstitucional ? 0 : null,
                    ClientTypeOrder = saleOrderModel.TypeClientOrder,
                    TipoPedido = saleOrderModel.ManufacturerClassificationCode,
                };

                AssingValues(order, saleOrderModel, attachmentId);

                order.OrderLines = saleOrderModel.Items.Select(x => new CreateOrderLineDto()
                {
                    ItemCode = x.ItemCode,
                    Quantity = x.Quantity,
                    UnitPrice = x.CostPerPiece,
                    DiscountPercent = x.DiscountPercentage,
                    Container = x.Container,
                    Label = x.Label,
                    Prescription = ServiceUtils.CalculateTernary(x.NeedRecipe == "Y", "Si", "No"),
                }).ToList();

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

        private static void AssingValues(CreateOrderDto order, CreateSaleOrderDto saleOrderModel, int? attachmentId)
        {
            if (saleOrderModel.IsSecondary != null)
            {
                order.IsSecondary = (bool)saleOrderModel.IsSecondary ? "Y" : "N";
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

            if (!string.IsNullOrEmpty(saleOrderModel.OrderComments))
            {
                order.OrderComments = saleOrderModel.OrderComments;
            }

            order.IsOmigenomics = saleOrderModel.IsOmigenomics ? ServiceConstants.Yes : ServiceConstants.No;
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
            foreach (var line in orderLines.Where(x => itemsList.Any(item => item.ItemCode == x.ItemCode)))
            {
                var batches = CreateBatchLine(line, itemsList);
                var product = itemsList.FirstOrDefault(x => x.ItemCode.Equals(line.ItemCode)) ?? new CreateDeliveryDto() { Batches = new List<AlmacenBatchDto> { new AlmacenBatchDto() { WarehouseCode = "PT" } } };
                var warehouseCode = product.Batches.Count() == 0 ? "PT" : product.Batches.First().WarehouseCode;
                var total = batches.Count() == 0 ? line.Quantity : batches.Sum(x => x.Quantity);
                inventoryGenExitLine = new InventoryGenExitLineDto
                {
                    BaseEntry = line.BaseEntry,
                    ItemCode = line.ItemCode,
                    BaseType = -1,
                    BaseLine = line.LineNum,
                    Quantity = total,
                    WarehouseCode = warehouseCode,
                    AccountCode = "6213001",
                    BatchNumbers = batches,
                };

                inventoryGenExitLines.Add(inventoryGenExitLine);
            }

            return inventoryGenExitLines;
        }

        private async Task<int?> CreateAttachment(PrescriptionServerResponseDto serverPathInfo)
        {
            var attachment = new CreateAttachmentDto();
            var attachmentLine = new AttachmentDto
            {
                FileName = serverPathInfo.PrescriptionFileName,
                FileExtension = serverPathInfo.PrescriptionFileExtension,
                SourcePath = serverPathInfo.ServerSourcePath,
                Override = "tYES",
            };

            attachment.AttachmentLines = new List<AttachmentDto>() { attachmentLine };

            this.logger.Information($"Sap Service Layer Adapter - LOG - The attached document will try to create {JsonConvert.SerializeObject(attachment)}");
            var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryAttachments2, JsonConvert.SerializeObject(attachment));
            if (!result.Success)
            {
                this.logger.Error($"Sap Service Layer Adapter - The attachement could not be saved {result.Code} - {result.UserError} - {result.ExceptionMessage}");
                return null;
            }

            var attachmentCreated = JsonConvert.DeserializeObject<CreateAttachmentResponseDto>(result.Response.ToString());
            return attachmentCreated.AbsoluteEntry;
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

                var resultCloseOrder = JsonConvert.DeserializeObject<InventoryGenExitDto>(responseOrder.Response.ToString());

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

                if (sampleOrder.CloseOrder)
                {
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
                }

                return (string.Format(ServiceConstants.DictionaryKeyOkIdResult, sampleOrder.SaleOrderId, resultCloseOrder.DocEntry), ServiceConstants.OkLabelResponse);
            }
            catch (Exception ex)
            {
                this.logger.Error(string.Format(
                    ServiceConstants.CloseSampleOrderAnInventoryError, sampleOrder.SaleOrderId, ex.Message, ex.StackTrace));
                return (string.Format(ServiceConstants.ServiceLayerErrorHandled, sampleOrder.SaleOrderId), $"{ex.Message}");
            }
        }

        private async Task<PrescriptionServerResponseDto> DownloadRecipeOnServer(string urlPrescription)
        {
            var serverPrescriptionInfo = new PrescriptionServerResponseDto();

            if (!string.IsNullOrEmpty(urlPrescription))
            {
                var resultSapFile = await this.sapFileService.PostAsync(
                                new List<PrescriptionServerRequestDto>
                                {
                                    new () { AzurePrescriptionUrl = urlPrescription, },
                                },
                                ServiceConstants.SavePrescriptionToServer);
                var result = JsonConvert.DeserializeObject<List<PrescriptionServerResponseDto>>(resultSapFile.Response.ToString());
                serverPrescriptionInfo = result.First(ts => ts.AzurePrescriptionUrl.Equals(urlPrescription));
            }

            return serverPrescriptionInfo;
        }

        private async Task<int?> GetPrescriptionId(CreateSaleOrderDto saleOrderModel)
        {
            this.logger.Information($"Sap Service Layer Adapter - LOG  - Order to create {JsonConvert.SerializeObject(saleOrderModel)}");
            var serverPrescriptionInfo = await this.DownloadRecipeOnServer(saleOrderModel.PrescriptionUrl);
            int? attachmentId = null;
            if (!string.IsNullOrEmpty(serverPrescriptionInfo.ServerSourcePath))
            {
                attachmentId = await this.CreateAttachment(serverPrescriptionInfo);
                if (attachmentId == null)
                {
                    throw new CustomServiceException("The attachment could not be created", HttpStatusCode.BadRequest);
                }
            }

            return attachmentId;
        }
    }
}
