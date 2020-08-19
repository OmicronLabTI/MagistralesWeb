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
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
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

        /// <summary>
        /// the logic to assign a order.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <param name="pedidosDao">the pedido dao.</param>
        /// <param name="sapDiApi">the di api.</param>
        /// <returns>the data.</returns>
        public static async Task<ResultModel> AssignOrder(ManualAssignModel assignModel, IPedidosDao pedidosDao, ISapDiApi sapDiApi)
        {
            var listToUpdate = new List<UpdateFabOrderModel>();
            var listProdOrders = new List<string>();

            assignModel.DocEntry.ForEach(x =>
            {
                listToUpdate.Add(new UpdateFabOrderModel
                {
                    OrderFabId = x,
                    Status = ServiceConstants.StatusSapLiberado,
                });

                listProdOrders.Add(x.ToString());
            });

            var resultSap = await sapDiApi.PostToSapDiApi(listToUpdate, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFavOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var userOrdersByProd = (await pedidosDao.GetUserOrderByProducionOrder(listProdOrders)).ToList();
            var listSales = userOrdersByProd.Select(x => x.Salesorderid).Distinct().ToList();
            var userOrderBySales = (await pedidosDao.GetUserOrderBySaleOrder(listSales)).ToList();

            userOrdersByProd = GetUpdateUserOrderModel(userOrdersByProd, userOrderBySales, assignModel.UserId);

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, assignModel.DocEntry, string.Format(ServiceConstants.AsignarVenta, assignModel.UserId), ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, listToUpdate.Select(x => x.OrderFabId).ToList(), string.Format(ServiceConstants.AsignarOrden, assignModel.UserId), ServiceConstants.OrdenFab));

            await pedidosDao.UpdateUserOrders(userOrdersByProd);
            await pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// Place the status for the orders.
        /// </summary>
        /// <param name="listFromOrders">the list sent from front.</param>
        /// <param name="listFromSales">list from DB.</param>
        /// <param name="user">the user to update.</param>
        /// <returns>the data.</returns>
        private static List<UserOrderModel> GetUpdateUserOrderModel(List<UserOrderModel> listFromOrders, List<UserOrderModel> listFromSales, string user)
        {
            var currentOrders = listFromOrders.Select(x => x.Productionorderid).ToList();
            var missing = listFromSales.Any(y => y.Status == ServiceConstants.Planificado && !string.IsNullOrEmpty(y.Productionorderid) && !currentOrders.Contains(y.Productionorderid));

            var listPedidos = new List<UserOrderModel>();

            listFromOrders.ForEach(o =>
            {
                o.Userid = user;
                o.Status = ServiceConstants.Asignado;

                var pedido = listFromSales.FirstOrDefault(x => x.Salesorderid == o.Salesorderid && string.IsNullOrEmpty(x.Productionorderid));
                pedido.Status = missing ? ServiceConstants.Planificado : ServiceConstants.Liberado;
                pedido.Userid = user;
                listPedidos.Add(pedido);
            });

            listFromOrders.AddRange(listPedidos);

            return listFromOrders;
        }
    }
}
