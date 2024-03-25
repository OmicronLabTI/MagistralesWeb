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

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service layer client.</param>
        /// <param name="logger">The logger.</param>
        public OrderService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
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
                batchNumber.Quantity = b.Quantity.ToString().ToParseDouble(0);
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
    }
}
