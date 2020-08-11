// <summary>
// <copyright file="AsignarLogic.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;

    /// <summary>
    /// Logic To assign orders.
    /// </summary>
    public static class AsignarLogic
    {
        /// <summary>
        /// makes the logic to assign a pedido.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <param name="pedidosDao">the pedidos dao.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="sapDiApi">The sap di api.</param>
        /// <returns>the result.</returns>
        public static async Task<ResultModel> AssignPedido(ManualAssignModel assignModel, IPedidosDao pedidosDao, ISapAdapter sapAdapter, ISapDiApi sapDiApi)
        {
            var orders = new List<CompleteDetailOrderModel>();
            var listToUpdate = new List<UpdateFabOrderModel>();
            var listSalesOrders = new List<string>();

            foreach (var de in assignModel.DocEntry)
            {
                listSalesOrders.Add(de.ToString());
                var sapResponse = await sapAdapter.GetSapAdapter(string.Format(ServiceConstants.GetFabOrdersByPedidoId, de));
                orders.AddRange(JsonConvert.DeserializeObject<List<CompleteDetailOrderModel>>(sapResponse.Response.ToString()));
            }

            orders.ForEach(o =>
            {
                listToUpdate.Add(new UpdateFabOrderModel
                {
                    OrderFabId = o.OrdenFabricacionId,
                    Status = ServiceConstants.StatusSapLiberado,
                });
            });

            orders.Where(x => x.Status.Equals(ServiceConstants.Planificado)).ToList().ForEach(o =>
            {
                listToUpdate.Add(new UpdateFabOrderModel
                {
                    OrderFabId = o.OrdenFabricacionId,
                    Status = ServiceConstants.StatusSapLiberado,
                });
            });

            var resultSap = await sapDiApi.PostToSapDiApi(listToUpdate, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFavOrd);
            var listErrorId = ServiceUtils.GetErrorsWhileInserting(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var userOrders = (await pedidosDao.GetUserOrderBySaleOrder(listSalesOrders)).ToList();

            userOrders.ForEach(x =>
            {
                x.Status = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.Liberado : ServiceConstants.Asignado;
                x.Userid = assignModel.UserId;
            });

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, assignModel.DocEntry, string.Format(ServiceConstants.AsignarVenta, assignModel.UserId), ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, listToUpdate.Select(x => x.OrderFabId).ToList(), string.Format(ServiceConstants.AsignarOrden, assignModel.UserId), ServiceConstants.OrdenFab));

            await pedidosDao.UpdateUserOrders(userOrders);
            await pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }
    }
}
