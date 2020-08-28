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
    using AutoMapper.Internal;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Enums;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Resources.Extensions;
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
            var listToSend = new List<OrderWithDetailModel>();

            ordersSap.ForEach(o =>
            {
                var listDetalle = new List<CompleteDetailOrderModel>();

                o.Detalle
                .Where(x => string.IsNullOrEmpty(x.Status))
                .ToList()
                .ForEach(y =>
                {
                    y.DescripcionProducto = y.DescripcionCorta;
                    listDetalle.Add(y);
                });

                var objectToCreate = new OrderWithDetailModel { Order = o.Order, Detalle = listDetalle };
                listToSend.Add(objectToCreate);
            });

            var resultSap = await this.sapDiApi.PostToSapDiApi(listToSend, ServiceConstants.CreateFabOrder);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSap.Response.ToString());
            var listToLook = ServiceUtils.GetValuesByExactValue(dictResult, ServiceConstants.Ok);
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);

            var prodOrders = await this.sapAdapter.PostSapAdapter(listToLook, ServiceConstants.GetProdOrderByOrderItem);
            var listOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(prodOrders.Response.ToString());

            var listPedidos = pedidosId.ListIds.Select(x => x.ToString()).ToList();
            var dataBaseSaleOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(listPedidos)).ToList();

            var listToInsert = ServiceUtils.CreateUserModelOrders(listOrders);
            var dataToInsertUpdate = ServiceUtils.GetListToUpdateInsert(pedidosId.ListIds, dataBaseSaleOrders);
            listToInsert.AddRange(dataToInsertUpdate.Item1);
            var listToUpdate = new List<UserOrderModel>(dataToInsertUpdate.Item2);

            var listOrderToInsert = new List<OrderLogModel>();
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(pedidosId.User, pedidosId.ListIds, ServiceConstants.OrdenVentaPlan, ServiceConstants.OrdenVenta));
            listOrderToInsert.AddRange(ServiceUtils.CreateOrderLog(pedidosId.User, listOrders.Select(x => x.OrdenId).ToList(), ServiceConstants.OrdenFabricacionPlan, ServiceConstants.OrdenFab));

            await this.pedidosDao.InsertUserOrder(listToInsert);
            await this.pedidosDao.UpdateUserOrders(listToUpdate);
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

            var dataToInsert = ServiceUtils.CreateUserModelOrders(listOrders);

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

            saleOrder.Status = dataBaseOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList().Count + dataToInsert.Count == completeListOrders ? ServiceConstants.Planificado : ServiceConstants.Abierto;

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
        /// Get the user order by fabrication order id.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetUserOrderByFabOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderByProducionOrder(listIdString);
            return ServiceUtils.CreateResult(true, 200, null, orders, null);
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
            if (resultSapApi.Success && !string.IsNullOrEmpty(updateFormula.Comments))
            {
                await this.UpdateFabOrderComments(updateFormula.FabOrderId, updateFormula.Comments);
            }

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
        /// updates order comments.
        /// </summary>
        /// <param name="updateComments">Fabrication order comments.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> UpdateFabOrderComments(List<UpdateOrderCommentsModel> updateComments)
        {
            var successfuly = new List<UserOrderModel>();
            var failed = new List<UpdateOrderCommentsModel>();

            foreach (var item in updateComments)
            {
                var result = await this.UpdateFabOrderComments(item.OrderId, item.Comments);
                if (result != null)
                {
                    successfuly.Add(result);
                }
                else
                {
                    failed.Add(item);
                }
            }

            var resultContent = new { successfuly, failed };
            return ServiceUtils.CreateResult(true, 200, null, resultContent, null);
        }

        /// <summary>
        ///  Update order comments.
        /// </summary>
        /// <param name="fabOrderId">Order to update.</param>
        /// <param name="comments">Comment to set.</param>
        /// <returns>Updated order. </returns>
        public async Task<UserOrderModel> UpdateFabOrderComments(int fabOrderId, string comments)
        {
            var fabOrderToUpdate = (await this.pedidosDao.GetUserOrderByProducionOrder(new List<string> { fabOrderId.ToString() })).FirstOrDefault();

            if (fabOrderToUpdate == null)
            {
                return null;
            }

            var listOrderLogs = new List<OrderLogModel>();
            fabOrderToUpdate.Comments = comments;
            await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { fabOrderToUpdate });
            return fabOrderToUpdate;
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
        public async Task<ResultModel> CancelOrder(List<OrderIdModel> cancelOrders)
        {
            var missingOrders = new List<OrderIdModel>();
            var missingSalesOrders = new List<OrderWithDetailModel>();
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();

            // User ordes to update
            var userOrdersToUpdate = new List<UserOrderModel>();
            var userOrdersToInsert = new List<UserOrderModel>();

            // Process existing orders
            foreach (var orderToCancel in cancelOrders)
            {
                // Get related fabrication orders
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { orderToCancel.OrderId.ToString() })).ToList();
                if (!relatedOrders.Any())
                {
                    missingOrders.Add(orderToCancel);
                    continue;
                }

                // Validate non finished production orders
                var salesOrder = relatedOrders.First(x => string.IsNullOrEmpty(x.Productionorderid));

                if (salesOrder.Status.Equals(ServiceConstants.Finalizado))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToCancel, ServiceConstants.ReasonSalesOrderFinished));
                    continue;
                }

                // Validate non finished production orders
                var finishedOrders = relatedOrders.Where(x => x.Status.Equals(ServiceConstants.Finalizado)).ToList();
                if (finishedOrders.Any())
                {
                    foreach (var finishedOrder in finishedOrders)
                    {
                        var message = string.Format(ServiceConstants.ReasonProductionOrderFinished, finishedOrder.Productionorderid);
                        failed.Add(ServiceUtils.CreateCancellationFail(orderToCancel, message));
                    }

                    continue;
                }

                foreach (var order in relatedOrders)
                {
                    var cancelledOnSap = true;
                    var docType = ServiceConstants.OrdenVenta;

                    // Process to cancel a fabrication order in SAP.
                    if (!string.IsNullOrEmpty(order.Productionorderid))
                    {
                        docType = ServiceConstants.OrdenFab;
                        var payload = new { OrderId = order.Productionorderid };
                        var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.CancelFabOrder);
                        cancelledOnSap = result.Success && (result.Response.ToString().Equals(ServiceConstants.Ok) || result.Response.ToString().Equals(ServiceConstants.ErrorProductionOrderCancelled));
                    }

                    // Process to cancel on local db
                    if (cancelledOnSap)
                    {
                        order.Status = ServiceConstants.Cancelled;
                        userOrdersToUpdate.Add(order);
                        successfuly.Add(orderToCancel);

                        if (docType.Equals(ServiceConstants.OrdenVenta))
                        {
                            logs.AddRange(ServiceUtils.CreateOrderLog(orderToCancel.UserId, new List<int> { orderToCancel.OrderId }, string.Format(ServiceConstants.OrderCancelled, orderToCancel.OrderId), ServiceConstants.OrdenVenta));
                        }

                        continue;
                    }

                    failed.Add(ServiceUtils.CreateCancellationFail(orderToCancel, ServiceConstants.ReasonSapError));
                }
            }

            // Update in local data base
            await this.pedidosDao.UpdateUserOrders(userOrdersToUpdate);
            await this.pedidosDao.InsertOrderLog(logs);

            // Process missing sales orders
            if (missingOrders.Any())
            {
                var orders = await this.sapAdapter.PostSapAdapter(missingOrders.Select(x => x.OrderId).ToList(), ServiceConstants.GetOrderWithDetail);
                var sapOrders = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(orders.Response));
                sapOrders = sapOrders.Where(x => x.Order != null).ToList();
                sapOrders.ForEach(o =>
                {
                    o.Detalle = o.Detalle.Where(x => !string.IsNullOrEmpty(x.Status)).ToList();
                });
                missingSalesOrders = sapOrders;

                var newLogs = new List<OrderLogModel>();
                foreach (var order in missingSalesOrders)
                {
                    var orderToCancel = cancelOrders.First(x => x.OrderId.Equals(order.Order.DocNum));

                    if (order.Order.PedidoStatus.Equals("C"))
                    {
                        failed.Add(ServiceUtils.CreateCancellationFail(orderToCancel, ServiceConstants.ReasonSalesOrderFinished));
                        continue;
                    }

                    var finishedOrders = order.Detalle.Where(x => x.Status.Equals("L")).ToList();
                    if (finishedOrders.Any())
                    {
                        foreach (var finishedOrder in finishedOrders)
                        {
                            var message = string.Format(ServiceConstants.ReasonProductionOrderFinished, finishedOrder.OrdenFabricacionId);
                            failed.Add(ServiceUtils.CreateCancellationFail(orderToCancel, message));
                        }

                        continue;
                    }

                    var missingUserOrdersToValidate = order.Detalle.Select(x => new UserOrderModel
                    {
                        Status = ServiceConstants.Cancelled,
                        Salesorderid = order.Order.DocNum.ToString(),
                        Productionorderid = x.OrdenFabricacionId.ToString(),
                    }).ToList();

                    foreach (var orderToUpdate in missingUserOrdersToValidate)
                    {
                        var payload = new { OrderId = orderToUpdate.Productionorderid };
                        var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.CancelFabOrder);
                    }

                    missingUserOrdersToValidate.Add(new UserOrderModel
                    {
                        Status = ServiceConstants.Cancelled,
                        Salesorderid = order.Order.DocNum.ToString(),
                    });

                    userOrdersToInsert.AddRange(missingUserOrdersToValidate);
                    successfuly.Add(orderToCancel);

                    newLogs.AddRange(ServiceUtils.CreateOrderLog(orderToCancel.UserId, new List<int> { orderToCancel.OrderId }, string.Format(ServiceConstants.OrderCancelled, orderToCancel.OrderId), ServiceConstants.OrdenVenta));
                }

                await this.pedidosDao.InsertUserOrder(userOrdersToInsert);
                await this.pedidosDao.InsertOrderLog(logs);
            }

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// Change order status to finish.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CloseSalesOrders(List<OrderIdModel> finishOrders)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();

            foreach (var orderToFinish in finishOrders)
            {
                var ids = new List<string> { orderToFinish.OrderId.ToString() };
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(ids)).ToList();

                // Identify sales order
                var salesOrder = relatedOrders.First(x => string.IsNullOrEmpty(x.Productionorderid));
                if (!salesOrder.Status.Equals(ServiceConstants.Completed))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, ServiceConstants.ReasonOrderNonCompleted));
                    continue;
                }

                var salesOrderId = int.Parse(salesOrder.Salesorderid);

                // Identify production order
                var productionOrders = relatedOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList();
                productionOrders = productionOrders.Where(x => !x.Status.Equals(ServiceConstants.Finalizado)).ToList();

                // Validate completed production orders
                var nonCompleted = productionOrders.Where(x => !x.Status.Equals(ServiceConstants.Completed)).ToList();
                if (nonCompleted.Any())
                {
                    foreach (var completeOrder in nonCompleted)
                    {
                        var message = string.Format(ServiceConstants.ReasonProductionOrderNonCompleted, completeOrder.Productionorderid);
                        failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, message));
                    }

                    continue;
                }

                // Update in SAP
                var payload = productionOrders.Select(x => new { OrderId = x.Productionorderid });
                var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.FinishFabOrder);

                if (!result.Success)
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, ServiceConstants.ReasonSapConnectionError));
                    continue;
                }

                var resultMessages = JsonConvert.DeserializeObject<Dictionary<int, string>>(result.Response.ToString());

                // Map errors
                foreach (var error in resultMessages.Where(x => x.Key > 0))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, error.Value));
                }

                // Update production order status
                foreach (var userOrder in productionOrders)
                {
                    int prodOrderId = int.Parse(userOrder.Productionorderid);
                    if (!resultMessages.Keys.Any(x => x.Equals(prodOrderId)))
                    {
                        userOrder.CloseUserId = orderToFinish.UserId;
                        userOrder.CloseDate = DateTime.Now.FormatedDate();
                        userOrder.Status = ServiceConstants.Finalizado;

                        logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { prodOrderId }, string.Format(ServiceConstants.OrderFinished, prodOrderId), ServiceConstants.OrdenFab));
                    }
                }

                await this.pedidosDao.UpdateUserOrders(productionOrders);

                // Update sales order status
                if (resultMessages.Keys.Any(x => x.Equals(0)))
                {
                    salesOrder.CloseUserId = orderToFinish.UserId;
                    salesOrder.CloseDate = DateTime.Now.FormatedDate();
                    salesOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { salesOrderId }, string.Format(ServiceConstants.OrderFinished, salesOrderId), ServiceConstants.OrdenVenta));

                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { salesOrder });
                    successfuly.Add(orderToFinish);
                }
            }

            await this.pedidosDao.InsertOrderLog(logs);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</para
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CancelFabOrder(List<OrderIdModel> cancelOrders)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();

            // Get related fabrication orders
            var orderIds = cancelOrders.Select(x => x.OrderId.ToString()).ToList();
            var userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(orderIds)).ToList();

            foreach (var missing in cancelOrders.Where(x => !userOrders.Any(y => y.Productionorderid.Equals(x.OrderId.ToString()))))
            {
                failed.Add(ServiceUtils.CreateCancellationFail(missing, ServiceConstants.ReasonNotExistsOrder));
            }

            // Process existing orders
            foreach (var order in userOrders)
            {
                var newOrderInfo = cancelOrders.First(y => y.OrderId.ToString().Equals(order.Productionorderid));

                // Dircarp cancelled orders
                if (order.Status.Equals(ServiceConstants.Cancelled))
                {
                    successfuly.Add(newOrderInfo);
                    continue;
                }

                // Dircarp finalized orders
                if (order.Status.Equals(ServiceConstants.Finalizado))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(newOrderInfo, ServiceConstants.ReasonOrderFinished));
                    continue;
                }

                var payload = new { OrderId = order.Productionorderid };
                var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.CancelFabOrder);
                var cancelledOnSap = result.Success && (result.Response.ToString().Equals(ServiceConstants.Ok) || result.Response.ToString().Equals(ServiceConstants.ErrorProductionOrderCancelled));

                // Process to cancel on local db
                if (cancelledOnSap)
                {
                    order.Status = ServiceConstants.Cancelled;
                    successfuly.Add(newOrderInfo);
                    logs.AddRange(ServiceUtils.CreateOrderLog(newOrderInfo.UserId, new List<int> { newOrderInfo.OrderId }, string.Format(ServiceConstants.OrderCancelled, newOrderInfo.OrderId), ServiceConstants.OrdenFab));
                    continue;
                }

                failed.Add(ServiceUtils.CreateCancellationFail(newOrderInfo, ServiceConstants.ReasonSapError));
            }

            // Update in local data base
            await this.pedidosDao.UpdateUserOrders(userOrders);

            // Cancel orders with complete cancellations
            var salesOrdersToUpdate = new List<UserOrderModel>();
            foreach (var order in userOrders)
            {
                var newOrderInfo = cancelOrders.First(y => y.OrderId.ToString().Equals(order.Productionorderid));
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { order.Salesorderid })).ToList();
                var productionOrders = relatedOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList();
                var salesOrder = relatedOrders.First(x => string.IsNullOrEmpty(x.Productionorderid));

                if (productionOrders.All(x => x.Status.Equals(ServiceConstants.Cancelled)) && !salesOrder.Status.Equals(ServiceConstants.Finalizado))
                {
                    salesOrder.Status = ServiceConstants.Cancelled;
                    salesOrdersToUpdate.Add(salesOrder);
                    logs.AddRange(ServiceUtils.CreateOrderLog(newOrderInfo.UserId, new List<int> { int.Parse(salesOrder.Salesorderid) }, string.Format(ServiceConstants.OrderCancelled, newOrderInfo.OrderId), ServiceConstants.OrdenVenta));
                }
            }

            await this.pedidosDao.UpdateUserOrders(salesOrdersToUpdate);
            await this.pedidosDao.InsertOrderLog(logs);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// Finish fabrication orders.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</para
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CloseFabOrders(List<OrderIdModel> finishOrders)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();
            var affectedSalesOrderIds = new List<KeyValuePair<string, string>>();

            foreach (var orderToFinish in finishOrders)
            {
                var productionOrderId = orderToFinish.OrderId;
                var ids = new List<string> { productionOrderId.ToString() };
                var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder(ids)).FirstOrDefault();

                if (productionOrder == null)
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderNotExists, productionOrderId);
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, message));
                    continue;
                }

                // Validate finished production orders
                if (productionOrder.Status.Equals(ServiceConstants.Finalizado))
                {
                    successfuly.Add(orderToFinish);
                    continue;
                }

                // Validate completed production orders
                if (!productionOrder.Status.Equals(ServiceConstants.Completed))
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderNonCompleted, productionOrderId);
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, message));
                    continue;
                }

                // Update in SAP
                var payload = new List<object> { new { OrderId = productionOrderId } };
                var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.FinishFabOrder);

                if (!result.Success)
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, ServiceConstants.ReasonSapConnectionError));
                    continue;
                }

                var resultMessages = JsonConvert.DeserializeObject<Dictionary<int, string>>(result.Response.ToString());

                // Map errors
                foreach (var error in resultMessages.Where(x => x.Key > 0))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, error.Value));
                }

                // Update production order status
                if (!resultMessages.Keys.Any(x => x.Equals(productionOrderId)))
                {
                    productionOrder.CloseUserId = orderToFinish.UserId;
                    productionOrder.CloseDate = DateTime.Now.FormatedDate();
                    productionOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { productionOrderId }, string.Format(ServiceConstants.OrderFinished, productionOrderId), ServiceConstants.OrdenFab));
                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { productionOrder });
                    affectedSalesOrderIds.Add(KeyValuePair.Create(orderToFinish.UserId, productionOrder.Salesorderid));
                    successfuly.Add(orderToFinish);
                }
            }

            // Validate affected sales order
            foreach (var salesOrderToValidate in affectedSalesOrderIds.Distinct())
            {
                var userId = salesOrderToValidate.Key;
                var salesOrderIdAsInt = int.Parse(salesOrderToValidate.Value);
                var ids = new List<string> { salesOrderToValidate.Value };
                var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(ids)).ToList();

                // Identify production orders
                var productionOrders = relatedOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList();
                if (productionOrders.All(x => x.Status.Equals(ServiceConstants.Finalizado)))
                {
                    var salesOrder = relatedOrders.Where(x => string.IsNullOrEmpty(x.Productionorderid)).First();
                    salesOrder.CloseUserId = userId;
                    salesOrder.CloseDate = DateTime.Now.FormatedDate();
                    salesOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(userId, new List<int> { salesOrderIdAsInt }, string.Format(ServiceConstants.OrderFinished, salesOrderIdAsInt), ServiceConstants.OrdenVenta));
                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { salesOrder });
                }
            }

            await this.pedidosDao.InsertOrderLog(logs);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, results, null);
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
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
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
        /// Makes the call to assign batches.
        /// </summary>
        /// <param name="assignBatches">the batches.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> UpdateBatches(List<AssignBatchModel> assignBatches)
        {
            var resultSapApi = await this.sapDiApi.PostToSapDiApi(assignBatches, ServiceConstants.UpdateBatches);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSapApi.Response.ToString());
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;
            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <summary>
        /// the signatures.
        /// </summary>
        /// <param name="signatureType">the type.</param>
        /// <param name="signatureModel">the model.</param>
        /// <returns>the value.</returns>
        public async Task<ResultModel> UpdateOrderSignature(SignatureTypeEnum signatureType, UpdateOrderSignatureModel signatureModel)
        {
            var ids = new List<string> { signatureModel.FabricationOrderId.ToString() };
            var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder(ids)).FirstOrDefault();

            if (productionOrder != null)
            {
                var orderSignatures = await this.pedidosDao.GetSignaturesByUserOrderId(productionOrder.Id);
                var isNew = false;
                if (orderSignatures == null)
                {
                    orderSignatures = new UserOrderSignatureModel();
                    orderSignatures.UserOrderId = productionOrder.Id;
                    isNew = true;
                }

                // Convert Base64 Encoded string to Byte Array.
                byte[] newSignatureAsByte = Convert.FromBase64String(signatureModel.Signature);

                switch (signatureType)
                {
                    case SignatureTypeEnum.LOGISTICS:
                        orderSignatures.LogisticSignature = newSignatureAsByte;
                        break;
                    case SignatureTypeEnum.TECHNICAL:
                        orderSignatures.TechnicalSignature = newSignatureAsByte;
                        break;
                    case SignatureTypeEnum.QFB:
                        orderSignatures.QfbSignature = newSignatureAsByte;
                        break;
                }

                if (isNew)
                {
                    await this.pedidosDao.InsertOrderSignatures(orderSignatures);
                }
                else
                {
                    await this.pedidosDao.SaveOrderSignatures(orderSignatures);
                }

                return ServiceUtils.CreateResult(true, 200, null, orderSignatures, null);
            }

            return ServiceUtils.CreateResult(true, 200, ServiceConstants.ReasonNotExistsOrder, null, null);
        }

        /// <summary>
        /// Get production order signatures.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultModel> GetOrderSignatures(int productionOrderId)
        {
            var ids = new List<string> { productionOrderId.ToString() };
            var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder(ids)).FirstOrDefault();

            if (productionOrder != null)
            {
                var orderSignatures = await this.pedidosDao.GetSignaturesByUserOrderId(productionOrder.Id);

                if (orderSignatures == null)
                {
                    orderSignatures = new UserOrderSignatureModel();
                    orderSignatures.UserOrderId = productionOrder.Id;
                }

                return ServiceUtils.CreateResult(true, 200, null, orderSignatures, null);
            }

            return ServiceUtils.CreateResult(true, 200, ServiceConstants.ReasonNotExistsOrder, null, null);
        }

        /// <summary>
        /// Finish the order by the QFB.
        /// </summary>
        /// <param name="updateOrderSignature">the model.</param>
        /// <returns>the result.</returns>
        public async Task<ResultModel> FinishOrder(FinishOrderModel updateOrderSignature)
        {
            var sapAdapterResponse = await this.sapAdapter.GetSapAdapter(string.Format(ServiceConstants.GetComponentsWithBatches, updateOrderSignature.FabricationOrderId));
            var components = JsonConvert.DeserializeObject<List<BatchesComponentModel>>(sapAdapterResponse.Response.ToString());

            if (components.Any(x => !x.LotesAsignados.Any()))
            {
                throw new CustomServiceException(ServiceConstants.BatchesAreMissingError, System.Net.HttpStatusCode.BadRequest);
            }

            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(new List<string> { updateOrderSignature.FabricationOrderId.ToString() })).FirstOrDefault();
            orders = orders == null ? new UserOrderModel() : orders;

            var allOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { orders.Salesorderid })).ToList();
            var orderSignature = await this.pedidosDao.GetSignaturesByUserOrderId(orders.Id);
            var newQfbSignatureAsByte = Convert.FromBase64String(updateOrderSignature.QfbSignature);
            var newTechSignatureAsByte = Convert.FromBase64String(updateOrderSignature.TechnicalSignature);

            if (orderSignature == null)
            {
                var newSignature = new UserOrderSignatureModel
                {
                    TechnicalSignature = newTechSignatureAsByte,
                    QfbSignature = newQfbSignatureAsByte,
                    UserOrderId = orders.Id,
                };

                await this.pedidosDao.InsertOrderSignatures(newSignature);
            }
            else
            {
                orderSignature.TechnicalSignature = newTechSignatureAsByte;
                orderSignature.QfbSignature = newQfbSignatureAsByte;
                await this.pedidosDao.SaveOrderSignatures(orderSignature);
            }

            orders.FinishDate = DateTime.Now.ToString("dd/MM/yyyy");
            orders.Status = ServiceConstants.Terminado;

            var listToUpdate = new List<UserOrderModel> { orders };

            var saleOrder = allOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid));
            saleOrder.Status = allOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid != orders.Productionorderid).Any(y => y.Status != ServiceConstants.Terminado) ? saleOrder.Status : ServiceConstants.Terminado;

            listToUpdate.Add(saleOrder);
            await this.pedidosDao.UpdateUserOrders(listToUpdate);

            var orderLogs = ServiceUtils.CreateOrderLog(updateOrderSignature.UserId, new List<int> { updateOrderSignature.FabricationOrderId }, $"{ServiceConstants.OrdenTerminada} {updateOrderSignature.UserId}", ServiceConstants.OrdenFab);
            await this.pedidosDao.InsertOrderLog(orderLogs);

            return ServiceUtils.CreateResult(true, 200, null, updateOrderSignature, null);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolateFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderModel isolateFabOrder)
        {
            var logs = new List<OrderLogModel>();
            var payload = new { ProductCode = isolateFabOrder.ProductCode };
            var diapiResult = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.CreateIsolatedFabOrder);

            if (!diapiResult.Success)
            {
                return ServiceUtils.CreateResult(true, 200, ServiceConstants.ReasonSapConnectionError, null, null);
            }

            var resultMessage = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(diapiResult.Response.ToString());
            var productionOrderId = 0;

            if (!string.IsNullOrEmpty(resultMessage.Key))
            {
                // Get new production order id
                var route = $"{ServiceConstants.GetLastIsolatedProductionOrderId}?productId={isolateFabOrder.ProductCode}&uniqueId={resultMessage.Key}";
                var result = await this.sapAdapter.GetSapAdapter(route);
                productionOrderId = int.Parse(result.Response.ToString());

                UserOrderModel newProductionOrder = new UserOrderModel();
                newProductionOrder.Salesorderid = string.Empty;
                newProductionOrder.Productionorderid = productionOrderId.ToString();
                newProductionOrder.CreatorUserId = isolateFabOrder.UserId;
                newProductionOrder.CreationDate = DateTime.Now.FormatedLargeDate();
                newProductionOrder.Status = ServiceConstants.Planificado;

                logs.AddRange(ServiceUtils.CreateOrderLog(isolateFabOrder.UserId, new List<int> { productionOrderId }, string.Format(ServiceConstants.IsolatedProductionOrderCreated, productionOrderId), ServiceConstants.OrdenFab));

                await this.pedidosDao.InsertUserOrder(new List<UserOrderModel> { newProductionOrder });
                await this.pedidosDao.InsertOrderLog(logs);
            }

            return ServiceUtils.CreateResult(true, 200, resultMessage.Value, productionOrderId, null);
        }

        /// <summary>
        /// Gets the ordersby the filter.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>the data.</returns>
        public Task<ResultModel> GetFabOrders(Dictionary<string, string> parameters)
        {
            var dateFilter = GetFabOrderUtils.GetDateFilter(parameters);

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
    }
}
