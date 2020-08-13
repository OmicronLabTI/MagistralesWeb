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

    /// <summary>
    /// clas for the data to sap.
    /// </summary>
    public class SapDiApiService : ISapDiApiService
    {
        private readonly Company company;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDiApiService"/> class.
        /// </summary>   
        public SapDiApiService()
        {
            this.company = Connection.Company;
        }

        /// <summary>
        /// Connects to SAP.
        /// </summary>
        /// <returns>the connection.</returns>
        public async Task<ResultModel> Connect()
        {
            var connected = this.company.Connected;
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

            if (orderFab)
            {
                double.TryParse(updateFormula.PlannedQuantity.ToString(), out double plannedQuantity);
                productionOrderObj.DueDate = updateFormula.FechaFin;
                productionOrderObj.PlannedQuantity = plannedQuantity;

                var components = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                components.DoQuery(string.Format(ServiceConstants.FindWor1ByDocEntry, updateFormula.FabOrderId.ToString()));
                var listIdsUpdated = new List<string>();

                if (components.RecordCount != 0)
                {
                    for (var i = 0; i < components.RecordCount; i++)
                    {
                        var sapItemCode = components.Fields.Item("ItemCode").Value;
                        var lineNum = components.Fields.Item("LineNum").Value;
                        productionOrderObj.Lines.SetCurrentLine(lineNum);

                        var component = updateFormula.Components.FirstOrDefault(x => x.ProductId.Equals(sapItemCode));
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
                foreach(var line in listNotInserted)
                {
                    double.TryParse(line.BaseQuantity.ToString(), out double baseQuantity);
                    double.TryParse(line.RequiredQuantity.ToString(), out double issuedQuantity);

                    productionOrderObj.Lines.Add();
                    productionOrderObj.Lines.ItemNo = line.ProductId;
                    productionOrderObj.Lines.Warehouse = line.Warehouse;
                    productionOrderObj.Lines.BaseQuantity = baseQuantity;
                    productionOrderObj.Lines.PlannedQuantity = issuedQuantity;
                }

                var updated = productionOrderObj.Update();
                dictResult = this.AddResult($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", ServiceConstants.ErrorUpdateFabOrd, updated, company, dictResult);
            }
            else
            {
                dictResult = this.AddResult($"{updateFormula.FabOrderId}-{updateFormula.FabOrderId}", $"{ServiceConstants.ErrorUpdateFabOrd}-{ServiceConstants.OrderNotFound}", -1, company, dictResult);                
            }

            return ServiceUtils.CreateResult(true, 200, null, dictResult, null);
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

        private Dictionary<string, string> AddResult(string key, string value, int status, Company company, Dictionary<string, string> dictResult)
        {
            if(status != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                errMsg = string.IsNullOrEmpty(errMsg) ? string.Empty : errMsg;
                dictResult.Add(key, string.Format(value, $"{value}-{errorCode.ToString()}-{errMsg}", errMsg));
            }
            else
            {
                dictResult.Add(key, "Ok");
            }

            return dictResult;
        }
    }
}
