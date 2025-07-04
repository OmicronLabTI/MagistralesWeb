// <summary>
// <copyright file="ProductionOrderService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.ProductionOrders
{
    /// <summary>
    /// the class.
    /// </summary>
    public class ProductionOrderService : IProductionOrderService
    {
        private readonly IServiceLayerClient serviceLayerClient;
        private readonly ILogger logger;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrderService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">The serviceLayerClient.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public ProductionOrderService(IServiceLayerClient serviceLayerClient, ILogger logger, IMapper mapper)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinishOrder(List<CloseProductionOrderDto> productionOrders)
        {
            var logBase = $"SapServiceLayerAdapter - Finish Order - {Guid.NewGuid():D}";
            var results = new Dictionary<int, string>();
            foreach (var productionOrderConfig in productionOrders)
            {
                var productionOrderId = productionOrderConfig.ProductionOrderId;
                try
                {
                    this.logger.Information($"{logBase} - Trying to finish production order {productionOrderId}");
                    var productionOrder = await this.GetFromServiceLayer<ProductionOrderDto>(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId), $"La orden de produción {productionOrderId} no existe.");
                    if (productionOrder.ProductionOrderStatus.Equals(ServiceConstants.ProductionOrderClosed))
                    {
                        this.logger.Information($"{logBase} - Production order is closed - {productionOrderId}");
                        continue;
                    }

                    if (!productionOrder.ProductionOrderStatus.Equals(ServiceConstants.ProductionOrderReleased))
                    {
                        this.logger.Information($"{logBase} - Production order not released - {productionOrderId}");
                        ServiceUtils.AddElementToDictionary(results, productionOrderId, string.Format(ServiceConstants.FailReasonNotReleasedProductionOrder, productionOrderId));
                        continue;
                    }

                    this.logger.Information($"{logBase} - Validate required quantity for retroactive issues - {productionOrderId}");
                    await this.ValidateRequiredQuantityForRetroactiveIssues(productionOrder);

                    this.logger.Information($"{logBase} - Validating new batches - {productionOrderId}");
                    await this.ValidateNewBatches(productionOrder.ItemNo, productionOrderConfig.Batches);

                    this.logger.Information($"{logBase} - Create inventory gen exit - {productionOrderId}");
                    await this.CreateInventoryGenExit(productionOrder, productionOrderId, logBase);

                    var productionOrderUpdated = await this.GetFromServiceLayer<ProductionOrderDto>(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId), $"La orden de produción {productionOrderId} no existe.");

                    this.logger.Information($"{logBase} - Create receipt from production order id - {productionOrderId}");
                    await this.CreateReceiptFromProductionOrderId(productionOrderId, productionOrderConfig, productionOrderUpdated, logBase);

                    this.logger.Information($"{logBase} - Close Production Order - {productionOrderId}");
                    await this.CloseProductionOrder(productionOrderId, productionOrderUpdated, logBase);
                }
                catch (CustomServiceException ex)
                {
                    this.logger.Error($"{logBase} - {ex.Message}", ex);
                    ServiceUtils.AddElementToDictionary(results, productionOrderId, ex.Message);
                }
                catch (Exception ex)
                {
                    this.logger.Error($"{logBase} - {ex.StackTrace}", ex);
                    ServiceUtils.AddElementToDictionary(results, productionOrderId, ServiceConstants.FailReasonUnexpectedError);
                }
            }

            if (!results.Any())
            {
                ServiceUtils.AddElementToDictionary(results, 0, "Ok");
            }

            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateFormula(UpdateFormulaDto updateFormula)
        {
            var dictResult = new Dictionary<string, string>();
            try
            {
                var productionOrder = await this.GetFromServiceLayer<ProductionOrderDto>(
                    string.Format(ServiceQuerysConstants.QryProductionOrderById, updateFormula.FabOrderId),
                    $"{ServiceConstants.ErrorUpdateFabOrd}-{ServiceConstants.OrderNotFound}");

                productionOrder.DueDate = updateFormula.FechaFin;
                productionOrder.PlannedQuantity = updateFormula.PlannedQuantity;
                productionOrder.Warehouse = updateFormula.Warehouse;

                var deleteItems = updateFormula.Components.Where(x => x.Action.Equals(ServiceConstants.DeleteComponent)).ToList();

                productionOrder.ProductionOrderLines = AddComponents(productionOrder.ProductionOrderLines, updateFormula.Components, updateFormula.FabOrderId);
                productionOrder.ProductionOrderLines = UpdateComponents(productionOrder.ProductionOrderLines, updateFormula.Components);
                productionOrder.ProductionOrderLines = DeleteComponents(productionOrder.ProductionOrderLines, deleteItems);

                var request = this.mapper.Map<UpdateProductionOrderDto>(productionOrder);
                await this.SaveChanges(request, updateFormula.FabOrderId);
            }
            catch (Exception ex)
            {
                dictResult.Add($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", ex.Message);
                return ServiceUtils.CreateResult(false, 400, null, dictResult, null);
            }

            dictResult.Add($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", "Ok");
            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateFabOrder(List<OrderWithDetailDto> orderWithDetail)
        {
            var dictResult = new Dictionary<string, string>();
            foreach (var order in orderWithDetail)
            {
                this.logger.Information($"The next order will be tried to be created: {order.Order.PedidoId} - {JsonConvert.SerializeObject(order.Detalle)}");
                var count = 0;
                foreach (var detail in order.Detalle)
                {
                    var plannedQtyNumber = detail.QtyPlannedDetalle ?? 0;
                    var productionOrder = new CreateProductionOrderDto();
                    productionOrder.StartDate = order.Order.FechaInicio;
                    productionOrder.DueDate = order.Order.FechaFin;
                    productionOrder.ItemNo = detail.CodigoProducto;
                    productionOrder.ProductDescription = detail.DescripcionProducto;
                    productionOrder.PlannedQuantity = plannedQtyNumber;
                    productionOrder.ProductionOrderOriginEntry = order.Order.PedidoId;

                    var body = JsonConvert.SerializeObject(productionOrder);
                    var result = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryProductionOrder, body);
                    if (!result.Success)
                    {
                        dictResult.Add(string.Format("{0}-{1}-{2}", order.Order.PedidoId, detail.CodigoProducto, count), string.Format("{0}-{1}-{2}", ServiceConstants.ErrorCreateFabOrd, result.UserError, result.UserError));
                    }
                    else
                    {
                        dictResult.Add(string.Format("{0}-{1}-{2}", order.Order.PedidoId, detail.CodigoProducto, count), "Ok");
                    }

                    count++;
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictResult), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateFabOrders(List<UpdateFabOrderDto> orderModels)
        {
            var dictResult = new Dictionary<string, string>();
            foreach (var order in orderModels)
            {
                try
                {
                    var productionOrder = await this.GetFromServiceLayer<ProductionOrderDto>(
                        string.Format(ServiceQuerysConstants.QryProductionOrderById, order.OrderFabId),
                        string.Format("{0}-{1}", ServiceConstants.ErrorUpdateFabOrd, ServiceConstants.OrderNotFound));

                    if (order.Status.Equals(ServiceConstants.StatusLiberado))
                    {
                        productionOrder.ProductionOrderStatus = ServiceConstants.ProductionOrderReleased;
                        await this.EditFabOrder(productionOrder, order.OrderFabId);
                    }

                    dictResult.Add(string.Format("{0}-{1}", order.OrderFabId, order.OrderFabId), "Ok");
                }
                catch (Exception ex)
                {
                    dictResult.Add(string.Format("{0}-{1}", order.OrderFabId, order.OrderFabId), ex.Message);
                    this.logger.Error(ex.StackTrace, ex.Message);
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CancelProductionOrder(CancelOrderDto order)
        {
            this.logger.Information($"Production order to cancel: {order.OrderId}.");
            try
            {
                var productionOrder = await this.GetFromServiceLayer<ProductionOrderDto>(
                     string.Format(ServiceQuerysConstants.QryProductionOrderById, order.OrderId),
                     ServiceConstants.NotFound);

                if (productionOrder.ProductionOrderStatus == ServiceConstants.ProductionOrderCancelled)
                {
                    this.logger.Information($"The production order {order.OrderId} is cancelled.");
                    throw new CustomServiceException(ServiceConstants.ErrorProductionOrderCancelled);
                }

                var body = JsonConvert.SerializeObject(productionOrder);
                var result = await this.serviceLayerClient.PostAsync(string.Format(ServiceQuerysConstants.QryProductionOrderByIdCancel, order.OrderId), body);

                if (!result.Success)
                {
                    this.logger.Error(result.UserError);
                    return ServiceUtils.CreateResult(true, 200, null, ServiceConstants.UnexpectedError, $"{result.Code} - {result.UserError}");
                }
            }
            catch (Exception ex)
            {
                return ServiceUtils.CreateResult(true, 200, null, ex.Message, null);
            }

            return ServiceUtils.CreateResult(true, 200, null, ServiceConstants.OkLabelResponse, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder)
        {
            KeyValuePair<string, string> result;
            var uniqueId = Guid.NewGuid().ToString();
            this.logger.Information($"Trying to CreateIsolatedProductionOrder to product {isolatedFabOrder.ProductCode}");
            try
            {
                var product = await this.GetFromServiceLayer<ItemDto>(
                    string.Format(ServiceQuerysConstants.QryProductById, isolatedFabOrder.ProductCode),
                    string.Format(ServiceConstants.FailReasonProductCodeNotExists, isolatedFabOrder.ProductCode));

                var productionOrder = new CreateIsolateProductionOrderDto();
                productionOrder.ProductionOrderType = "bopotStandard";
                productionOrder.StartDate = DateTime.Now;
                productionOrder.DueDate = DateTime.Now;
                productionOrder.ItemNo = product.ItemCode;
                productionOrder.ProductDescription = product.ItemName;
                productionOrder.PlannedQuantity = 1;
                productionOrder.DistributionRule = string.Empty;
                productionOrder.DistributionRule2 = string.Empty;
                productionOrder.DistributionRule3 = string.Empty;
                productionOrder.DistributionRule4 = string.Empty;
                productionOrder.DistributionRule5 = string.Empty;
                productionOrder.Project = string.Empty;
                productionOrder.Remarks = uniqueId;

                var body = JsonConvert.SerializeObject(productionOrder);
                var response = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryProductionOrder, body);
                if (!response.Success)
                {
                    this.logger.Error(string.Format(ServiceConstants.FailReasonUnexpectedErrorToCreateIsolatedProductionOrder, isolatedFabOrder.ProductCode, response.UserError));
                    throw new CustomServiceException(string.Format(ServiceConstants.FailReasonUnexpectedErrorToCreateIsolatedProductionOrder, isolatedFabOrder.ProductCode, response.UserError));
                }

                result = new KeyValuePair<string, string>(uniqueId, ServiceConstants.OkLabelResponse);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.StackTrace, ex.Message);
                result = new KeyValuePair<string, string>(string.Empty, ex.Message);
            }

            return ServiceUtils.CreateResult(true, 200, null, result, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateProductionOrdersBatches(List<AssignBatchDto> batchesToAssign)
        {
            var groupedByProductionOrderId = batchesToAssign.GroupBy(x => x.OrderId).ToList();

            var dictResult = new Dictionary<string, string>();
            foreach (var productionOrderGrouped in groupedByProductionOrderId)
            {
                dictResult = await this.ProcessToUpdateBatches(dictResult, productionOrderGrouped);
            }

            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PrimaryValidationForProductionOrderFinalizationInSap(
            List<CloseProductionOrderDto> productionOrderInfoToValidate)
        {
            var validationsResult = new List<ValidationsToFinalizeProductionOrdersResultDto>();
            string error;

            foreach (var productionOrderInfo in productionOrderInfoToValidate)
            {
                var logBase = string.Format(LogsConstants.PrimaryValidationForProductionOrderFinalization, productionOrderInfo.ProcessId);

                try
                {
                    this.logger.Information(LogsConstants.SearchProductionOrder, logBase, productionOrderInfo.ProductionOrderId);
                    var productionOrder = await this.GetProductionOrder(productionOrderInfo.ProductionOrderId);

                    if (productionOrder.ProductionOrderStatus.Equals(ServiceConstants.ProductionOrderClosed))
                    {
                        this.logger.Error(LogsConstants.ProductionOrderIsClosed, logBase, productionOrderInfo.ProductionOrderId);
                        validationsResult.Add(
                            GenerateValidationResult(
                                productionOrderInfo.ProductionOrderId,
                                string.Format(ServiceConstants.FailReasonClosedProductionOrder, productionOrderInfo.ProductionOrderId),
                                string.Empty));
                        continue;
                    }

                    if (!productionOrder.ProductionOrderStatus.Equals(ServiceConstants.ProductionOrderReleased))
                    {
                        this.logger.Error(LogsConstants.ProductionOrderNotReleased, logBase, productionOrderInfo.ProductionOrderId);
                        validationsResult.Add(
                            GenerateValidationResult(
                                productionOrderInfo.ProductionOrderId,
                                string.Format(ServiceConstants.FailReasonNotReleasedProductionOrder, productionOrderInfo.ProductionOrderId),
                                string.Empty));
                        continue;
                    }

                    this.logger.Information(LogsConstants.ValidateRequiredQuantityForRetroactiveIssue, logBase, productionOrderInfo.ProductionOrderId);
                    await this.ValidateRequiredQuantityForRetroactiveIssues(productionOrder);

                    this.logger.Information(LogsConstants.ValidatingNewBatches, logBase, productionOrderInfo.ProductionOrderId);
                    await this.ValidateNewBatches(productionOrder.ItemNo, productionOrderInfo.Batches);

                    validationsResult.Add(
                            GenerateValidationResult(
                                productionOrderInfo.ProductionOrderId,
                                string.Empty,
                                string.Empty));
                }
                catch (CustomServiceException ex)
                {
                    error = string.Format(LogsConstants.ProcessLogTwoParts, logBase, ex.Message);
                    this.logger.Error(ex, error);
                    validationsResult.Add(
                            GenerateValidationResult(
                                productionOrderInfo.ProductionOrderId,
                                ex.Message,
                                string.Empty));
                }
                catch (Exception ex)
                {
                    error = string.Format(LogsConstants.ProcessLogThreeParts, logBase, ex.StackTrace, ex.Message);
                    this.logger.Error(ex, error);
                    validationsResult.Add(
                            GenerateValidationResult(
                                productionOrderInfo.ProductionOrderId,
                                ServiceConstants.FailReasonUnexpectedError,
                                string.Empty));
                }
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, validationsResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrderInSap(List<CloseProductionOrderDto> productionOrdersToFinalize)
        {
            var processResultResult = new List<ValidationsToFinalizeProductionOrdersResultDto>();
            string error;
            foreach (var productionOrder in productionOrdersToFinalize)
            {
                var logBase = string.Format(LogsConstants.FinalizeProductionOrderInSap, productionOrder.ProcessId);

                try
                {
                    this.logger.Information(LogsConstants.ExecuteFinalizationSteps, logBase, productionOrder.ProductionOrderId);
                    await this.ExecuteFinalizationStepAsync(productionOrder, logBase);
                    processResultResult.Add(
                            GenerateValidationResult(
                                productionOrder.ProductionOrderId,
                                string.Empty,
                                productionOrder.LastStep));
                }
                catch (CustomServiceException ex)
                {
                    error = string.Format(LogsConstants.ProcessLogTwoParts, logBase, ex.Message);
                    this.logger.Error(ex, error);
                    processResultResult.Add(
                            GenerateValidationResult(
                                productionOrder.ProductionOrderId,
                                ex.Message,
                                productionOrder.LastStep));
                }
                catch (Exception ex)
                {
                    error = string.Format(LogsConstants.ProcessLogThreeParts, logBase, ex.StackTrace, ex.Message);
                    this.logger.Error(ex, error);
                    processResultResult.Add(
                            GenerateValidationResult(
                                productionOrder.ProductionOrderId,
                                ServiceConstants.FailReasonUnexpectedError,
                                productionOrder.LastStep));
                }
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, processResultResult, null);
        }

        private static ValidationsToFinalizeProductionOrdersResultDto GenerateValidationResult(
            int productionOrderId,
            string errorMessage,
            string lastStep)
        {
            return new ValidationsToFinalizeProductionOrdersResultDto
            {
                ProductionOrderId = productionOrderId,
                ErrorMessage = errorMessage,
                LastStep = lastStep,
            };
        }

        private static Dictionary<string, string> GetDictionaryResult(Dictionary<string, string> dictResult, int code, string error, List<AssignBatchDto> batches, int productionOrderId, bool success)
        {
            var errorItems = batches.Select((x, index) => new KeyValuePair<string, string>(
                    $"{productionOrderId}-{x.ItemCode}-{index}",
                    success ? "Ok" : $"{ServiceConstants.ErrorUpdateFabOrd}-{code}-{error}"))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

            return dictResult.Concat(errorItems).GroupBy(kv => kv.Key).ToDictionary(g => g.Key, g => g.First().Value);
        }

        private static ProductionOrderDto AddBatches(ProductionOrderDto productionOrder, List<AssignBatchDto> batchesToAdd)
        {
            foreach (var component in productionOrder.ProductionOrderLines)
            {
                var batchesToAddInComponent = batchesToAdd.Where(x => x.ItemCode.Equals(component.ItemNo)).ToList();
                component.BatchNumbers.AddRange(batchesToAddInComponent.Select(x => new ProductionOrderItemBatchDto()
                {
                    BatchNumber = x.BatchNumber,
                    Quantity = x.AssignedQty,
                    ItemCode = x.ItemCode,
                    BaseLineNumber = component.LineNumber ?? 0,
                    SystemSerialNumber = x.SysNumber,
                }));
            }

            return productionOrder;
        }

        private static ProductionOrderDto DeleteBatches(ProductionOrderDto productionOrder, List<AssignBatchDto> batchesToDelete)
        {
            foreach (var component in productionOrder.ProductionOrderLines)
            {
                var batchesToDeleteInComponent = batchesToDelete.Where(x => x.ItemCode.Equals(component.ItemNo)).Select(x => x.BatchNumber).ToList();
                component.BatchNumbers = component.BatchNumbers.Where(x => !batchesToDeleteInComponent.Contains(x.BatchNumber)).ToList();
            }

            return productionOrder;
        }

        private static List<ProductionOrderLineDto> DeleteComponents(List<ProductionOrderLineDto> completeList, List<CompleteDetalleFormulaDto> componentsToDelete)
        {
            var deletedItems = completeList.Where(component => !componentsToDelete.Any(x => x.ProductId.Equals(component.ItemNo))).ToList();
            if (!deletedItems.Any())
            {
                componentsToDelete.RemoveAt(0);
            }

            return completeList.Where(component => !componentsToDelete.Any(x => x.ProductId.Equals(component.ItemNo))).ToList();
        }

        private static List<ProductionOrderLineDto> UpdateComponents(List<ProductionOrderLineDto> completeList, List<CompleteDetalleFormulaDto> componentsToUpdate)
        {
            completeList.ForEach(itemToUpdate =>
            {
                var updatedData = componentsToUpdate.FirstOrDefault(x => x.ProductId.Equals(itemToUpdate.ItemNo));
                if (updatedData != null)
                {
                    itemToUpdate.BaseQuantity = (double)updatedData.BaseQuantity;
                    itemToUpdate.PlannedQuantity = (double)updatedData.RequiredQuantity;
                    itemToUpdate.Warehouse = updatedData.Warehouse;
                }
            });

            return completeList;
        }

        private static List<ProductionOrderLineDto> AddComponents(List<ProductionOrderLineDto> completeList, List<CompleteDetalleFormulaDto> components, int orderId)
        {
            var existingItemCodes = new HashSet<string>(completeList.Select(c => c.ItemNo));

            completeList.AddRange(
                components
                    .Where(x => !existingItemCodes.Contains(x.ProductId))
                    .Select(component => new ProductionOrderLineDto
                    {
                        ItemNo = component.ProductId,
                        Warehouse = component.Warehouse,
                        BaseQuantity = (double)component.BaseQuantity,
                        PlannedQuantity = (double)component.RequiredQuantity,
                        DocumentAbsoluteEntry = orderId,
                        BatchNumbers = AssignedBatchesOnNewComponent(component),
                    }));

            return completeList;
        }

        private static List<ProductionOrderItemBatchDto> AssignedBatchesOnNewComponent(CompleteDetalleFormulaDto component)
        {
            if (component.AssignedBatches.ListIsNullOrEmpty())
            {
                return new List<ProductionOrderItemBatchDto>();
            }

            return component.AssignedBatches.Select(ab => new ProductionOrderItemBatchDto
            {
                BatchNumber = ab.BatchNumber,
                Quantity = ab.AssignedQty,
                ItemCode = component.ProductId,
                BaseLineNumber = 0,
                SystemSerialNumber = ab.SysNumber,
            }).ToList();
        }

        private static List<BatchNumbersDto> GetBatchNumbers(ProductionOrderLineDto productionOrderProducts)
        {
            return productionOrderProducts.BatchNumbers.Select(x => new BatchNumbersDto()
            {
                BatchNumber = x.BatchNumber,
                Quantity = x.Quantity,
            }).ToList();
        }

        private async Task<Dictionary<string, string>> ProcessToUpdateBatches(Dictionary<string, string> dictResult, IGrouping<int, AssignBatchDto> productionOrderGrouped)
        {
            var batches = productionOrderGrouped.Select(x => x).ToList();
            var responseGetProductionOrder = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderGrouped.Key));

            if (!responseGetProductionOrder.Success)
            {
                this.logger.Error($"Sap Service Layer Adapter - ProductionOrderService - The production order {productionOrderGrouped.Key} was not found.");
                dictResult.Add(
                    $"{productionOrderGrouped.Key}-{productionOrderGrouped.Key}",
                    $"{ServiceConstants.ErrorUpdateFabOrd}-{ServiceConstants.OrderNotFound}-{responseGetProductionOrder.UserError}-{responseGetProductionOrder.ExceptionMessage}");
                return dictResult;
            }

            var productionOrder = JsonConvert.DeserializeObject<ProductionOrderDto>(responseGetProductionOrder.Response.ToString());

            var batchesToDelete = batches.Where(x => x.Action.Equals(ServiceConstants.DeleteBatch)).ToList();
            var batchesToAdd = batches.Where(x => !x.Action.Equals(ServiceConstants.DeleteBatch)).ToList();
            productionOrder = DeleteBatches(productionOrder, batchesToDelete);
            productionOrder = AddBatches(productionOrder, batchesToAdd);

            var body = JsonConvert.SerializeObject(productionOrder);
            var result = await this.serviceLayerClient.PutAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderGrouped.Key), body);

            dictResult = GetDictionaryResult(dictResult, result.Code, result.UserError, batches, productionOrderGrouped.Key, result.Success);
            if (!result.Success)
            {
                this.logger.Error($"Error updating production order {productionOrderGrouped.Key} - Error: {result.Code} - {result.UserError}");
            }

            return dictResult;
        }

        private async Task CreateReceiptFromProductionOrderId(int productionOrderId, CloseProductionOrderDto closeConfiguration, ProductionOrderDto productionOrder, string logBase)
        {
            var receiptProduction = new InventoryGenEntryDto();
            var separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            closeConfiguration.Batches = closeConfiguration.Batches ?? new List<BatchesConfigurationDto>();

            var quantityToReceipt = productionOrder.PlannedQuantity;
            receiptProduction.DocumentLines = new List<InventoryGenEntryLineDto>();
            var line = new InventoryGenEntryLineDto();
            line.BatchNumbers = new List<BatchInventoryGenEntryDto>();

            var product = await this.GetFromServiceLayer<ItemDto>(
                string.Format(ServiceQuerysConstants.QryProductById, productionOrder.ItemNo),
                $"{string.Format(ServiceConstants.FailGetProduct, productionOrder.ItemNo)}");

            if (product.ManageBatchNumbers == ServiceConstants.ManageBatchNumbers)
            {
                this.logger.Information(LogsConstants.LogBatchesQuantityWithDecimalSeparator, logBase, separator);
                this.logger.Information(LogsConstants.LogBatchesSum, logBase, closeConfiguration.Batches.Sum(x => double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator))));

                quantityToReceipt = closeConfiguration.Batches.Sum(x => double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator)));
                foreach (var batchConfig in closeConfiguration.Batches)
                {
                    line.BatchNumbers.Add(
                        new BatchInventoryGenEntryDto
                        {
                            BatchNumber = batchConfig.BatchCode,
                            ManufacturingDate = batchConfig.ManufacturingDate,
                            ExpiryDate = batchConfig.ExpirationDate,
                            Quantity = double.Parse(System.Text.RegularExpressions.Regex.Replace(batchConfig.Quantity, "[.,]", separator)),
                        });
                }
            }

            line.BaseEntry = productionOrder.DocumentNumber;
            line.BaseType = 202;
            line.Quantity = quantityToReceipt;
            line.TransactionType = ServiceConstants.InventoryGenEntryLineTransactionType;
            line.WarehouseCode = productionOrder.Warehouse;
            receiptProduction.DocumentLines.Add(line);

            await this.SaveInventoryGenEntry(receiptProduction, productionOrderId, logBase);
        }

        private async Task SaveInventoryGenEntry(InventoryGenEntryDto data, int productionOrderId, string logBase)
        {
            var response = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryPostInventoryGenEntries, JsonConvert.SerializeObject(data));
            if (!response.Success)
            {
                this.logger.Error(LogsConstants.AnErrorOcurredOnSaveReceiptProduction, logBase, response.Code, response.UserError);
                throw new CustomServiceException($"{string.Format(ServiceConstants.FailReasonNotReceipProductionCreated, productionOrderId)} - {response.UserError}");
            }
        }

        private async Task CloseProductionOrder(int productionOrderId, ProductionOrderDto productionOrder, string logBase)
        {
            productionOrder.ProductionOrderStatus = ServiceConstants.ProductionOrderClosedStatus;

            var response = await this.serviceLayerClient.PatchAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId), JsonConvert.SerializeObject(productionOrder));
            if (!response.Success)
            {
                this.logger.Error(LogsConstants.AnErrorOcurredOnUpdateProductionOrderStatus, logBase, response.Code, response.UserError);
                throw new CustomServiceException($"{string.Format(ServiceConstants.FailReasonNotProductionStatusClosed, productionOrder.DocumentNumber)} - {response.UserError}");
            }
        }

        private async Task CreateInventoryGenExit(ProductionOrderDto productionOrder, int productionOrderId, string logBase)
        {
            var filteredProductionOrderLines = productionOrder.ProductionOrderLines.Where(x => x.ProductionOrderIssueType.Equals(ServiceConstants.ProductionOrderTypeM)).ToList();

            var inventoryGenExit = new InventoryGenExitDto
            {
                InventoryGenExitLines = new List<InventoryGenExitLineDto>(),
            };

            foreach (var productOrderLine in filteredProductionOrderLines)
            {
                if (productOrderLine.IssuedQuantity != 0)
                {
                    this.logger.Error(LogsConstants.ComponentAlreadyHasConsumedQuantity, logBase, productOrderLine.ItemNo, productOrderLine.PlannedQuantity, productOrderLine.IssuedQuantity);
                    throw new CustomServiceException($"{string.Format(ServiceConstants.FailConsumedQuantity, productionOrderId)}");
                }

                var line = new InventoryGenExitLineDto();
                line.BaseType = 202;
                line.BaseEntry = productionOrderId;
                line.BaseLine = productOrderLine.LineNumber ?? 0;
                line.Quantity = productOrderLine.PlannedQuantity;
                line.WarehouseCode = productOrderLine.Warehouse;
                line.BatchNumbers = GetBatchNumbers(productOrderLine);
                inventoryGenExit.InventoryGenExitLines.Add(line);
            }

            if (filteredProductionOrderLines.Count > 0)
            {
                await this.SaveInventoryGenExit(inventoryGenExit, productionOrderId, logBase);
            }
        }

        private async Task SaveInventoryGenExit(InventoryGenExitDto inventoryGen, int productionOrderId, string logBase)
        {
            var response = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryPostInventoryGenExists, JsonConvert.SerializeObject(inventoryGen));
            if (!response.Success)
            {
                this.logger.Error(LogsConstants.ErrorOcurredOnCreateInventoryGenExit, logBase, response.Code, response.UserError);
                throw new CustomServiceException($"{string.Format(ServiceConstants.FailReasonNotGetExitCreated, productionOrderId)} - {response.UserError}");
            }
        }

        private async Task ValidateRequiredQuantityForRetroactiveIssues(ProductionOrderDto productionOrder)
        {
            var filteredProductionOrderLines = productionOrder.ProductionOrderLines.Where(x => x.ProductionOrderIssueType.Equals(ServiceConstants.ProductionOrderTypeB)).ToList();
            var missingComponents = new List<string>();

            foreach (var productLine in filteredProductionOrderLines)
            {
                if (productLine.IssuedQuantity != 0)
                {
                    throw new CustomServiceException($"{string.Format(ServiceConstants.FailConsumedQuantity, productionOrder.AbsoluteEntry)}");
                }

                var isAvailableRequredQuantity = await this.IsAvailableRequiredQuantity(productLine.ItemNo, productLine.Warehouse, productLine.PlannedQuantity);
                if (!isAvailableRequredQuantity)
                {
                    missingComponents.Add(productLine.ItemNo);
                }
            }

            if (!missingComponents.Any())
            {
                return;
            }

            var formated = string.Join(", ", missingComponents);
            throw new CustomServiceException(string.Format(ServiceConstants.FailReasonNotAvailableRequiredQuantity, productionOrder.AbsoluteEntry, formated));
        }

        private async Task<bool> IsAvailableRequiredQuantity(string itemCode, string warehouse, double requiredQuantity)
        {
            var item = await this.GetFromServiceLayer<ItemDto>(string.Format(ServiceQuerysConstants.QryProductById, itemCode), $"{string.Format(ServiceConstants.FailGetProduct, itemCode)}");
            var availableQuantity = item.ItemWarehouseInfoCollection.Where(x => x.WarehouseCode.Equals(warehouse)).Sum(x => x.InStock);
            return requiredQuantity < availableQuantity;
        }

        private async Task ValidateNewBatches(string itemCode, List<BatchesConfigurationDto> batches)
        {
            if (batches == null)
            {
                return;
            }

            foreach (var batche in batches)
            {
                var batchDetail = await this.GetFromServiceLayer<BatchNumberResponseDto>(string.Format(ServiceQuerysConstants.QryBatchNumbers, itemCode, batche.BatchCode), $"{string.Format(ServiceConstants.FailGetProduct, itemCode)}");
                if (batchDetail.Results.Count != 0)
                {
                    throw new CustomServiceException(string.Format(ServiceConstants.FailReasonBatchAlreadyExists, batche.BatchCode, itemCode));
                }
            }
        }

        private async Task<T> GetFromServiceLayer<T>(string url, string errorMessage)
        {
            var response = await this.serviceLayerClient.GetAsync(url);
            if (!response.Success)
            {
                this.logger.Error(response.UserError);
                throw new CustomServiceException(errorMessage);
            }

            return JsonConvert.DeserializeObject<T>(response.Response.ToString());
        }

        private async Task SaveChanges(UpdateProductionOrderDto productionOrder, int id)
        {
            var body = JsonConvert.SerializeObject(productionOrder);
            var result = await this.serviceLayerClient.PutAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, id), body);

            if (!result.Success)
            {
                this.logger.Error(result.UserError);
                throw new CustomServiceException(result.UserError);
            }
        }

        private async Task EditFabOrder(ProductionOrderDto productionOrder, int orderId)
        {
            var body = JsonConvert.SerializeObject(productionOrder);
            var result = await this.serviceLayerClient.PutAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, orderId), body);
            if (!result.Success)
            {
                throw new CustomServiceException(string.Format("{0}-{1}-{2}", ServiceConstants.ErrorUpdateFabOrd, result.UserError, result.UserError));
            }
        }

        private async Task ExecuteFinalizationStepAsync(CloseProductionOrderDto productionOrder, string logBase)
        {
            switch (productionOrder.LastStep?.Trim())
            {
                case null:
                case ServiceConstants.EmptyValue:
                case ServiceConstants.PrimaryValidationsStep:
                    await this.ExecuteFullFinalizationFlow(productionOrder, logBase);
                    break;
                case ServiceConstants.CreateInventoryStep:
                    await this.ExecuteFinalizationFromCreateReceiptStep(productionOrder, logBase);
                    break;
                case ServiceConstants.CreateReceiptStep:
                    await this.ExecuteFinalizationFromCloseProductionOrderStep(productionOrder, logBase);
                    break;
                default:
                    this.logger.Error(LogsConstants.StepNotRecognized, logBase, productionOrder.LastStep);
                    break;
            }
        }

        private async Task ExecuteFullFinalizationFlow(CloseProductionOrderDto productionOrder, string logBase)
        {
            var productionOrderSap = await this.GetProductionOrder(productionOrder.ProductionOrderId);
            this.logger.Information(LogsConstants.CreateInventoryGenExit, logBase, productionOrder.ProductionOrderId);
            await this.CreateInventoryGenExit(productionOrderSap, productionOrder.ProductionOrderId, logBase);
            productionOrder.LastStep = ServiceConstants.CreateInventoryStep;
            var updatedOrder = await this.GetProductionOrder(productionOrder.ProductionOrderId);
            this.logger.Information(LogsConstants.CreateReceiptFromProductionOrderId, logBase, productionOrder.ProductionOrderId);
            await this.CreateReceiptFromProductionOrderId(productionOrder.ProductionOrderId, productionOrder, updatedOrder, logBase);
            productionOrder.LastStep = ServiceConstants.CreateReceiptStep;
            this.logger.Information(LogsConstants.CloseProductionOrder, logBase, productionOrder.ProductionOrderId);
            await this.CloseProductionOrder(productionOrder.ProductionOrderId, updatedOrder, logBase);
            productionOrder.LastStep = ServiceConstants.SuccessfullyClosedInSapStep;
        }

        private async Task ExecuteFinalizationFromCreateReceiptStep(CloseProductionOrderDto productionOrder, string logBase)
        {
            var updatedOrder = await this.GetProductionOrder(productionOrder.ProductionOrderId);
            this.logger.Information(LogsConstants.CreateReceiptFromProductionOrderId, logBase, productionOrder.ProductionOrderId);
            await this.CreateReceiptFromProductionOrderId(productionOrder.ProductionOrderId, productionOrder, updatedOrder, logBase);
            productionOrder.LastStep = ServiceConstants.CreateReceiptStep;
            this.logger.Information(LogsConstants.CloseProductionOrder, logBase, productionOrder.ProductionOrderId);
            await this.CloseProductionOrder(productionOrder.ProductionOrderId, updatedOrder, logBase);
            productionOrder.LastStep = ServiceConstants.SuccessfullyClosedInSapStep;
        }

        private async Task ExecuteFinalizationFromCloseProductionOrderStep(CloseProductionOrderDto productionOrder, string logBase)
        {
            var updatedOrder = await this.GetProductionOrder(productionOrder.ProductionOrderId);
            this.logger.Information(LogsConstants.CloseProductionOrder, logBase, productionOrder.ProductionOrderId);
            await this.CloseProductionOrder(productionOrder.ProductionOrderId, updatedOrder, logBase);
            productionOrder.LastStep = ServiceConstants.SuccessfullyClosedInSapStep;
        }

        private async Task<ProductionOrderDto> GetProductionOrder(int productionOrderId)
        {
            return await this.GetFromServiceLayer<ProductionOrderDto>(
                        string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId),
                        string.Format(LogsConstants.NotFoundProductionOrder, productionOrderId));
        }
    }
}