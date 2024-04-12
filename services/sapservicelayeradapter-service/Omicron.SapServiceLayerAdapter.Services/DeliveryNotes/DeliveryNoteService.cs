// <summary>
// <copyright file="DeliveryNoteService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.DeliveryNotes
{
    /// <summary>
    /// Class representing a generic service of create delivery.
    /// </summary>
    public class DeliveryNoteService : IDeliveryNoteService
    {
        private readonly IServiceLayerClient serviceLayerClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryNoteService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">The Service layer client instance to use for sending requests.</param>
        /// <param name="logger">The logger.</param>
        public DeliveryNoteService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDelivery(List<CreateDeliveryNoteDto> createDelivery)
        {
            this.logger.Information($"order to be delivered {JsonConvert.SerializeObject(createDelivery)}");
            var dictionaryResult = new Dictionary<string, string>();
            var createDeliveryFirst = createDelivery.First();
            var saleOrderId = createDeliveryFirst.SaleOrderId;
            try
            {
                var response = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, saleOrderId));
                if (!response.Success)
                {
                    this.logger.Information($"Error to get the order {saleOrderId}, {response.UserError}");
                    dictionaryResult.Add($"{saleOrderId}-Error", response.UserError);
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var saleOrder = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());

                var deliveryNote = new DeliveryNoteDto();
                deliveryNote.CustomerCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocumentsOwner = saleOrder.DocumentsOwner;
                deliveryNote.DocumentType = "dDocument_Items";
                deliveryNote.DocumentSubType = "bod_None";
                deliveryNote.BillingAddress = saleOrder.BillingAddress;
                deliveryNote.ShippingAddress = saleOrder.ShippingAddress;
                deliveryNote.ShippingCode = saleOrder.ShippingCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = saleOrder.Comments;
                deliveryNote.RemissionComment = $"Basado en pedido: {saleOrderId}";
                deliveryNote.OrderPackage = createDelivery.Any(x => x.IsPackage == ServiceConstants.IsPackage) ? ServiceConstants.IsPackage : ServiceConstants.IsNotPackage;
                deliveryNote.IsOmigenomics = saleOrder.IsOmigenomics;
                deliveryNote.DeliveryNoteLines = new List<DeliveryNoteLineDto>();

                for (var i = 0; i < saleOrder.OrderLines.Count; i++)
                {
                    var itemCode = saleOrder.OrderLines[i].ItemCode;
                    deliveryNote = UpdateDelivery(deliveryNote, saleOrder, saleOrderId, i, createDelivery, itemCode);
                }

                if (createDelivery.Any(x => x.ItemCode == ServiceConstants.ShippingCostItemCode))
                {
                    var shippingCost = createDelivery.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode);
                    var correctBaseLineId = await this.GetShippingCostBaseLine(shippingCost.ShippingCostOrderId);

                    double.TryParse(shippingCost.OrderType, out var price);

                    var newDeliveryNote = new DeliveryNoteLineDto()
                    {
                        ItemCode = shippingCost.ItemCode,
                        Quantity = 1,
                        BaseType = 17,
                        BaseEntry = shippingCost.ShippingCostOrderId,
                        UnitPrice = price,
                        BaseLine = correctBaseLineId,
                        SalesPersonCode = saleOrder.SalesPersonCode,
                        Price = price,
                        LineTotal = price,
                    };
                    deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
                    await this.UpdateShippingCostBaseLine(shippingCost.ShippingCostOrderId, saleOrder.IsOmigenomics, saleOrder.SalesPersonCode, saleOrder.DocumentsOwner);
                }

                var deliveryNotesStg = JsonConvert.SerializeObject(deliveryNote);
                var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryDeliveryNotes, deliveryNotesStg);

                if (!result.Success)
                {
                    this.logger.Error($"The saleORder {saleOrderId} was tried to be delivered {result.Code} - {result.UserError} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {result.UserError}");
                }
                else
                {
                    this.logger.Information($"The saleORder {saleOrderId} was delivered - {result.Code}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictionaryResult), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDeliveryPartial(List<CreateDeliveryNoteDto> createDelivery)
        {
            this.logger.Information($"order to be delivered partial {JsonConvert.SerializeObject(createDelivery)}");
            var dictionaryResult = new Dictionary<string, string>();
            var createDeliveryFirst = createDelivery.First();
            var saleOrderId = createDeliveryFirst.SaleOrderId;
            var productsIds = createDelivery.Where(x => x.ItemCode != ServiceConstants.ShippingCostItemCode).Select(x => x.ItemCode).ToList();

            try
            {
                var response = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, saleOrderId));
                if (!response.Success)
                {
                    this.logger.Information($"Error to get the order {saleOrderId}, {response.UserError}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {response.UserError}");
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var saleOrder = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());
                var deliveryNote = new DeliveryNoteDto();
                deliveryNote.DocumentsOwner = saleOrder.DocumentsOwner;
                deliveryNote.CustomerCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocumentType = "dDocument_Items";
                deliveryNote.DocumentSubType = "bod_None";
                deliveryNote.BillingAddress = saleOrder.BillingAddress;
                deliveryNote.ShippingAddress = saleOrder.ShippingAddress;
                deliveryNote.ShippingCode = saleOrder.ShippingCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = saleOrder.Comments;
                deliveryNote.RemissionComment = $"Basado en pedido: {saleOrderId}";
                deliveryNote.IsOmigenomics = saleOrder.IsOmigenomics;
                deliveryNote.DeliveryNoteLines = new List<DeliveryNoteLineDto>();

                for (var i = 0; i < saleOrder.OrderLines.Count; i++)
                {
                    var itemCode = saleOrder.OrderLines[i].ItemCode;

                    if (!productsIds.Contains(itemCode))
                    {
                        continue;
                    }

                    deliveryNote = UpdateDelivery(deliveryNote, saleOrder, saleOrderId, i, createDelivery, itemCode);
                }

                if (createDelivery.Any(x => x.ItemCode == ServiceConstants.ShippingCostItemCode))
                {
                    var shippingCost = createDelivery.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode);
                    var shippingOrder = saleOrder.OrderLines.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode);

                    double.TryParse(shippingCost.OrderType, out var price);

                    var newDeliveryNote = new DeliveryNoteLineDto()
                    {
                        ItemCode = shippingCost.ItemCode,
                        Quantity = 1,
                        DiscountPercent = shippingOrder.DiscountPercent,
                        TaxCode = shippingOrder.TaxCode,
                        BaseType = -1,
                        WarehouseCode = shippingOrder.WarehouseCode,
                        Price = price,
                        Currency = shippingOrder.Currency,
                        UnitPrice = price,
                        LineTotal = shippingOrder.LineTotal,
                        Container = shippingOrder.Container,
                        Label = shippingOrder.Label,
                        SalesPersonCode = shippingOrder.SalesPersonCode,
                        BaseEntry = null,
                        BaseLine = null,
                    };

                    deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
                }

                var deliveryNotesStg = JsonConvert.SerializeObject(deliveryNote);
                var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryDeliveryNotes, deliveryNotesStg);

                if (!result.Success)
                {
                    this.logger.Error($"The saleORder {saleOrderId} was tried to be delivered {result.Code} - {result.UserError} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {result.UserError}");
                }
                else
                {
                    this.logger.Information($"The saleORder {saleOrderId} was delivered - {result.Code}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictionaryResult), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDeliveryBatch(List<CreateDeliveryNoteDto> createDelivery)
        {
            this.logger.Information($"order to be delivered batch {JsonConvert.SerializeObject(createDelivery)}");
            var dictionaryResult = new Dictionary<string, string>();
            var createDeliveryFirst = createDelivery.First();
            var saleOrderId = createDeliveryFirst.SaleOrderId;

            try
            {
                var response = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, saleOrderId));
                if (!response.Success)
                {
                    this.logger.Information($"Error to get the order {saleOrderId}, {response.UserError}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {response.UserError}");
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var listOrderType = new List<string>();
                var isOmigenomicsStr = createDeliveryFirst.IsOmigenomics ? "Y" : "N";

                var saleOrder = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());
                var deliveryNote = new DeliveryNoteDto();

                deliveryNote.CustomerCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocumentType = "dDocument_Items";
                deliveryNote.DocumentSubType = "bod_None";
                deliveryNote.DocumentsOwner = saleOrder.DocumentsOwner;
                deliveryNote.BillingAddress = saleOrder.BillingAddress;
                deliveryNote.ShippingAddress = saleOrder.ShippingAddress;
                deliveryNote.ShippingCode = saleOrder.ShippingCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.RemissionComment = $"Basado en pedido: {saleOrderId}";
                deliveryNote.IsOmigenomics = isOmigenomicsStr;
                deliveryNote.DeliveryNoteLines = new List<DeliveryNoteLineDto>();

                var commentMultiple = new StringBuilder();

                var ordersGrouped = createDelivery.GroupBy(p => p.SaleOrderId).ToList();
                foreach (var sale in ordersGrouped)
                {
                    var saleOrderFound = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, sale.FirstOrDefault().SaleOrderId));
                    if (!saleOrderFound.Success)
                    {
                        this.logger.Information($"The sale Order {sale.FirstOrDefault().SaleOrderId} was not found for creating the delivery");
                        dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {saleOrderFound.UserError}");
                        continue;
                    }

                    var saleOrderFoundJson = saleOrderFound.Response.ToString();
                    var saleOrderFoundLocal = JsonConvert.DeserializeObject<OrderDto>(saleOrderFoundJson);

                    listOrderType.Add(saleOrder.TypeOrder ?? string.Empty);
                    commentMultiple.Append($"{saleOrder.Comments} |");

                    for (var i = 0; i < saleOrderFoundLocal.OrderLines.Count; i++)
                    {
                        var itemCode = saleOrderFoundLocal.OrderLines[i].ItemCode;
                        deliveryNote = UpdateDelivery(deliveryNote, saleOrderFoundLocal, sale.FirstOrDefault().SaleOrderId, i, createDelivery, itemCode);
                    }

                    if (createDelivery.Any(x => x.ItemCode == ServiceConstants.ShippingCostItemCode && x.SaleOrderId == sale.Key))
                    {
                        this.logger.Information($"Here Starts the fl 1 when its apart.");
                        var shippingCost = createDelivery.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode && x.SaleOrderId == sale.Key);
                        double.TryParse(shippingCost.OrderType, out var price);
                        this.logger.Information($"The price is {price}");

                        var correctBaseLineId = await this.GetShippingCostBaseLine(shippingCost.ShippingCostOrderId);
                        var newDeliveryNote = new DeliveryNoteLineDto()
                        {
                            ItemCode = shippingCost.ItemCode,
                            Quantity = 1,
                            BaseType = 17,
                            BaseEntry = shippingCost.ShippingCostOrderId,
                            UnitPrice = price,
                            BaseLine = correctBaseLineId,
                            SalesPersonCode = saleOrder.SalesPersonCode,
                            Price = price,
                            LineTotal = price,
                        };
                        deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
                        await this.UpdateShippingCostBaseLine(shippingCost.ShippingCostOrderId, isOmigenomicsStr, saleOrder.SalesPersonCode, saleOrder.DocumentsOwner);
                    }
                }

                var areAllSame = listOrderType.All(o => o == listOrderType.FirstOrDefault());
                var tipoPedidos = areAllSame ? listOrderType.FirstOrDefault() : "MX";
                deliveryNote.DeliveryOrderType = tipoPedidos == "UN" ? "LN" : tipoPedidos;
                deliveryNote.Comments = commentMultiple.Length > 253 ? commentMultiple.ToString().Substring(0, 253) : commentMultiple.ToString();

                var deliveryNotesStg = JsonConvert.SerializeObject(deliveryNote);
                var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryDeliveryNotes, deliveryNotesStg);

                if (!result.Success)
                {
                    this.logger.Error($"The saleORder {saleOrderId} was tried to be delivered batch {result.Code} - {result.UserError} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {result.UserError}");
                }
                else
                {
                    this.logger.Information($"The saleORder {saleOrderId} was delivered batch - {result.Code}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictionaryResult), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CancelDelivery(string type, List<CancelDeliveryDto> deliveryNotesToCancel)
        {
            this.logger.Information($"Deliveries to cancel: {JsonConvert.SerializeObject(deliveryNotesToCancel)}.");
            var dictionaryResult = new Dictionary<string, string>();
            foreach (var deliveryNote in deliveryNotesToCancel)
            {
                dictionaryResult = await this.CancelDeliveryNote(type, deliveryNote, dictionaryResult);
            }

            return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
        }

        private async Task UpdateShippingCostBaseLine(int saleOrderId, string isOmigenomics, int salesPersonCode, int documentsOwner)
        {
            var response = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, saleOrderId));
            if (response.Code == (int)HttpStatusCode.NotFound)
            {
                throw new CustomServiceException($"Almacen - Create Delivery Service - SaleOrderId {saleOrderId} Not found", HttpStatusCode.NotFound);
            }

            var saleOrderShipping = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());

            saleOrderShipping.DocumentsOwner = documentsOwner;
            saleOrderShipping.SalesPersonCode = salesPersonCode;
            saleOrderShipping.IsOmigenomics = isOmigenomics;

            for (var i = 0; i < saleOrderShipping.OrderLines.Count; i++)
            {
                var orderLine = saleOrderShipping.OrderLines[i];
                if (orderLine.ItemCode == ServiceConstants.ShippingCostItemCode)
                {
                    orderLine.OwnerCode = documentsOwner;
                    orderLine.SalesPersonCode = salesPersonCode;
                    break;
                }
            }

            await this.serviceLayerClient.PatchAsync("Orders", JsonConvert.SerializeObject(saleOrderShipping));
        }

        private static DeliveryNoteDto UpdateDelivery(DeliveryNoteDto deliveryNote, OrderDto saleOrder, int saleOrderId, int i, List<CreateDeliveryNoteDto> createDelivery, string itemCode)
        {
            var orderLine = saleOrder.OrderLines[i];
            var newDeliveryNote = new DeliveryNoteLineDto()
            {
                ItemCode = orderLine.ItemCode,
                Quantity = orderLine.Quantity,
                DiscountPercent = orderLine.DiscountPercent,
                TaxCode = orderLine.TaxCode,
                LineTotal = orderLine.LineTotal,
                BaseType = 17,
                WarehouseCode = orderLine.WarehouseCode,
                Container = orderLine.Container,
                Label = orderLine.Label,
                BaseEntry = saleOrderId,
                BaseLine = orderLine.LineNum,
                BatchNumbers = new List<DeliveryNoteBatchNumbersDto>(),
                UnitPrice = orderLine.UnitPrice,
                SalesPersonCode = orderLine.SalesPersonCode,
            };

            var product = createDelivery.FirstOrDefault(x => x.ItemCode.Equals(itemCode) && x.SaleOrderId == saleOrderId);
            product = product ?? new CreateDeliveryNoteDto { OrderType = ServiceConstants.Magistral };

            if (product.OrderType != ServiceConstants.Magistral)
            {
                foreach (var b in product.Batches)
                {
                    double.TryParse(b.BatchQty.ToString(), out var doubleQuantity);
                    var batch = new DeliveryNoteBatchNumbersDto();
                    batch.Quantity = doubleQuantity;
                    batch.BatchNumber = b.BatchNumber;
                    newDeliveryNote.BatchNumbers.Add(batch);
                }
            }

            deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
            return deliveryNote;
        }

        private async Task<int> GetShippingCostBaseLine(int saleOrderId)
        {
            var saleOrderShipping = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryOrdersDocumentByDocEntry, saleOrderId));
            if (saleOrderShipping.Code == (int)HttpStatusCode.NotFound)
            {
                throw new CustomServiceException($"Almacen - Create Delivery Service - SaleOrderId {saleOrderId} Not found", HttpStatusCode.NotFound);
            }

            var saleOrder = JsonConvert.DeserializeObject<OrderDto>(saleOrderShipping.Response.ToString());
            var correctBaseLineId = 0;
            for (var i = 0; i < saleOrder.OrderLines.Count; i++)
            {
                var saleOrderLine = saleOrder.OrderLines[i];
                var itemCode = saleOrderLine.ItemCode;
                if (itemCode == ServiceConstants.ShippingCostItemCode)
                {
                    correctBaseLineId = saleOrderLine.LineNum;
                    break;
                }
            }

            return correctBaseLineId;
        }

        private async Task<Dictionary<string, string>> CancelDeliveryNote(string type, CancelDeliveryDto deliveryNote, Dictionary<string, string> dictionaryResult)
        {
            try
            {
                var responseDeliveryNote = await this.serviceLayerClient.GetAsync(
                            string.Format(ServiceQuerysConstants.QryGetDeliveryNoteById, deliveryNote.Delivery));

                if (!responseDeliveryNote.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.CancelDeliveryNoteNotFoundError, deliveryNote.Delivery));
                    dictionaryResult.Add(
                        string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, deliveryNote.Delivery),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            responseDeliveryNote.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, responseDeliveryNote.UserError)));
                    return dictionaryResult;
                }

                var responseCreateCancellationDocument = await this.serviceLayerClient.PostAsync(
                    string.Format(
                        ServiceQuerysConstants.QryToCreateDeliveryNoteCancelDocumentById, deliveryNote.Delivery), string.Empty);

                if (!responseCreateCancellationDocument.Success)
                {
                    this.logger.Error(
                        string.Format(ServiceConstants.CancelDeliveryErrorToCreateDocument, deliveryNote.Delivery, responseCreateCancellationDocument.UserError));
                    dictionaryResult.Add(
                        string.Format(ServiceConstants.DictionaryKeyErrorGenericFormat, deliveryNote.Delivery),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            responseCreateCancellationDocument.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, responseCreateCancellationDocument.UserError)));
                    return dictionaryResult;
                }

                var resultDeliveryNote = JsonConvert.DeserializeObject<DeliveryNoteDto>(responseDeliveryNote.Response.ToString());
                this.logger.Information(string.Format(ServiceConstants.CancelDeliveryLogToCancelDelivery, deliveryNote.Delivery));
                dictionaryResult.Add(string.Format(ServiceConstants.DictionaryKeyOkGenericFormat, deliveryNote.Delivery), ServiceConstants.OkLabelResponse);

                if (type == ServiceConstants.Total)
                {
                    dictionaryResult = await this.CancelOrdersFromDeliveryNote(dictionaryResult, deliveryNote.SaleOrderId);
                }

                if (ServiceUtils.CalculateAnd(type == ServiceConstants.Total, deliveryNote.MagistralProducts.Any()))
                {
                    dictionaryResult = await this.CreateStockTransfer(deliveryNote, dictionaryResult, resultDeliveryNote);
                }

                return dictionaryResult;
            }
            catch (Exception ex)
            {
                dictionaryResult.Add(string.Format(ServiceConstants.ServiceLayerErrorHandled, deliveryNote.Delivery), $"{ex.Message}");
                return dictionaryResult;
            }
        }

        private async Task<Dictionary<string, string>> CreateStockTransfer(CancelDeliveryDto deliveryNote, Dictionary<string, string> dictionaryResult, DeliveryNoteDto resultDeliveryNote)
        {
            var finalWhs = ServiceConstants.DictWhs.ContainsKey(resultDeliveryNote.DeliveryOrderType) ? ServiceConstants.DictWhs[resultDeliveryNote.DeliveryOrderType] : "MG";
            var saleOrders = JsonConvert.SerializeObject(deliveryNote.SaleOrderId).Replace("[", string.Empty).Replace("]", string.Empty);

            var stockTransferLinesRequest = new List<StockTransferLineDto>();

            foreach (var item in deliveryNote.MagistralProducts.Where(mp => mp.ItemCode != ServiceConstants.ShippingCostItemCode))
            {
                stockTransferLinesRequest.Add(
                new StockTransferLineDto
                {
                    ItemCode = item.ItemCode,
                    FromWarehouseCode = "PT",
                    WarehouseCode = finalWhs,
                    Quantity = item.Pieces,
                });
            }

            var stockTransferRequest = new StockTransferDto
            {
                DocumentDate = DateTime.Today,
                FromWarehouse = "PT",
                ToWarehouse = finalWhs,
                JournalMemo = $"Traspaso por Cancelación: {saleOrders}",
                StockTransferLines = stockTransferLinesRequest,
            };

            var stockTransferResponse = await this.serviceLayerClient.PostAsync(
                    ServiceQuerysConstants.QryPostStockTransfers, JsonConvert.SerializeObject(stockTransferRequest));

            if (!stockTransferResponse.Success)
            {
                this.logger.Error(string.Format(ServiceConstants.CancelDeliveryErrorToCreateStockTransfer, deliveryNote.Delivery, stockTransferResponse.UserError));
                dictionaryResult.Add(
                    string.Format(ServiceConstants.CancelDeliveryTransferError, deliveryNote.Delivery),
                    ServiceUtils.GetDictionaryValueString(
                        ServiceConstants.RelationMessagesServiceLayer,
                        stockTransferResponse.UserError,
                        string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, stockTransferResponse.UserError)));
                return dictionaryResult;
            }

            this.logger.Information(string.Format(ServiceConstants.TransferRequestForDeliveryDone, deliveryNote.Delivery));
            dictionaryResult.Add(string.Format(ServiceConstants.TransferRequestForDeliveryOk, deliveryNote.Delivery), ServiceConstants.OkLabelResponse);
            return dictionaryResult;
        }

        private async Task<Dictionary<string, string>> CancelOrdersFromDeliveryNote(Dictionary<string, string> dictionaryResult, List<int> ordersToCancel)
        {
            ResultModel cancelOrderResult;
            foreach (var orderId in ordersToCancel)
            {
                cancelOrderResult = new ResultModel();
                cancelOrderResult = await this.serviceLayerClient.PostAsync(string.Format(ServiceQuerysConstants.QryCancelOrders, orderId), string.Empty);
                if (!cancelOrderResult.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.CancelDeliveryErrorToCancelOrder, orderId, cancelOrderResult.UserError));
                    dictionaryResult.Add(
                        string.Format(ServiceConstants.CancelDeliveryTransferError, orderId),
                        ServiceUtils.GetDictionaryValueString(
                            ServiceConstants.RelationMessagesServiceLayer,
                            cancelOrderResult.UserError,
                            string.Format(ServiceConstants.DictionaryValueErrorGenericFormat, cancelOrderResult.UserError)));
                }
            }

            return dictionaryResult;
        }
    }
}