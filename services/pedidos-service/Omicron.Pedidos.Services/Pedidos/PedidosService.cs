// <summary>
// <copyright file="PedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Enums;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// the pedidos service.
    /// </summary>
    public class PedidosService : IPedidosService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly ISapDiApi sapDiApi;

        private readonly IUsersService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="userService">The user service.</param>
        public PedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi, IUsersService userService)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosId">the ids of the orders.</param>
        /// <returns>the result.</returns>
        public async Task<ResultModel> ProcessOrders(ProcessOrderModel pedidosId)
        {
            var orders = await this.sapAdapter.PostSapAdapter(pedidosId.ListIds, ServiceConstants.GetOrderWithDetail);
            var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));

            ordersSap.ForEach(o =>
            {
                o.Detalle = o.Detalle.Where(x => string.IsNullOrEmpty(x.Status)).ToList();
            });

            var resultSap = await this.sapDiApi.PostToSapDiApi(ordersSap, ServiceConstants.CreateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);

            var prodOrders = await this.sapAdapter.PostSapAdapter(listToLook, ServiceConstants.GetProdOrderByOrderItem);
            var listOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(prodOrders.Response.ToString());

            var listToInsert = ServiceUtils.CreateUserOrder(listOrders);
            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(pedidosId.User, pedidosId.ListIds, ServiceConstants.OrdenVentaPlan, ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(pedidosId.User, listOrders.Select(x => x.OrdenId).ToList(), ServiceConstants.OrdenFabricacionPlan, ServiceConstants.OrdenFab));

            await this.pedidosDao.InsertUserOrder(listToInsert);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            var userError = listErrorId.Any() ? ServiceConstants.ErrorAlInsertar : null;
            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// Process by order.
        /// </summary>
        /// <param name="processByOrder">the orders.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> ProcessByOrder(ProcessByOrderModel processByOrder)
        {
            var listSalesOrder = new List<int> { processByOrder.PedidoId };
            var sapResponse = await this.sapAdapter.PostSapAdapter(listSalesOrder, ServiceConstants.GetOrderWithDetail);
            var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(sapResponse.Response));

            var orders = ordersSap.FirstOrDefault(x => x.Order.PedidoId == processByOrder.PedidoId);
            var completeListOrders = orders.Detalle.Count;
            var ordersToCreate = orders.Detalle.Where(x => processByOrder.ProductId.Contains(x.CodigoProducto)).ToList();

            var objectToCreate = ServiceUtils.CreateOrderWithDetail(orders, ordersToCreate);
            var resultSap = await this.sapDiApi.PostToSapDiApi(new List<OrderWithDetailModel> { objectToCreate }, ServiceConstants.CreateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);

            var prodOrders = await this.sapAdapter.PostSapAdapter(listToLook, ServiceConstants.GetProdOrderByOrderItem);
            var listOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(prodOrders.Response.ToString());

            var dataBaseOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { processByOrder.PedidoId.ToString() })).ToList();

            var dataToInsert = ServiceUtils.CreateUserModel(listOrders);

            var saleOrder = dataBaseOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid));
            bool insertUserOrdersale = false;

            if (saleOrder == null)
            {
                saleOrder = new UserOrderModel
                {
                    Salesorderid = processByOrder.PedidoId.ToString(),
                };

                insertUserOrdersale = true;
            }

            saleOrder.Status = dataBaseOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList().Count + dataToInsert.Count == completeListOrders ? ServiceConstants.Liberado : ServiceConstants.Planificado;

            if (insertUserOrdersale)
            {
                dataToInsert.Add(saleOrder);
            }
            else
            {
                await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });
            }

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(processByOrder.UserId, new List<int> { processByOrder.PedidoId }, ServiceConstants.OrdenVentaPlan, ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(processByOrder.UserId, listOrders.Select(x => x.OrdenId).ToList(), ServiceConstants.OrdenFabricacionPlan, ServiceConstants.OrdenFab));

            await this.pedidosDao.InsertUserOrder(dataToInsert);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            var userError = listErrorId.Any() ? ServiceConstants.ErrorAlInsertar : null;
            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// returns the orders.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderBySaleOrder(listIdString);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(orders), null);
        }

        /// <summary>
        /// Gets the QFB orders (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetFabOrderByUserID(string userId)
        {
            var userOrders = (await this.pedidosDao.GetUserOrderByUserId(new List<string> { userId })).ToList();
            var resultFormula = await this.GetSapOrders(userOrders);

            var groups = ServiceUtils.GroupUserOrder(resultFormula, userOrders);
            return ServiceUtils.CreateResult(true, 200, null, groups, null);
        }

        /// <summary>
        /// Gets the list of user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetUserOrdersByUserId(List<string> listIds)
        {
            var userOrder = await this.pedidosDao.GetUserOrderByUserId(listIds);
            return ServiceUtils.CreateResult(true, 200, null, userOrder, null);
        }

        /// <summary>
        /// Assign the orders.
        /// </summary>
        /// <param name="manualAssign">the manual assign.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> AssignOrder(ManualAssignModel manualAssign)
        {
            if (manualAssign.OrderType.Equals(ServiceConstants.TypePedido))
            {
                return await AsignarLogic.AssignPedido(manualAssign, this.pedidosDao, this.sapAdapter, this.sapDiApi);
            }
            else
            {
                return await AsignarLogic.AssignOrder(manualAssign, this.pedidosDao, this.sapDiApi);
            }
        }

        /// <summary>
        /// Updates the formula for an order.
        /// </summary>
        /// <param name="updateFormula">upddates the formula.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> UpdateComponents(UpdateFormulaModel updateFormula)
        {
            var resultSapApi = await this.sapDiApi.PostToSapDiApi(updateFormula, ServiceConstants.UpdateFormula);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(resultSapApi.Response), null);
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="updateStatusOrder">the status model.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> UpdateStatusOrder(List<UpdateStatusOrderModel> updateStatusOrder)
        {
            var orders = updateStatusOrder.Select(x => x.OrderId.ToString()).ToList();
            var ordersList = (await this.pedidosDao.GetUserOrderByProducionOrder(orders)).ToList();

            var listOrderLogs = new List<OrderLogModel>();

            ordersList.ForEach(x =>
            {
                var order = updateStatusOrder.FirstOrDefault(y => y.OrderId.ToString().Equals(x.Productionorderid));
                x.Status = order == null ? x.Status : order.Status;
                x.Userid = order == null ? x.Userid : order.UserId;
                listOrderLogs.AddRange(ServiceUtils.CreateOrderLog(x.Userid, new List<int> { order.OrderId }, string.Format(ServiceConstants.OrdenProceso, x.Productionorderid), ServiceConstants.OrdenFab));
            });

            await this.pedidosDao.UpdateUserOrders(ordersList);
            await this.pedidosDao.InsertOrderLog(listOrderLogs);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(ordersList), null);
        }

        /// <summary>
        /// Gets the connection to sap di api.
        /// </summary>
        /// <returns>the conection.</returns>
        public async Task<ResultModel> ConnectDiApi()
        {
            var sapResponse = await this.sapDiApi.GetSapDiApi(ServiceConstants.ConnectSapDiApi);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(sapResponse.Response), null);
        }

        /// <summary>
        /// Change order status to cancel.
        /// </summary>
        /// <param name="cancelOrders">Update orders info.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CancelOrder(List<CancelOrderModel> cancelOrders)
        {
            return await this.CancelFabOrders(ServiceConstants.OrdenVenta, cancelOrders);
        }

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</para
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CancelFabOrder(List<CancelOrderModel> cancelOrders)
        {
            return await this.CancelFabOrders(ServiceConstants.OrdenFab, cancelOrders);
        }

        /// <summary>
        /// Makes the automatic assign.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> AutomaticAssign(AutomaticAssingModel assignModel)
        {
            var invalidStatus = new List<string> { ServiceConstants.Finalizado, ServiceConstants.Pendiente };
            var users = await ServiceUtils.GetUsersByRole(this.userService, ServiceConstants.QfbRoleId.ToString(), true);
            var userOrders = (await this.pedidosDao.GetUserOrderByUserId(users.Select(x => x.Id).ToList())).ToList();
            userOrders = userOrders.Where(x => !invalidStatus.Contains(x.Status)).ToList();
            var validUsers = await AsignarLogic.GetValidUsersByLoad(users, userOrders, this.sapAdapter, 200);

            if (!validUsers.Any())
            {
                throw new CustomServiceException(ServiceConstants.ErrorQfbAutomatico, System.Net.HttpStatusCode.BadRequest);
            }

            var pedidosId = assignModel.DocEntry.Select(x => x).ToList();
            var pedidosString = assignModel.DocEntry.Select(x => x.ToString()).ToList();
            var orders = await this.sapAdapter.PostSapAdapter(pedidosId, ServiceConstants.GetOrderWithDetail);
            var ordersSap = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));

            var userSaleOrder = AsignarLogic.GetValidUsersByFormula(validUsers, ordersSap, userOrders);

            var listToUpdate = ServiceUtils.GetOrdersToAssign(ordersSap);
            var resultSap = await this.sapDiApi.PostToSapDiApi(listToUpdate, ServiceConstants.UpdateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFavOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var userOrdersToUpdate = (await this.pedidosDao.GetUserOrderBySaleOrder(pedidosString)).ToList();

            var listOrderToInsert = new List<OrderLogModel>();
            userOrdersToUpdate.ForEach(x =>
            {
                int.TryParse(x.Salesorderid, out int saleOrderInt);
                int.TryParse(x.Productionorderid, out int productionId);

                if (userSaleOrder.ContainsKey(saleOrderInt))
                {
                    x.Status = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.Liberado : ServiceConstants.Asignado;
                    x.Userid = userSaleOrder[saleOrderInt];

                    var orderId = string.IsNullOrEmpty(x.Productionorderid) ? saleOrderInt : productionId;
                    var ordenType = string.IsNullOrEmpty(x.Productionorderid) ? ServiceConstants.OrdenVenta : ServiceConstants.OrdenFab;
                    var textAction = string.IsNullOrEmpty(x.Productionorderid) ? string.Format(ServiceConstants.AsignarVenta, userSaleOrder[saleOrderInt]) : string.Format(ServiceConstants.AsignarOrden, userSaleOrder[saleOrderInt]);
                    listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(assignModel.UserLogistic, new List<int> { orderId }, textAction, ordenType));
                }
            });

            await this.pedidosDao.UpdateUserOrders(userOrders);
            await this.pedidosDao.InsertOrderLog(listOrderToInsert);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// gets the order from sap.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <returns>tje data.</returns>
        private async Task<List<CompleteFormulaWithDetalle>> GetSapOrders(List<UserOrderModel> userOrders)
        {
            var resultFormula = new List<CompleteFormulaWithDetalle>();

            await Task.WhenAll(userOrders.Select(async x =>
            {
                if (!string.IsNullOrEmpty(x.Productionorderid))
                {
                    var route = $"{ServiceConstants.GetFormula}{x.Productionorderid}";
                    var result = await this.sapAdapter.GetSapAdapter(route);

                    lock (resultFormula)
                    {
                        var formula = JsonConvert.DeserializeObject<CompleteFormulaWithDetalle>(JsonConvert.SerializeObject(result.Response));
                        resultFormula.Add(formula);
                    }
                }
            }));

            return resultFormula;
        }

        /// <summary>
        /// Cancel fabrication orders by level.
        /// </summary>
        /// <param name="level">The level cancellation  => Sales orders or fabrication order.</param>
        /// <param name="cancelOrders">Orders to cancel.</param>
        /// <returns>Orders with updated info.</returns>urns>
        private async Task<ResultModel> CancelFabOrders(string level, List<CancelOrderModel> cancelOrders)
        {
            var orderIds = cancelOrders.Select(x => x.OrderId.ToString()).ToList();
            var userOrders = new List<UserOrderModel>();
            var logs = new List<OrderLogModel>();
            var successfuly = new List<CancelOrderModel>();
            var failed = new List<CancelOrderModel>();

            // Get related fabrication orders
            if (level.Equals(ServiceConstants.OrdenVenta))
            {
                userOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(orderIds)).ToList();
            }
            else if (level.Equals(ServiceConstants.OrdenFab))
            {
                userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(orderIds)).ToList();
            }

            foreach (var order in userOrders)
            {
                var newOrderInfo = new CancelOrderModel();
                if (level.Equals(ServiceConstants.OrdenVenta))
                {
                    newOrderInfo = cancelOrders.First(y => y.OrderId.ToString().Equals(order.Salesorderid));
                }
                else if (level.Equals(ServiceConstants.OrdenFab))
                {
                    newOrderInfo = cancelOrders.First(y => y.OrderId.ToString().Equals(order.Productionorderid));
                }

                // Dircarp cancelled orders
                if (order.Status.Equals(ServiceConstants.Cancelled))
                {
                    successfuly.Add(newOrderInfo);
                    continue;
                }

                // Dircarp finalized orders
                if (order.Status.Equals(ServiceConstants.Finalizado))
                {
                    continue;
                }

                var cancelledOnSap = true;

                // Process to cancel a fabrication order in SAP.
                if (!string.IsNullOrEmpty(order.Productionorderid))
                {
                    var payload = new { OrderId = order.Productionorderid };
                    var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.CancelFabOrder);
                    cancelledOnSap = result.Success && (result.Response.ToString().Equals(ServiceConstants.Ok) || result.Response.ToString().Equals(ServiceConstants.ErrorProductionOrderCancelled));
                }

                // Process to cancel on local db
                if (cancelledOnSap)
                {
                    order.Status = ServiceConstants.Cancelled;
                    successfuly.Add(newOrderInfo);
                    logs.AddRange(ServiceUtils.CreateOrderLog(newOrderInfo.UserId, new List<int> { newOrderInfo.OrderId }, string.Format(ServiceConstants.OrderCancelled, newOrderInfo.OrderId), level));
                    continue;
                }

                failed.Add(newOrderInfo);
            }

            // Update in local data base
            await this.pedidosDao.UpdateUserOrders(userOrders);
            await this.pedidosDao.InsertOrderLog(logs);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(results), null);
        }
    }
}
