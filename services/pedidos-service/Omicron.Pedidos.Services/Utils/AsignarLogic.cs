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
            var listSalesOrders = assignModel.DocEntry.Select(x => x.ToString()).ToList();

            listToUpdate = await ServiceUtils.GetOrdersToAssign(assignModel.DocEntry, sapAdapter);

            var resultSap = await sapDiApi.PostToSapDiApi(listToUpdate, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
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

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var userOrdersByProd = (await pedidosDao.GetUserOrderByProducionOrder(listProdOrders)).ToList();
            var listSales = userOrdersByProd.Select(x => x.Salesorderid).Distinct().ToList();
            var userOrderBySales = (await pedidosDao.GetUserOrderBySaleOrder(listSales)).ToList();

            userOrdersByProd = GetUpdateUserOrderModel(userOrdersByProd, userOrderBySales, assignModel.UserId, ServiceConstants.Asignado);

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, assignModel.DocEntry, string.Format(ServiceConstants.AsignarVenta, assignModel.UserId), ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, listToUpdate.Select(x => x.OrderFabId).ToList(), string.Format(ServiceConstants.AsignarOrden, assignModel.UserId), ServiceConstants.OrdenFab));

            await pedidosDao.UpdateUserOrders(userOrdersByProd);
            await pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// Gets the valid users by total count of piezas.
        /// </summary>
        /// <param name="users">the list of users.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="maxCountPedidos">the max count of piezas.</param>
        /// <returns>the users.</returns>
        public static async Task<List<AutomaticAssignUserModel>> GetValidUsersByLoad(List<UserModel> users, List<UserOrderModel> userOrders, ISapAdapter sapAdapter, int maxCountPedidos)
        {
            var validUsers = new List<AutomaticAssignUserModel>();

            foreach (var user in users)
            {
                var pedidosId = userOrders.Where(x => x.Userid.Equals(user.Id)).Select(y => int.Parse(y.Salesorderid)).Distinct().ToList();
                var orders = await sapAdapter.PostSapAdapter(pedidosId, ServiceConstants.GetOrderWithDetail);
                var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));

                var total = GetCountOfPlannedQtyByOrder(ordersSap);

                if (total < maxCountPedidos)
                {
                    var listIds = new List<string>();
                    var listProdIds = new List<int>();
                    ordersSap.ForEach(x =>
                    {
                        listIds.AddRange(x.Detalle.Select(y => y.CodigoProducto).ToList());
                        listProdIds.AddRange(x.Detalle.Select(y => y.OrdenFabricacionId).ToList());
                    });

                    validUsers.Add(new AutomaticAssignUserModel
                    {
                        User = user,
                        TotalCount = total,
                        ItemCodes = listIds,
                        ProductionOrders = listProdIds,
                    });
                }
            }

            return validUsers.OrderBy(x => x.TotalCount).ThenBy(y => y.User.FirstName).ToList();
        }

        /// <summary>
        /// Gets the user available by formula.
        /// </summary>
        /// <param name="users">the ussers with item codes.</param>
        /// <param name="orderDetail">the order with details.</param>
        /// <param name="userOrders">The user orders.</param>
        /// <returns>the data to return.</returns>
        public static Dictionary<int, string> GetValidUsersByFormula(List<AutomaticAssignUserModel> users, List<OrderWithDetailModel> orderDetail, List<UserOrderModel> userOrders)
        {
            var dictUserPedido = new Dictionary<int, string>();

            foreach (var p in orderDetail)
            {
                users = users.OrderBy(x => x.TotalCount).ThenBy(x => x.User.FirstName).ToList();

                if (!p.Detalle.Any(d => d.CodigoProducto.Contains("   ")))
                {
                    dictUserPedido.Add(p.Order.DocNum, users.FirstOrDefault().User.Id);
                    continue;
                }

                dictUserPedido = GetEntryUserValue(dictUserPedido, p.Detalle, users, p.Order.DocNum, users.FirstOrDefault().User.Id, userOrders);

                users.ForEach(x =>
                {
                    x.TotalCount = x.User.Id.Equals(dictUserPedido[p.Order.DocNum]) ? p.Detalle.Where(z => z.QtyPlanned.HasValue).Sum(y => y.QtyPlanned.Value) + x.TotalCount : x.TotalCount;
                });
            }

            return dictUserPedido;
        }

        /// <summary>
        /// Place the status for the orders.
        /// </summary>
        /// <param name="listFromOrders">the list sent from front.</param>
        /// <param name="listFromSales">list from DB.</param>
        /// <param name="user">the user to update.</param>
        /// <param name="statusOrder">Status for the order fab.</param>
        /// <returns>the data.</returns>
        public static List<UserOrderModel> GetUpdateUserOrderModel(List<UserOrderModel> listFromOrders, List<UserOrderModel> listFromSales, string user, string statusOrder)
        {
            var listToUpdate = new List<UserOrderModel>();

            listFromSales
                .GroupBy(x => x.Salesorderid)
                .ToList()
                .ForEach(y =>
                {
                    var currentOrdersBySale = listFromOrders.Where(z => z.Salesorderid == y.Key).ToList();
                    var currentOrders = currentOrdersBySale.Select(x => x.Productionorderid).ToList();
                    var missing = y.Any(z => z.Status == ServiceConstants.Planificado && !string.IsNullOrEmpty(z.Productionorderid) && !currentOrders.Contains(z.Productionorderid));

                    currentOrdersBySale.ForEach(o =>
                    {
                        o.Userid = user;
                        o.Status = statusOrder;
                        listToUpdate.Add(o);

                        if (!string.IsNullOrEmpty(o.Salesorderid))
                        {
                            var pedido = listFromSales.FirstOrDefault(x => x.Salesorderid == o.Salesorderid && string.IsNullOrEmpty(x.Productionorderid));
                            pedido.Status = missing ? ServiceConstants.Planificado : ServiceConstants.Liberado;
                            pedido.Userid = user;
                            listToUpdate.Add(o);
                        }
                    });
                });

            return listToUpdate;
        }

        /// <summary>
        /// populates the dict for the user pedido.
        /// </summary>
        /// <param name="pedidoUser">the dictionary.</param>
        /// <param name="detailModel">the detail.</param>
        /// <param name="availableUsers">the available users by formula.</param>
        /// <param name="pedidoId">the pedido id.</param>
        /// <param name="defaultUser">the default user if nothing matches.</param>
        /// <param name="userOrders">The user orders already assigned.</param>
        /// <returns>the dict.</returns>
        private static Dictionary<int, string> GetEntryUserValue(Dictionary<int, string> pedidoUser, List<CompleteDetailOrderModel> detailModel, List<AutomaticAssignUserModel> availableUsers, int pedidoId, string defaultUser, List<UserOrderModel> userOrders)
        {
            if (pedidoUser.ContainsKey(pedidoId))
            {
                return pedidoUser;
            }

            var listOrdersAignado = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Status.Equals(ServiceConstants.Asignado)).Select(y => int.Parse(y.Productionorderid)).ToList();
            var usersFormulaAvailable = availableUsers.Where(x => x.ProductionOrders.Any(y => listOrdersAignado.Contains(y))).ToList();

            var users = new List<AutomaticAssignUserModel>();
            detailModel
                .Where(x => x.CodigoProducto.Contains("   "))
                .Select(y => y.CodigoProducto.Split("   ")[0])
                .ToList()
                .ForEach(d =>
                {
                    users.AddRange(usersFormulaAvailable.Where(z => z.ItemCodes.Any(a => a.Contains(d))).ToList());
                });

            var user = users.OrderBy(x => x.TotalCount).ThenBy(x => x.User.FirstName).ToList().FirstOrDefault();

            if (user != null)
            {
                pedidoUser.Add(pedidoId, user.User.Id);
                return pedidoUser;
            }

            pedidoUser.Add(pedidoId, defaultUser);
            return pedidoUser;
        }

        /// <summary>
        /// Gets the total of pieces by all the pedidos.
        /// </summary>
        /// <param name="listPedidos">the list of pedidos.</param>
        /// <returns>the total.</returns>
        private static int GetCountOfPlannedQtyByOrder(List<OrderWithDetailModel> listPedidos)
        {
            var total = 0;
            listPedidos.ForEach(x =>
            {
                total += x.Detalle.Where(z => z.QtyPlanned.HasValue).Sum(y => y.QtyPlanned.Value);
            });

            return total;
        }
    }
}
