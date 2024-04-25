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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrderService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">The serviceLayerClient.</param>
        /// <param name="logger">The logger.</param>
        public ProductionOrderService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinishOrder(List<CloseProductionOrderDto> productionOrders)
        {
            var results = new Dictionary<int, string>();
            foreach (var productionOrderConfig in productionOrders)
            {
                var productionOrderId = productionOrderConfig.OrderId;
                try
                {
                    this.logger.Information($"Trying to finish production order {productionOrderId}");
                    var productionOrder = await this.GetFromServiceLayer<ProductionOrderDto>(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId), $"La orden de produción {productionOrderId} no existe.");
                    if (productionOrder.ProductionOrderStatus.Equals(ServiceConstants.ProductionOrderClosed))
                    {
                        continue;
                    }

                    if (!productionOrder.ProductionOrderStatus.Equals(ServiceConstants.ProductionOrderReleased))
                    {
                        results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotReleasedProductionOrder, productionOrderId));
                    }

                    this.logger.Information($"Validating production order {productionOrderId}");
                    await this.ValidateRequiredQuantityForRetroactiveIssues(productionOrder);
                    await this.ValidateNewBatches(productionOrder.ItemNo, productionOrderConfig.Batches);
                    await this.CreateInventoryGenExit(productionOrder, productionOrderId);

                    var productionOrderUpdated = await this.GetFromServiceLayer<ProductionOrderDto>(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId), $"La orden de produción {productionOrderId} no existe.");
                    await this.CreateReceiptFromProductionOrderId(productionOrderId, productionOrderConfig, productionOrderUpdated);
                    await this.CloseProductionOrder(productionOrderId, productionOrderUpdated);
                }
                catch (CustomServiceException ex)
                {
                    this.logger.Error(ex.Message, ex);
                    results.Add(productionOrderId, ex.Message);
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex.StackTrace, ex);
                    results.Add(productionOrderId, ServiceConstants.FailReasonUnexpectedError);
                }
            }

            if (!results.Any())
            {
                results.Add(0, "Ok");
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

                await this.SaveChanges(productionOrder, updateFormula.FabOrderId);
            }
            catch (Exception ex)
            {
                dictResult.Add($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", ex.Message);
                return ServiceUtils.CreateResult(false, 400, null, dictResult, null);
            }

            dictResult.Add($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", "Ok");
            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        private static List<ProductionOrderLineDto> DeleteComponents(List<ProductionOrderLineDto> completeList, List<CompleteDetalleFormulaDto> componentsToDelete)
        {
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
            var componentsToAdd = components.Where(x => !completeList.Any(c => c.ItemNo.Equals(x.ProductId))).ToList();
            foreach (var component in componentsToAdd)
            {
                var newComponent = new ProductionOrderLineDto();
                newComponent.ItemNo = component.ProductId;
                newComponent.Warehouse = component.Warehouse;
                newComponent.BaseQuantity = (double)component.BaseQuantity;
                newComponent.PlannedQuantity = (double)component.RequiredQuantity;
                newComponent.DocumentAbsoluteEntry = orderId;
                newComponent.BatchNumbers = new List<ProductionOrderItemBatchDto>();
                completeList.Add(newComponent);
            }

            return completeList;
        }

        private async Task CreateReceiptFromProductionOrderId(int productionOrderId, CloseProductionOrderDto closeConfiguration, ProductionOrderDto productionOrder)
        {
            this.logger.Information($"Create oInventoryGenEntry for {productionOrderId}");
            var receiptProduction = new InventoryGenEntryDto();
            var separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            closeConfiguration.Batches = closeConfiguration.Batches ?? new List<BatchesConfigurationDto>();

            var quantityToReceipt = productionOrder.PlannedQuantity;
            receiptProduction.DocumentLines = new List<InventoryGenEntryLineDto>();
            var line = new InventoryGenEntryLineDto();
            line.BatchNumbers = new List<BatchInventoryGenEntryDto>();

            var product = await this.GetFromServiceLayer<ItemDto>(string.Format(ServiceQuerysConstants.QryProductById, productionOrder.ItemNo), $"{string.Format(ServiceConstants.FailGetProduct, productionOrder.ItemNo)}");
            if (product.ManageBatchNumbers.Equals("tYES"))
            {
                this.logger.Information($"Log batches quantity with decimal separator {separator}.");
                this.logger.Information($"Sum {closeConfiguration.Batches.Sum(x => double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator)))}.");

                quantityToReceipt = closeConfiguration.Batches.Sum(x => double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator)));
                foreach (var batchConfig in closeConfiguration.Batches)
                {
                    var batchNumber = new BatchInventoryGenEntryDto();
                    batchNumber.BatchNumber = batchConfig.BatchCode;
                    batchNumber.ManufacturingDate = batchConfig.ManufacturingDate;
                    batchNumber.ExpiryDate = batchConfig.ExpirationDate;
                    batchNumber.Quantity = double.Parse(System.Text.RegularExpressions.Regex.Replace(batchConfig.Quantity, "[.,]", separator));
                    line.BatchNumbers.Add(batchNumber);
                }
            }

            line.BaseEntry = productionOrder.DocumentNumber;
            line.BaseType = 202;
            line.Quantity = quantityToReceipt;
            line.TransactionType = "botrntComplete";
            line.WarehouseCode = productionOrder.Warehouse;
            receiptProduction.DocumentLines.Add(line);

            await this.SaveInventoryGenEntry(receiptProduction, productionOrderId);
        }

        private async Task SaveInventoryGenEntry(InventoryGenEntryDto data, int productionOrderId)
        {
            var response = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryPostInventoryGenEntries, JsonConvert.SerializeObject(data));
            if (!response.Success)
            {
                this.logger.Error($"An error has ocurred on save receipt production {response.Code} - {response.UserError}.");
                throw new CustomServiceException($"{string.Format(ServiceConstants.FailReasonNotReceipProductionCreated, productionOrderId)} - {response.UserError}");
            }
        }

        private async Task CloseProductionOrder(int productionOrderId, ProductionOrderDto productionOrder)
        {
            this.logger.Information($"Close production order {productionOrderId}.");
            productionOrder.ProductionOrderStatus = "boposClosed";

            var response = await this.serviceLayerClient.PatchAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, productionOrderId), JsonConvert.SerializeObject(productionOrder));
            if (!response.Success)
            {
                this.logger.Error($"An error has ocurred on update production order status {response.Code} - {response.UserError}.");
                throw new CustomServiceException($"{string.Format(ServiceConstants.FailReasonNotProductionStatusClosed, productionOrder.DocumentNumber)} - {response.UserError}");
            }
        }

        private async Task CreateInventoryGenExit(ProductionOrderDto productionOrder, int productionOrderId)
        {
            this.logger.Information($"Create oInventoryGenExit for {productionOrderId}");
            var filteredProductionOrderLines = productionOrder.ProductionOrderLines.Where(x => x.ProductionOrderIssueType.Equals(ServiceConstants.ProductionOrderTypeM)).ToList();

            var inventoryGenExit = new InventoryGenExitDto();
            inventoryGenExit.InventoryGenExitLines = new List<InventoryGenExitLineDto>();
            foreach (var productOrderLine in filteredProductionOrderLines)
            {
                if (productOrderLine.IssuedQuantity != 0)
                {
                    this.logger.Information($"[VALIDATE CONSUMED QUANTITY] the component already has consumed quantity: {productOrderLine.ItemNo}, required  {productOrderLine.PlannedQuantity} , consumed: {productOrderLine.IssuedQuantity}.");
                    throw new CustomServiceException($"{string.Format(ServiceConstants.FailConsumedQuantity, productionOrderId)}");
                }

                var line = new InventoryGenExitLineDto();
                line.BaseType = 202;
                line.BaseEntry = productionOrderId;
                line.BaseLine = productOrderLine.LineNumber ?? 0;
                line.Quantity = productOrderLine.PlannedQuantity;
                line.WarehouseCode = productOrderLine.Warehouse;
                line.BatchNumbers = this.GetBatchNumbers(productOrderLine);
                inventoryGenExit.InventoryGenExitLines.Add(line);
            }

            if (filteredProductionOrderLines.Count > 0)
            {
                await this.SaveInventoryGenExit(inventoryGenExit, productionOrderId);
            }
        }

        private List<BatchNumbersDto> GetBatchNumbers(ProductionOrderLineDto productionOrderProducts)
        {
            return productionOrderProducts.BatchNumbers.Select(x => new BatchNumbersDto()
            {
                BatchNumber = x.BatchNumber,
                Quantity = x.Quantity,
            }).ToList();
        }

        private async Task SaveInventoryGenExit(InventoryGenExitDto inventoryGen, int productionOrderId)
        {
            var response = await this.serviceLayerClient.PostAsync(ServiceQuerysConstants.QryPostInventoryGenExists, JsonConvert.SerializeObject(inventoryGen));
            if (!response.Success)
            {
                this.logger.Error($"An error has ocurred on create oInventoryGenExit {response.Code} - {response.UserError}.");
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
                throw new CustomServiceException(errorMessage);
            }

            return JsonConvert.DeserializeObject<T>(response.Response.ToString());
        }

        private async Task SaveChanges(ProductionOrderDto productionOrder, int id)
        {
            var body = JsonConvert.SerializeObject(productionOrder);
            var result = await this.serviceLayerClient.PutAsync(string.Format(ServiceQuerysConstants.QryProductionOrderById, id), body);

            if (!result.Success)
            {
                throw new CustomServiceException(result.UserError);
            }
        }
    }
}