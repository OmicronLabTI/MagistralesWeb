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
                        dictResult.Add(string.Format("{0}", order.OrderFabId), string.Format("{0}-{1}-{2}", ServiceConstants.ErrorUpdateFabOrd, errorCode.ToString(), errMsg));
                    }
                    else
                    {
                        dictResult.Add(string.Format("{0}", order.OrderFabId), "Ok");
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
    }
}
