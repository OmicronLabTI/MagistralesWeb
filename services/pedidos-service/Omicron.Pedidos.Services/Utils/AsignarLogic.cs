// <summary>
// <copyright file="AsignarLogic.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// Logic To assign orders.
    /// </summary>
    public static class AsignarLogic
    {
        /// <summary>
        /// Function to filter Dz not omigenoimcs.
        /// </summary>
        /// <param name="detail">detail.</param>
        /// <returns>Boolean.</returns>
        public static bool IsDzAndIsNotOmigenomics(CompleteDetailOrderModel detail)
            => ServiceShared.CalculateAnd(
                detail.CodigoProducto.ToUpper().StartsWith("DZ "),
                ServiceConstants.SignaturesToAssignProductsDZ.Contains(detail.ProductFirmName.ToLower()),
                !detail.IsOmigenomics);

        /// <summary>
        /// makes the logic to assign a pedido.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <param name="qfbInfoValidated"> QfbINfoValidated.</param>
        /// <param name="pedidosDao">the pedidos dao.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="serviceLayerAdapterService">The service layer service.</param>
        /// <param name="kafkaConnector">The kafka conector.</param>
        /// <returns>the result.</returns>
        public static async Task<ResultModel> AssignPedido(ManualAssignModel assignModel, QfbTecnicInfoDto qfbInfoValidated, IPedidosDao pedidosDao, ISapAdapter sapAdapter, ISapServiceLayerAdapterService serviceLayerAdapterService, IKafkaConnector kafkaConnector)
        {
            var listSalesOrders = assignModel.DocEntry.Select(x => x.ToString()).ToList();
            var userOrders = (await pedidosDao.GetUserOrderBySaleOrder(listSalesOrders)).ToList();

            var listToUpdate = userOrders
                .Where(x => ServiceShared.CalculateAnd(x.IsProductionOrder, x.Status == ServiceConstants.Planificado))
                .Select(y => new UpdateFabOrderModel { Status = ServiceConstants.StatusSapLiberado, OrderFabId = int.Parse(y.Productionorderid) })
                .ToList();

            var resultSap = await serviceLayerAdapterService.PostAsync(listToUpdate, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = ServiceShared.CalculateTernary(listErrorId.Any(), ServiceConstants.ErroAlAsignar, null);
            var listOrderLogToInsert = new List<SalesLogs>();
            userOrders.ForEach(x =>
            {
                x.Status = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.Liberado : ServiceConstants.Asignado;
                x.Userid = assignModel.UserId;
                x.TecnicId = qfbInfoValidated.TecnicId;
                x.StatusForTecnic = x.Status;
                x.AssignmentDate = DateTime.Now;
                /** add logs**/
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(assignModel.UserLogistic, new List<UserOrderModel> { x }));
            });

            await pedidosDao.UpdateUserOrders(userOrders);
            _ = kafkaConnector.PushMessage(listOrderLogToInsert);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// the logic to assign a order.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <param name="qfbInfoValidated">Qfb Info Validated.</param>
        /// <param name="pedidosDao">the pedido dao.</param>
        /// <param name="serviceLayerAdapterService">the serviceLayerAdapterService.</param>
        /// <param name="sapAdapter">Sap adapter.</param>
        /// <param name="kafkaConnector">The kafka conector.</param>
        /// <returns>the data.</returns>
        public static async Task<ResultModel> AssignOrder(ManualAssignModel assignModel, QfbTecnicInfoDto qfbInfoValidated, IPedidosDao pedidosDao, ISapServiceLayerAdapterService serviceLayerAdapterService, ISapAdapter sapAdapter, IKafkaConnector kafkaConnector)
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
            Dictionary<string, string> dictResult = await UpdateSapOrders(serviceLayerAdapterService, listToUpdate);

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;
            var userOrdersByProd = (await pedidosDao.GetUserOrderByProducionOrder(listProdOrders)).ToList();
            var listSales = userOrdersByProd.Select(x => x.Salesorderid).Distinct().ToList();
            var userOrderBySales = (await pedidosDao.GetUserOrderBySaleOrder(listSales)).ToList();

            var listSalesNumber = listSales.Where(y => !string.IsNullOrEmpty(y)).Select(x => int.Parse(x)).ToList();
            var sapOrders = listSalesNumber.Any() ? await ServiceUtils.GetOrdersWithFabOrders(sapAdapter, listSalesNumber) : new List<OrderWithDetailModel>();

            var getUpdateUserOrderModel = GetUpdateUserOrderModel(userOrdersByProd, userOrderBySales, sapOrders, assignModel.UserId, ServiceConstants.Asignado, assignModel.UserLogistic, true, qfbInfoValidated);
            userOrdersByProd = getUpdateUserOrderModel.Item1;
            var listOrderLogToInsert = getUpdateUserOrderModel.Item2;

            await pedidosDao.UpdateUserOrders(userOrdersByProd);
            _ = kafkaConnector.PushMessage(listOrderLogToInsert);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// Gets the valid users by total count of piezas.
        /// </summary>
        /// <param name="users">the list of users.</param>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the users.</returns>
        public static List<AutomaticAssignUserModel> GetValidUsersByLoad(List<UserModel> users, List<UserOrderModel> userOrders, ISapAdapter sapAdapter)
        {
            var validUsers = new List<AutomaticAssignUserModel>();
            users.ForEach(user =>
            {
                var pedidosId = userOrders.Where(x => x.Userid.Equals(user.Id) && x.IsProductionOrder).Select(y => int.Parse(y.Productionorderid)).Distinct().ToList();
                var orders = sapAdapter.PostSapAdapter(pedidosId, ServiceConstants.GetUsersByOrdersById).Result;
                var ordersSap = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(JsonConvert.SerializeObject(orders.Response));
                var total = ordersSap.Sum(x => x.Quantity);
                if (ServiceShared.CalculateAnd(total < user.Piezas, !user.Classification.Equals(ServiceConstants.UserClassificationDZ)))
                {
                    validUsers.Add(new AutomaticAssignUserModel
                    {
                        User = user,
                        TotalCount = (int)total,
                        ItemCodes = ordersSap.Select(x => x.ProductoId).ToList(),
                        ProductionOrders = ordersSap.Select(x => x.OrdenId).ToList(),
                    });
                }
            });
            return validUsers.OrderBy(x => x.TotalCount).ThenBy(y => y.User.FirstName).ToList();
        }

        /// <summary>
        /// Gets the user available by formula.
        /// </summary>
        /// <param name="users">the ussers with item codes.</param>
        /// <param name="orderDetail">the order with details.</param>
        /// <param name="userOrders">The user orders.</param>
        /// <param name="listRelation">list relation.</param>
        /// <returns>the data to return.</returns>
        public static Tuple<Dictionary<int, string>, List<int>> GetValidUsersByFormula(
            List<AutomaticAssignUserModel> users, List<OrderWithDetailModel> orderDetail, List<UserOrderModel> userOrders, List<RelationDxpDocEntryModel> listRelation)
        {
            var dictUserPedido = new Dictionary<int, string>();
            var listOrdersWithNoUser = new List<int>();
            var localUserOrders = new List<UserOrderModel>();

            foreach (var p in orderDetail)
            {
                var ordersByDxp = listRelation.FirstOrDefault(x => x.DxpDocNum == p.Order.DocNumDxp);
                ordersByDxp ??= new RelationDxpDocEntryModel { DocNum = new List<RelationOrderAndTypeModel>() };
                var pedidoIds = ordersByDxp.DocNum.Select(x => x.DocNum.ToString()).ToList();

                var ordersByUser = userOrders.Where(y => pedidoIds.Contains(y.Salesorderid)).ToList();
                ordersByUser.AddRange(localUserOrders.Where(y => pedidoIds.Contains(y.Salesorderid)));

                var orderByType = ordersByDxp.DocNum.Where(x => x.OrderType == p.Order.OrderType).ToList();
                var ordersByUserContainsType = ordersByUser.Any(x => orderByType.Any(y => x.Salesorderid == y.DocNum.ToString()));
                if (ordersByDxp.DocNum.Any() && ordersByUser.Any() && orderByType.Any() && ordersByUserContainsType)
                {
                    var ordersIdsByType = orderByType.Select(y => y.DocNum.ToString()).ToList();
                    var userByType = ordersByUser.Where(x => ordersIdsByType.Contains(x.Salesorderid));
                    dictUserPedido.Add(p.Order.DocNum, userByType.FirstOrDefault().Userid);
                    users.ForEach(x =>
                    {
                        x.TotalCount = ServiceShared.CalculateTernary(x.User.Id.Equals(dictUserPedido[p.Order.DocNum]), p.Detalle.Where(z => z.QtyPlanned.HasValue).Sum(y => y.QtyPlanned.Value) + x.TotalCount, x.TotalCount);
                    });

                    localUserOrders.Add(new UserOrderModel { Userid = ordersByUser.FirstOrDefault().Userid, Salesorderid = p.Order.DocNum.ToString() });
                    continue;
                }

                if (ServiceShared.CalculateAnd(p.Order.OrderType != ServiceConstants.Mix, !users.Any(x => x.User.Classification == p.Order.OrderType)))
                {
                    listOrdersWithNoUser.Add(p.Order.DocNum);
                    continue;
                }

                var localUsers = ServiceShared.CalculateTernary(p.Order.OrderType == ServiceConstants.Mix, users, users.Where(x => x.User.Classification == p.Order.OrderType).ToList());
                localUsers = localUsers.OrderBy(x => x.TotalCount).ThenBy(x => x.User.FirstName).ToList();

                if (!p.Detalle.Any(d => d.CodigoProducto.Contains("   ")))
                {
                    dictUserPedido.Add(p.Order.DocNum, localUsers.FirstOrDefault().User.Id);
                    localUserOrders.Add(new UserOrderModel { Userid = localUsers.FirstOrDefault().User.Id, Salesorderid = p.Order.DocNum.ToString() });
                    continue;
                }

                dictUserPedido = GetEntryUserValue(dictUserPedido, p.Detalle, localUsers, p.Order.DocNum, localUsers.FirstOrDefault().User.Id, userOrders);
                localUserOrders.Add(new UserOrderModel { Userid = dictUserPedido[p.Order.DocNum], Salesorderid = p.Order.DocNum.ToString() });

                users.ForEach(x =>
                {
                    x.TotalCount = ServiceShared.CalculateTernary(x.User.Id.Equals(dictUserPedido[p.Order.DocNum]), p.Detalle.Where(z => z.QtyPlanned.HasValue).Sum(y => y.QtyPlanned.Value) + x.TotalCount, x.TotalCount);
                });
            }

            return new Tuple<Dictionary<int, string>, List<int>>(dictUserPedido, listOrdersWithNoUser);
        }

        /// <summary>
        /// Place the status for the orders.
        /// </summary>
        /// <param name="listFromOrders">the list sent from front.</param>
        /// <param name="listFromSales">list from DB.</param>
        /// <param name="sapOrders">The sapOrders.</param>
        /// <param name="user">the user to update.</param>
        /// <param name="statusOrder">Status for the order fab.</param>
        /// <param name="userLogistic">user modificate.</param>
        /// <param name="isFromAssignOrder">Is from assignOrder.</param>
        /// <param name="qfbInfoValidated">Qfb info validated.</param>
        /// <returns>the data.</returns>
        public static Tuple<List<UserOrderModel>, List<SalesLogs>> GetUpdateUserOrderModel(
            List<UserOrderModel> listFromOrders,
            List<UserOrderModel> listFromSales,
            List<OrderWithDetailModel> sapOrders,
            string user,
            string statusOrder,
            string userLogistic,
            bool isFromAssignOrder,
            QfbTecnicInfoDto qfbInfoValidated)
        {
            var listToUpdate = new List<UserOrderModel>();
            var listOrderLogToInsert = new List<SalesLogs>();
            listFromSales
                .GroupBy(x => x.Salesorderid)
                .ToList()
                .ForEach(y =>
                {
                    var orderWithDetails = sapOrders.FirstOrDefault(z => z.Order != null && z.Order.PedidoId.ToString().Equals(y.Key));
                    var currentOrdersBySale = listFromOrders.Where(z => z.Salesorderid == y.Key).ToList();
                    var currentOrders = currentOrdersBySale.Select(x => x.Productionorderid).ToList();
                    var missing = y.Any(z => !string.IsNullOrEmpty(z.Productionorderid) && z.Status == ServiceConstants.Planificado && !currentOrders.Contains(z.Productionorderid)) || y.Count(z => !string.IsNullOrEmpty(z.Productionorderid)) != orderWithDetails.Detalle.Count;

                    currentOrdersBySale.ForEach(o =>
                    {
                        o.Userid = user;
                        o.Status = statusOrder;
                        o.StatusForTecnic = statusOrder;
                        o.ReassignmentDate = ServiceShared.CalculateTernary(isFromAssignOrder, o.ReassignmentDate, DateTime.Now);
                        o.AssignmentDate = ServiceShared.CalculateTernary(isFromAssignOrder, DateTime.Now, o.AssignmentDate);
                        o.PackingDate = ServiceShared.CalculateTernary(isFromAssignOrder, o.PackingDate, null);
                        o.TecnicId = ServiceShared.CalculateTernary(!string.IsNullOrEmpty(y.Key), qfbInfoValidated.TecnicId, null);
                        listToUpdate.Add(o);
                        /** add logs**/
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(userLogistic, new List<UserOrderModel> { o }));
                    });

                    if (!string.IsNullOrEmpty(y.Key))
                    {
                        var pedido = listFromSales.FirstOrDefault(x => x.Salesorderid == y.Key && string.IsNullOrEmpty(x.Productionorderid));
                        pedido.Status = ServiceShared.CalculateTernary(missing, pedido.Status, ServiceConstants.Liberado);
                        pedido.Userid = user;
                        pedido.StatusForTecnic = pedido.Status;
                        pedido.ReassignmentDate = ServiceShared.CalculateTernary(isFromAssignOrder, pedido.ReassignmentDate, DateTime.Now);
                        pedido.AssignmentDate = ServiceShared.CalculateTernary(isFromAssignOrder, DateTime.Now, pedido.AssignmentDate);
                        pedido.TecnicId = qfbInfoValidated.TecnicId;
                        listToUpdate.Add(pedido);
                        if (!missing)
                        {
                            /** add logs**/
                            listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(userLogistic, new List<UserOrderModel> { pedido }));
                        }
                    }
                });
            return new Tuple<List<UserOrderModel>, List<SalesLogs>>(listToUpdate, listOrderLogToInsert);
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

            var user = users.OrderBy(x => x.TotalCount).ThenBy(x => x.User.FirstName).AsEnumerable().FirstOrDefault();

            if (user != null)
            {
                pedidoUser.Add(pedidoId, user.User.Id);
                return pedidoUser;
            }

            pedidoUser.Add(pedidoId, defaultUser);
            return pedidoUser;
        }

        /// <summary>
        /// Sends to update.
        /// </summary>
        /// <param name="serviceLayerAdapterService">the service layer.</param>
        /// <param name="listToUpdate">the list to update.</param>
        /// <returns>teh dict result.</returns>
        private static async Task<Dictionary<string, string>> UpdateSapOrders(ISapServiceLayerAdapterService serviceLayerAdapterService, List<UpdateFabOrderModel> listToUpdate)
        {
            var resultSap = await serviceLayerAdapterService.PostAsync(listToUpdate, ServiceConstants.UpdateFabOrder);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
        }
    }
}
