// <summary>
// <copyright file="ISapDiApiService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Newtonsoft.Json;
    using Omicron.SapDiApi.Entities.Context;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Services.Constants;
    using Omicron.SapDiApi.Services.Utils;
    using SAPbobsCOM;
    using Omicron.SapDiApi.Log;
    using Omicron.LeadToCash.Resources.Exceptions;

    /// <summary>
    /// clas for the data to sap.
    /// </summary>
    public class SapDiApiService : ISapDiApiService
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDiApiService"/> class.
        /// </summary>   
        public SapDiApiService(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <summary>
        /// Connects to SAP.
        /// </summary>
        /// <returns>the connection.</returns>
        public async Task<ResultModel> Connect()
        {
            var connected = this.company.Connected;
            _loggerProxy.Info($"SAP connection is: {connected}");
            return ServiceUtils.CreateResult(true, 200, null, connected, null);
        }

        /// <summary>
        /// the insert.
        /// </summary>
        /// <param name="orderWithDetail">the list of data.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> InsertOrdenFab(List<OrderWithDetailModel> orderWithDetail)
        {
            var dictResult = new Dictionary<string, string>();
            foreach(var pedido in orderWithDetail)
            {
                var count = 0;

                foreach (var orf in pedido.Detalle)
                {
                    
                    var plannedQty = orf.QtyPlannedDetalle == null ? "0" : orf.QtyPlannedDetalle.ToString();
                    double.TryParse(plannedQty, out double plannedQtyNumber);

                    var prodObj = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
                    prodObj.StartDate = pedido.Order.FechaInicio;
                    prodObj.DueDate = pedido.Order.FechaFin;
                    prodObj.ItemNo = orf.CodigoProducto;
                    prodObj.ProductDescription = orf.DescripcionProducto;
                    prodObj.PlannedQuantity = plannedQtyNumber;
                    prodObj.ProductionOrderOriginEntry = pedido.Order.PedidoId;                    

                    var inserted =  prodObj.Add();

                    if(inserted != 0)
                    {
                        company.GetLastError(out int errorCode, out string errMsg);
                        dictResult.Add(string.Format("{0}-{1}-{2}", pedido.Order.PedidoId, orf.CodigoProducto, count), string.Format("{0}-{1}-{2}", ServiceConstants.ErrorCreateFabOrd, errorCode.ToString(), errMsg));
                        _loggerProxy.Info($"The next order was tried to be created: {errorCode} - {errMsg} - {pedido.Order.PedidoId}");
                    }
                    else
                    {
                        dictResult.Add(string.Format("{0}-{1}-{2}", pedido.Order.PedidoId, orf.CodigoProducto, count), "Ok");
                    }

                    count++;
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictResult), null);
        }

        /// <summary>
        /// Updates the fabrication orders.
        /// </summary>
        /// <param name="orderModels">the models to update.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> UpdateFabOrders(List<UpdateFabOrderModel> orderModels)
        {
            var dictResult = new Dictionary<string, string>();
            foreach (var order in orderModels)
            {
                var productionOrderObj = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
                var orderFab = productionOrderObj.GetByKey(order.OrderFabId);

                if (orderFab)
                {
                    productionOrderObj = this.UpdateEntry(order, productionOrderObj);
                    var updated = productionOrderObj.Update();

                    if (updated != 0)
                    {
                        company.GetLastError(out int errorCode, out string errMsg);
                        dictResult.Add(string.Format("{0}-{1}", order.OrderFabId, order.OrderFabId), string.Format("{0}-{1}-{2}", ServiceConstants.ErrorUpdateFabOrd, errorCode.ToString(), errMsg));
                        _loggerProxy.Info($"The next order was tried to be updated: {errorCode} - {errMsg} - {order.OrderFabId}");
                    }
                    else
                    {
                        dictResult.Add(string.Format("{0}-{1}", order.OrderFabId, order.OrderFabId), "Ok");
                    }
                }
                else
                {
                    dictResult.Add(string.Format("{0}", order.OrderFabId), string.Format("{0}-{1}", ServiceConstants.ErrorUpdateFabOrd, ServiceConstants.OrderNotFound));
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the formula.</param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateFormula(UpdateFormulaModel updateFormula)
        {
            var dictResult = new Dictionary<string, string>();
            var productionOrderObj = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
            var orderFab = productionOrderObj.GetByKey(updateFormula.FabOrderId);

            if (!orderFab)
            {
                dictResult = this.AddResult($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", $"{ServiceConstants.ErrorUpdateFabOrd}-{ServiceConstants.OrderNotFound}", -1, company, dictResult);
                return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
            }

            if (!this.DeleteComponentsToFormula(updateFormula))
            {
                dictResult = this.AddResult($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", ServiceConstants.ErrorUpdateFabOrd, -1, company, dictResult);
                return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
            }

            updateFormula.Components = updateFormula.Components.Where(x => !x.Action.Equals(ServiceConstants.DeleteComponent)).ToList();

            // Reload fab order.
            productionOrderObj = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
            productionOrderObj.GetByKey(updateFormula.FabOrderId);

            productionOrderObj.DueDate = updateFormula.FechaFin;
            productionOrderObj.PlannedQuantity = updateFormula.PlannedQuantity;
            productionOrderObj.Warehouse = updateFormula.Warehouse;

            var components = this.ExecuteQuery(ServiceConstants.FindWor1ByDocEntry, updateFormula.FabOrderId);

            var listIdsUpdated = new List<string>();
            var counter = 0;

            if (components.RecordCount != 0)
            {
                counter = components.RecordCount;
                for (var i = 0; i < components.RecordCount; i++)
                {
                    var sapItemCode = components.Fields.Item("ItemCode").Value;
                    var lineNum = components.Fields.Item("VisOrder").Value;

                    try
                    {
                        productionOrderObj.Lines.SetCurrentLine(lineNum);
                    }
                    catch(Exception ex)
                    {
                        continue;
                    }

                    var component = updateFormula.Components.FirstOrDefault(x => x.ProductId.Equals(sapItemCode));

                    if (component == null)
                    {
                        components.MoveNext();
                        continue;
                    } 

                    _loggerProxy.Info($"Item to update: { sapItemCode } on line { lineNum }.");

                    double.TryParse(component.BaseQuantity.ToString(), out double baseQuantity);
                    double.TryParse(component.RequiredQuantity.ToString(), out double issuedQuantity);
                    productionOrderObj.Lines.BaseQuantity = baseQuantity;
                    productionOrderObj.Lines.PlannedQuantity = issuedQuantity;
                    productionOrderObj.Lines.Warehouse = component.Warehouse;
                    listIdsUpdated.Add(sapItemCode);
                    components.MoveNext();
                }
            }

            var listNotInserted = updateFormula.Components.Where(x => !listIdsUpdated.Contains(x.ProductId));
            
            foreach (var line in listNotInserted)
            {
                _loggerProxy.Info($"Item to insert: { line.ProductId } on line { counter }.");

                double.TryParse(line.BaseQuantity.ToString(), out double baseQuantity);
                double.TryParse(line.RequiredQuantity.ToString(), out double issuedQuantity);
                productionOrderObj.Lines.Add();
                productionOrderObj.Lines.SetCurrentLine(counter);
                productionOrderObj.Lines.ItemNo = line.ProductId;
                productionOrderObj.Lines.Warehouse = line.Warehouse;
                productionOrderObj.Lines.BaseQuantity = baseQuantity;
                productionOrderObj.Lines.PlannedQuantity = issuedQuantity;
                counter += 1;
            }

            var updated = productionOrderObj.Update();

            if (updated != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                _loggerProxy.Info($"The next order was tried to be updated: {errorCode} - {errMsg} - {JsonConvert.SerializeObject(updateFormula)}");
            }

            dictResult = this.AddResult($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", ServiceConstants.ErrorUpdateFabOrd, updated, company, dictResult);
            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <summary>
        /// Delete components to formula.
        /// </summary>
        /// <param name="updateFormula">the formula.</param>
        /// <returns></returns>
        public bool DeleteComponentsToFormula(UpdateFormulaModel updateFormula)
        {
            var productionOrderObj = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
            productionOrderObj.GetByKey(updateFormula.FabOrderId);

            double.TryParse(updateFormula.PlannedQuantity.ToString(), out double plannedQuantity);
            productionOrderObj.DueDate = updateFormula.FechaFin;
            productionOrderObj.PlannedQuantity = plannedQuantity;
            productionOrderObj.Warehouse = updateFormula.Warehouse;

            var components = this.ExecuteQuery(ServiceConstants.FindWor1ByDocEntry, updateFormula.FabOrderId);
            var linesToDelete = new List<int>();
            
            if (components.RecordCount != 0)
            {
                for (var i = 0; i < components.RecordCount; i++)
                {
                    var sapItemCode = components.Fields.Item("ItemCode").Value;
                    var lineNum = components.Fields.Item("VisOrder").Value;

                    try
                    {
                        productionOrderObj.Lines.SetCurrentLine(lineNum);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    var component = updateFormula.Components.FirstOrDefault(x => x.ProductId.Equals(sapItemCode) && x.Action.Equals(ServiceConstants.DeleteComponent));
                    var anotherComponent = updateFormula.Components.FirstOrDefault(x => x.ProductId.Equals(sapItemCode) && !x.Action.Equals(ServiceConstants.DeleteComponent));

                    if (component != null && component.Action.Equals(ServiceConstants.DeleteComponent) && anotherComponent == null)
                    {
                        _loggerProxy.Info($"Item to delete: { sapItemCode } on line { lineNum }.");
                        linesToDelete.Add(lineNum);
                    }

                    components.MoveNext();
                }
            }
 
            foreach (var lineToDelete in linesToDelete.OrderByDescending(x => x))
            {
                productionOrderObj.Lines.SetCurrentLine(lineToDelete);
                productionOrderObj.Lines.Delete();
            }

            var updatedResult = productionOrderObj.Update();

            if (updatedResult != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                _loggerProxy.Info($"The next order was tried to be updated: {errorCode} - {errMsg} - {JsonConvert.SerializeObject(updateFormula)}");
            }

            return updatedResult == 0;
        }

        /// <summary>
        /// Cancel a prodution order
        /// </summary>
        /// <param name="productionOrder">Production order to update</param>
        /// <returns>the data.</returns
        public async Task<ResultModel> CancelProductionOrder(CancelOrderModel productionOrder)
        {
            _loggerProxy.Debug($"Production order to cancel: {productionOrder.OrderId}.");
            var orderReference = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);

            if (!orderReference.GetByKey(productionOrder.OrderId))
            {
                _loggerProxy.Debug($"The production order {productionOrder.OrderId} doesn´t exists.");
                return ServiceUtils.CreateResult(true, 200, null, ServiceConstants.NotFound, null);
            }

            if (orderReference.ProductionOrderStatus == BoProductionOrderStatusEnum.boposCancelled)
            {
                _loggerProxy.Debug($"The production order {productionOrder.OrderId} is cancelled.");
                return ServiceUtils.CreateResult(true, 200, null, ServiceConstants.ErrorProductionOrderCancelled, null);
            }

            // Cancel production order
            orderReference.ProductionOrderStatus = BoProductionOrderStatusEnum.boposCancelled;

            if (!orderReference.Update().Equals(0))
            {
                company.GetLastError(out int errorCode, out string errorMessage);
                _loggerProxy.Debug($"The production order {productionOrder.OrderId} cancellation failed, {errorCode} - {errorMessage}.");
                return ServiceUtils.CreateResult(true, 200, null, ServiceConstants.UnexpectedError, $"{errorCode} - {errorMessage}");
            }

            _loggerProxy.Debug($"The production order {productionOrder.OrderId} cancelled succesfuly.");
            return ServiceUtils.CreateResult(true, 200, null, ServiceConstants.Ok, null);
        }

        /// <summary>
        /// The method to update batches.
        /// </summary>
        /// <param name="updateBatches">the update batches.</param>
        /// <returns>the batches updated.</returns>
        public async Task<ResultModel> UpdateBatches(List<AssignBatchModel> updateBatches)
        {
            var dictResult = new Dictionary<string, string>();
            var listyGrouped = updateBatches.GroupBy(x => x.OrderId).ToList();

            foreach (var group in listyGrouped)
            {
                var productionOrderObj = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
                var orderFab = productionOrderObj.GetByKey(group.Key);

                if (!orderFab)
                {
                    _loggerProxy.Info($"The production order {group.Key} was not found.");
                    dictResult = this.AddResult($"{group.Key}-{group.Key}", $"{ServiceConstants.ErrorUpdateFabOrd}-{ServiceConstants.OrderNotFound}", -1, company, dictResult);
                    continue;
                }

                var components = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                components.DoQuery(string.Format(ServiceConstants.FindWor1ByDocEntry, group.Key));

                for (var i = 0; i < components.RecordCount; i++)
                {
                    var lineNum = components.Fields.Item("VisOrder").Value;
                    var itemCode = components.Fields.Item("ItemCode").Value;

                    if (!group.Any(x => x.ItemCode == itemCode))
                    {
                        components.MoveNext();
                        continue;
                    }

                    var lastError = 0;
                    productionOrderObj.Lines.SetCurrentLine(lineNum);
                    group
                        .Where(x => x.ItemCode == itemCode)
                        .GroupBy(z => z.Action)
                        .OrderBy(a => a.Key)
                        .ToList()
                        .ForEach(sg => 
                        {
                            sg
                            .ToList()
                            .ForEach(z =>
                            {
                                productionOrderObj.Lines.BatchNumbers.Add();
                                productionOrderObj.Lines.BatchNumbers.Quantity = z.Action.Equals(ServiceConstants.DeleteBatch) ? -z.AssignedQty : z.AssignedQty;
                                productionOrderObj.Lines.BatchNumbers.BatchNumber = z.BatchNumber;                                                                
                            });
                            
                            var updated = productionOrderObj.Update();

                            if (updated != 0)
                            {
                                lastError = updated;
                            }
                            
                            company.GetLastError(out var error, out var lastMsg);
                            _loggerProxy.Info($"The next Batch was tried to be assign with status {error} - {lastMsg} - {group.Key}-{JsonConvert.SerializeObject(sg)}");
                        });

                    dictResult = this.AddResult($"{group.Key}-{itemCode}-{i}", ServiceConstants.ErrorUpdateFabOrd, lastError, company, dictResult);

                    components.MoveNext();                    
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
        }

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        public ResultModel FinishOrder(List<CloseProductionOrderModel> productionOrders)
        {
            var results = new Dictionary<int, string>();
            foreach (var productionOrderConfig in productionOrders)
            {
                var productionOrderId = productionOrderConfig.OrderId;
                try
                {
                    var orderReference = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);

                    if (!orderReference.GetByKey(productionOrderConfig.OrderId))
                    {
                        results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotExistsProductionOrder, productionOrderId));
                    }

                    if (orderReference.ProductionOrderStatus == BoProductionOrderStatusEnum.boposClosed)
                    {
                        continue;
                    }

                    if (orderReference.ProductionOrderStatus != BoProductionOrderStatusEnum.boposReleased)
                    {
                        results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotReleasedProductionOrder, productionOrderId));
                    }

                    this.ValidateRequiredQuantityForRetroactiveIssues(productionOrderId);
                    
                    this.ValidateNewBatches(orderReference.ItemNo, productionOrderConfig.Batches);

                    this.CreateoGoodIssueForProductionByOrderId(productionOrderId);
                    
                    this.CreateReceiptFromProductionOrderId(productionOrderId, productionOrderConfig);
                    
                    this.CloseProductionOrder(productionOrderId);
                }
                catch (ValidationException ex)
                {
                    _loggerProxy.Error(ex.Message, ex);
                    results.Add(productionOrderId, ex.Message);
                }
                catch (Exception ex)
                {
                    _loggerProxy.Error(ex.StackTrace, ex);
                    results.Add(productionOrderId, ServiceConstants.FailReasonUnexpectedError);
                }
            }

            if (!results.Any())
            {
                results.Add(0, ServiceConstants.Ok);
            }

            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderModel isolatedFabOrder)
        {
            var result = new KeyValuePair<string, string>();
            var item = this.GetProductByCode(isolatedFabOrder.ProductCode);
            
            if (item != null)
            {
                var uniqueId = Guid.NewGuid().ToString();
                var productionOrder = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
                productionOrder.ProductionOrderType = BoProductionOrderTypeEnum.bopotStandard;
                productionOrder.StartDate = DateTime.Now;
                productionOrder.DueDate = DateTime.Now;
                productionOrder.ItemNo = item.ItemCode;
                productionOrder.ProductDescription = item.ItemName;
                productionOrder.PlannedQuantity = 1;
                productionOrder.DistributionRule = string.Empty;
                productionOrder.DistributionRule2 = string.Empty;
                productionOrder.DistributionRule3 = string.Empty;
                productionOrder.DistributionRule4 = string.Empty;
                productionOrder.DistributionRule5 = string.Empty;
                productionOrder.Project = string.Empty;
                productionOrder.Remarks = uniqueId;

                if (productionOrder.Add() != 0)
                {
                    this.company.GetLastError(out int errorCode, out string errorMessage);
                    _loggerProxy.Debug($"An error has ocurred to create isolated production order { errorCode } - {errorMessage}.");
                    result = new KeyValuePair<string, string>(string.Empty, string.Format(ServiceConstants.FailReasonUnexpectedErrorToCreateIsolatedProductionOrder, isolatedFabOrder.ProductCode));
                }
                else
                {
                    result = new KeyValuePair<string, string>(uniqueId, ServiceConstants.Ok);
                }
            }
            else
            {
                _loggerProxy.Debug($"The product with code { isolatedFabOrder.ProductCode } doesn´t exists.");
                result = new KeyValuePair<string, string>(string.Empty, string.Format(ServiceConstants.FailReasonProductCodeNotExists, isolatedFabOrder.ProductCode));
            }
            return ServiceUtils.CreateResult(true, 200, null, result, null);
        }

        /// <summary>
        /// sets the data to update.
        /// </summary>
        /// <param name="model">the model from controller.</param>
        /// <param name="prodOrder">the data from sap.</param>
        /// <returns>the data to update.</returns>
        private ProductionOrders UpdateEntry(UpdateFabOrderModel model, ProductionOrders prodOrder)
        {
            if (model.Status.Equals(ServiceConstants.StatusLiberado))
            {
                prodOrder.ProductionOrderStatus = BoProductionOrderStatusEnum.boposReleased;
            }

            return prodOrder;
        }

        /// <summary>
        /// makes the dict resutl.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <param name="value">the value.</param>
        /// <param name="status">the status of the action.</param>
        /// <param name="company">the object company.</param>
        /// <param name="dictResult">the dic to return.</param>
        /// <returns>the result.</returns>
        private Dictionary<string, string> AddResult(string key, string value, int status, Company company, Dictionary<string, string> dictResult)
        {
            if(status != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                errMsg = string.IsNullOrEmpty(errMsg) ? string.Empty : errMsg;
                dictResult.Add(key, $"{value}-{errorCode.ToString()}-{errMsg}");
            }
            else
            {
                dictResult.Add(key, "Ok");
            }

            return dictResult;
        }

        /// <summary>
        /// Get product by code
        /// </summary>
        /// <param name="productCode">The product code</param>
        /// <returns></returns>
        private Items GetProductByCode(string productCode)
        {
            var item = (Items)this.company.GetBusinessObject(BoObjectTypes.oItems);
            if (item.GetByKey(productCode))
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// Validate new batches.
        /// </summary>
        /// <param name="itemCode">Item code.</param>
        /// <param name="batches">New batches.</param>
        private void ValidateNewBatches(string itemCode, List<BatchesConfigurationModel> batches)
        {
            if (batches == null)
            {
                return;
            }

            foreach (var batche in batches)
            {
                _loggerProxy.Debug($"Validate new batch { batche.BatchCode } - { itemCode }.");
                var existingBatch = this.ExecuteQuery(ServiceConstants.FindBatchCodeForItem, batche.BatchCode, itemCode);
                if (existingBatch.RecordCount != 0)
                {
                    throw new ValidationException(string.Format(ServiceConstants.FailReasonBatchAlreadyExists, batche.BatchCode, itemCode));
                }
            }
        }

        /// <summary>
        /// Create good issue for production order id.
        /// </summary>
        /// <param name="productionOrderId">The production order id.</param>
        private void CreateoGoodIssueForProductionByOrderId(int productionOrderId)
        {
            _loggerProxy.Debug($"Create oInventoryGenExit for { productionOrderId }.");
            var recordSet = this.ExecuteQuery(ServiceConstants.FindManualExit, productionOrderId);
            var inventoryGenExit = (Documents)this.company.GetBusinessObject(BoObjectTypes.oInventoryGenExit);

            for (var i = 0; i < recordSet.RecordCount; i++)
            {
                inventoryGenExit.Lines.SetCurrentLine(i);
                inventoryGenExit.Lines.BaseType = (int)BoObjectTypes.oProductionOrders;
                inventoryGenExit.Lines.BaseEntry = productionOrderId;
                inventoryGenExit.Lines.BaseLine = int.Parse(recordSet.Fields.Item("LineNum").Value.ToString());
                inventoryGenExit.Lines.Quantity = double.Parse(recordSet.Fields.Item("PlannedQty").Value.ToString());
                inventoryGenExit.Lines.WarehouseCode = recordSet.Fields.Item("warehouse").Value.ToString();
                this.SetBatchNumbersToGoodIssueForProduction(ref inventoryGenExit, recordSet.Fields.Item("ItemCode").Value.ToString());
                inventoryGenExit.Lines.Add();
                recordSet.MoveNext();
            }

            if (recordSet.RecordCount > 0 && inventoryGenExit.Add() != 0)
            {
                this.company.GetLastError(out int errorCode, out string errorMessage);
                _loggerProxy.Debug($"An error has ocurred on create oInventoryGenExit { errorCode } - { errorMessage }.");
                throw new ValidationException(string.Format(ServiceConstants.FailReasonNotGetExitCreated, productionOrderId));
            }
        }

        /// <summary>
        /// Create receipt from production order id.
        /// </summary>
        /// <param name="productionOrderId">The production order id.</param>
        /// <param name="closeConfiguration">Configuration for close order.</param>
        private void CreateReceiptFromProductionOrderId(int productionOrderId, CloseProductionOrderModel closeConfiguration) {
            _loggerProxy.Debug($"Create oInventoryGenEntry for { productionOrderId }.");
            var separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var productionOrder = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
            var receiptProduction = this.company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);
            closeConfiguration.Batches = closeConfiguration.Batches ?? new List<BatchesConfigurationModel>();

            productionOrder.GetByKey(productionOrderId);
            var product = this.GetProductByCode(productionOrder.ItemNo);
            var quantityToReceipt = productionOrder.PlannedQuantity;

            if (product.ManageBatchNumbers == BoYesNoEnum.tYES)
            {
                _loggerProxy.Debug($"Log batches quantity with decimal separator { separator }.");
                closeConfiguration.Batches.ForEach(x => _loggerProxy.Debug($"Batch { x.BatchCode } with quantity { x.Quantity }."));
                closeConfiguration.Batches.ForEach(x => _loggerProxy.Debug($"Batch { x.BatchCode } with quantity { Double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator)) }."));
                _loggerProxy.Debug($"Sum { closeConfiguration.Batches.Sum(x => Double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator))) }.");

                quantityToReceipt = closeConfiguration.Batches.Sum(x => Double.Parse(System.Text.RegularExpressions.Regex.Replace(x.Quantity, "[.,]", separator)));
                var counter = 0;
                foreach (var batchConfig in closeConfiguration.Batches)
                {
                    receiptProduction.Lines.BatchNumbers.SetCurrentLine(counter);
                    receiptProduction.Lines.BatchNumbers.BatchNumber = batchConfig.BatchCode;
                    receiptProduction.Lines.BatchNumbers.ManufacturingDate = batchConfig.ManufacturingDate;
                    receiptProduction.Lines.BatchNumbers.ExpiryDate = batchConfig.ExpirationDate;
                    receiptProduction.Lines.BatchNumbers.Quantity = Double.Parse(System.Text.RegularExpressions.Regex.Replace(batchConfig.Quantity, "[.,]", separator));
                    receiptProduction.Lines.BatchNumbers.Add();
                    counter += 1;
                }
            }

            receiptProduction.Lines.BaseEntry = productionOrder.DocumentNumber;
            receiptProduction.Lines.BaseType = 202;
            receiptProduction.Lines.Quantity = quantityToReceipt;
            receiptProduction.Lines.TransactionType = BoTransactionTypeEnum.botrntComplete;
            receiptProduction.Lines.WarehouseCode = productionOrder.Warehouse;
            receiptProduction.Lines.Add();

            if (receiptProduction.Add() != 0)
            {
                this.company.GetLastError(out int errorCode, out string errorMessage);
                _loggerProxy.Debug($"An error has ocurred on save receipt production { errorCode } - {errorMessage}.");
                throw new ValidationException(string.Format(ServiceConstants.FailReasonNotReceipProductionCreated, productionOrderId));
            }
        }

        /// <summary>
        /// Close production order
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        private void CloseProductionOrder(int productionOrderId)
        {
            _loggerProxy.Debug($"Close production order { productionOrderId }.");
            var productionOrder = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
            productionOrder.GetByKey(productionOrderId);
            productionOrder.ProductionOrderStatus = BoProductionOrderStatusEnum.boposClosed;

            if (productionOrder.Update() != 0)
            {
                this.company.GetLastError(out int errorCode, out string errorMessage);
                _loggerProxy.Debug($"An error has ocurred on update production order status { errorCode } - {errorMessage}.");
                throw new ValidationException(string.Format(ServiceConstants.FailReasonNotProductionStatusClosed, productionOrder.DocumentNumber));
            }
        }

        /// <summary>
        /// Set batch numbers for good issue line.
        /// </summary>
        /// <param name="goodIssue">Good issue.</param>
        /// <param name="itemCode">Item code.</param>
        private void SetBatchNumbersToGoodIssueForProduction(ref Documents goodIssue, string itemCode)
        {
            Items product = this.GetProductByCode(itemCode);
            if (product.ManageBatchNumbers != BoYesNoEnum.tYES)
            {
                return;
            }

            var assignedBatches = this.GetAssignmentBatches(goodIssue.Lines.BaseEntry, itemCode, goodIssue.Lines.WarehouseCode);
            var batchCounter = 0;
            foreach (var batch in assignedBatches)
            {
                goodIssue.Lines.BatchNumbers.SetCurrentLine(batchCounter);
                goodIssue.Lines.BatchNumbers.BatchNumber = batch.Key;
                goodIssue.Lines.BatchNumbers.Quantity = batch.Value;
                goodIssue.Lines.BatchNumbers.Add();
                batchCounter += 1;
            }
        }

        /// <summary>
        /// Get last invetory log entry for item code in a document.
        /// </summary>
        /// <param name="itemCode">Item code.</param>
        /// <param name="docNumber">Doc number.</param>
        /// <returns>Last log entry.</returns>
        private long GetLastInventoryLogEntry(string itemCode, int docNumber)
        {
            var results = this.ExecuteQuery(ServiceConstants.FindLastInventoryLogEntry, itemCode, docNumber);
            return long.Parse(results.Fields.Item("LogEntry").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get batch code by item code, warehouse and sysNumber.
        /// </summary>
        /// <param name="itemCode">Item code.</param>
        /// <param name="wharehouse">Wharehouse code.</param>
        /// <param name="sysNumber">SysNumber.</param>
        /// <returns>Batch code.</returns>
        private string GetBatchCode(string itemCode, string wharehouse, int sysNumber)
        {
            var results = this.ExecuteQuery(ServiceConstants.FindBatchCode, itemCode, wharehouse, sysNumber);
            return results.Fields.Item("DistNumber").Value.ToString();
        }

        /// <summary>
        /// Validate required quantities in an production order.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        private void ValidateRequiredQuantityForRetroactiveIssues(int productionOrderId)
        {
            _loggerProxy.Debug($"Validate required quantities for { productionOrderId }.");

            var recordSet = this.ExecuteQuery(ServiceConstants.GetRetroactiveIssuesInProductionOrder, productionOrderId);
            var missingComponents = new List<string>();

            for (var i = 0; i < recordSet.RecordCount; i++)
            {
                var requiredQuantity = double.Parse(recordSet.Fields.Item("PlannedQty").Value.ToString());
                var warehouse = recordSet.Fields.Item("warehouse").Value.ToString();
                var itemCode = recordSet.Fields.Item("ItemCode").Value.ToString();

                if (!this.IsAvailableRequiredQuantity(itemCode, warehouse, requiredQuantity))
                {
                    missingComponents.Add(itemCode);
                }

                recordSet.MoveNext();
            }

            if (!missingComponents.Any()) return;

            var formated = string.Join(", ", missingComponents);
            throw new ValidationException(string.Format(ServiceConstants.FailReasonNotAvailableRequiredQuantity, productionOrderId, formated));
        }

        /// <summary>
        /// Is available product quantity in warehouse.
        /// </summary>
        /// <param name="itemCode">Item code.</param>
        /// <param name="wharehouse">Wharehouse code.</param>
        /// <param name="requiredQuantity">Required quantity.</param>
        /// <returns>Flag result.</returns>
        private bool IsAvailableRequiredQuantity(string itemCode, string wharehouse, double requiredQuantity)
        {
            var results = this.ExecuteQuery(ServiceConstants.QueryAvailableQuantityByWarehouse, itemCode, wharehouse);
            var availableQuantityAsString = results.Fields.Item("Available").Value.ToString();
            availableQuantityAsString = string.IsNullOrEmpty(availableQuantityAsString) ? "0" : availableQuantityAsString;
            var availableQuantity = double.Parse(availableQuantityAsString);
            return requiredQuantity < availableQuantity;
        }

        /// <summary>
        /// Get assignment batches to item in a document.
        /// </summary>
        /// <param name="docNumber">Document number.</param>
        /// <param name="itemCode">Item code.</param>
        /// <param name="warehouse">Warehouse code.</param>
        /// <returns>Assignment batches.</returns>
        private Dictionary<string, double> GetAssignmentBatches(int docNumber, string itemCode, string warehouse)
        {
            var results = new Dictionary<string, double>();
            var lastTransaction = this.GetLastInventoryLogEntry(itemCode, docNumber);
            var assignments = this.ExecuteQuery(ServiceConstants.FindAssignedBatchesByLogEntry, lastTransaction);

            for (var i = 0; i < assignments.RecordCount; i++)
            {
                var sysNumber = int.Parse(assignments.Fields.Item("SysNumber").Value.ToString());
                var quantity = double.Parse(assignments.Fields.Item("AllocQty").Value.ToString());
                var batchCode = this.GetBatchCode(itemCode, warehouse, sysNumber);
                results.Add(batchCode, quantity);
                assignments.MoveNext();
            }

            return results;
        }
        
        /// <summary>
        /// Execute query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Recordset.</returns>
        private Recordset ExecuteQuery(string query, params object[] parameters)
        {
            query = string.Format(query, parameters);
            var recordSet = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            recordSet.DoQuery(query);
            return recordSet;
        }
    }
}
