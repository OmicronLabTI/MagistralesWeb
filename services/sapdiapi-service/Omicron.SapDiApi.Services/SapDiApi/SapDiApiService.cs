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
                        dictResult.Add(string.Format("{0}-{1}", pedido.Order.PedidoId, orf.CodigoProducto), string.Format("{0}-{1}-{2}", ServiceConstants.ErrorCreateFabOrd, errorCode.ToString(), errMsg));
                        _loggerProxy.Info($"The next order was tried to be created: {errorCode} - {errMsg} - {pedido.Order.PedidoId}");
                    }
                    else
                    {
                        dictResult.Add(string.Format("{0}-{1}", pedido.Order.PedidoId, orf.CodigoProducto), "Ok");
                    }
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

            double.TryParse(updateFormula.PlannedQuantity.ToString(), out double plannedQuantity);
            productionOrderObj.DueDate = updateFormula.FechaFin;
            productionOrderObj.PlannedQuantity = plannedQuantity;

            var components = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            components.DoQuery(string.Format(ServiceConstants.FindWor1ByDocEntry, updateFormula.FabOrderId.ToString()));
            
            var listIdsUpdated = new List<string>();
            var listToDelete = new List<int>();

            if (components.RecordCount != 0)
            {
                for (var i = 0; i < components.RecordCount; i++)
                {
                    var sapItemCode = components.Fields.Item("ItemCode").Value;
                    var lineNum = components.Fields.Item("VisOrder").Value;
                    var itemCode = components.Fields.Item("ItemCode").Value;
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

                    if (component.Action.Equals(ServiceConstants.DeleteComponent))
                    {
                        listToDelete.Add(lineNum);
                        listIdsUpdated.Add(sapItemCode);
                        components.MoveNext();
                        continue;
                    }

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
                double.TryParse(line.BaseQuantity.ToString(), out double baseQuantity);
                double.TryParse(line.RequiredQuantity.ToString(), out double issuedQuantity);

                productionOrderObj.Lines.Add();
                productionOrderObj.Lines.ItemNo = line.ProductId;
                productionOrderObj.Lines.Warehouse = line.Warehouse;
                productionOrderObj.Lines.BaseQuantity = baseQuantity;
                productionOrderObj.Lines.PlannedQuantity = issuedQuantity;
            }

            foreach (var lineDel in listToDelete.OrderByDescending(x => x))
            {
                productionOrderObj.Lines.SetCurrentLine(lineDel);
                productionOrderObj.Lines.Delete();
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

                    dictResult = this.AddResult($"{group.Key}-{itemCode}", ServiceConstants.ErrorUpdateFabOrd, lastError, company, dictResult);

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
        public async Task<ResultModel> FinishOrder(List<CancelOrderModel> productionOrders)
        {
            var results = new Dictionary<int, string>();

            foreach (var productionOrder in productionOrders)
            {
                var productionOrderId = productionOrder.OrderId;

                _loggerProxy.Debug($"Production order to finish: { productionOrderId }.");
                var orderReference = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);

                if (!orderReference.GetByKey( productionOrder.OrderId ))
                {
                    _loggerProxy.Debug($"The production order { productionOrderId } doesn´t exists.");
                    results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotExistsProductionOrder, productionOrderId));
                }

                if (orderReference.ProductionOrderStatus == BoProductionOrderStatusEnum.boposClosed)
                {
                    _loggerProxy.Debug($"The production order { productionOrderId } already closed.");
                    continue;
                }

                if (orderReference.ProductionOrderStatus != BoProductionOrderStatusEnum.boposReleased)
                {
                    _loggerProxy.Debug($"The production order { productionOrderId } isn't released.");
                    results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotReleasedProductionOrder, productionOrderId));
                }

                // Production orders
                _loggerProxy.Debug($"Data production order { productionOrderId }.");
                _loggerProxy.Debug($"DocumentNumber: { orderReference.DocumentNumber }.");
                _loggerProxy.Debug($"PlannedQuantity: { orderReference.PlannedQuantity }.");
                _loggerProxy.Debug($"Warehouse: { orderReference.Warehouse }.");

                // Create a new receipt production order
                var receiptProduction = this.company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);
                receiptProduction.Lines.BaseEntry = orderReference.DocumentNumber;
                receiptProduction.Lines.BaseType = 202;
                receiptProduction.Lines.Quantity = orderReference.PlannedQuantity;
                receiptProduction.Lines.TransactionType = BoTransactionTypeEnum.botrntComplete;
                receiptProduction.Lines.WarehouseCode = orderReference.Warehouse;
                receiptProduction.Lines.Add();

                // Save receipt production
                if (receiptProduction.Add() > 0)
                {
                    this.company.GetLastError(out int errorCode, out string errorMessage);
                    _loggerProxy.Debug($"An error has ocurred on save receipt production { errorCode } - {errorMessage}.");
                    results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotReceipProductionCreated, productionOrderId));
                    continue;
                }

                // Set closed status 
                orderReference.ProductionOrderStatus = BoProductionOrderStatusEnum.boposClosed;

                if (orderReference.Update() > 0)
                {
                    this.company.GetLastError(out int errorCode, out string errorMessage);
                    _loggerProxy.Debug($"An error has ocurred on update production order status { errorCode } - {errorMessage}.");
                    results.Add(productionOrderId, string.Format(ServiceConstants.FailReasonNotProductionStatusClosed, productionOrderId ));
                    continue;
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

            var item =  (ProductTrees)this.company.GetBusinessObject(BoObjectTypes.oProductTrees);
            if (item.GetByKey(isolatedFabOrder.ProductCode))
            {
                var uniqueId = Guid.NewGuid().ToString();
                var productionOrder = (ProductionOrders)company.GetBusinessObject(BoObjectTypes.oProductionOrders);
                productionOrder.ProductionOrderType = BoProductionOrderTypeEnum.bopotStandard;
                productionOrder.StartDate = DateTime.Now;
                productionOrder.DueDate = DateTime.Now;
                productionOrder.ItemNo = item.TreeCode;
                productionOrder.ProductDescription = item.ProductDescription;
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
    }
}
