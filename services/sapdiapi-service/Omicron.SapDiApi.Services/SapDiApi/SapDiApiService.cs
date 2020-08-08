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
                        dictResult.Add(string.Format("{0}-{1}", pedido.Order.PedidoId, orf.CodigoProducto), string.Format("ErrorCreateFabOrd-{0}-{1}", errorCode.ToString(), errMsg));
                    }
                    else
                    {
                        dictResult.Add(string.Format("{0}-{1}", pedido.Order.PedidoId, orf.CodigoProducto), "Ok");
                    }
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictResult), null);            
        }
    }
}
