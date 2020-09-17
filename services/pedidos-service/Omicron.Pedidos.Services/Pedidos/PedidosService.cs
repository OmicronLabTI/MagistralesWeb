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
                var (salesOrder, productionOrders) = await this.GetRelatedOrdersToSalesOrder(orderToFinish.OrderId, ServiceConstants.Cancelled, ServiceConstants.Finalizado);

                if (!salesOrder.Status.Equals(ServiceConstants.Completed))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, ServiceConstants.ReasonOrderNonCompleted));
                    continue;
                }

                var salesOrderId = int.Parse(salesOrder.Salesorderid);

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

                // Validate with SAP pre-production orders.
                if ((await ServiceUtils.GetPreProductionOrdersFromSap(salesOrder, this.sapAdapter)).Any())
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderToFinish, ServiceConstants.ReasonPreProductionOrdersInSap));
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
        /// Finish fabrication orders.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</para
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CloseFabOrders(List<CloseProductionOrderModel> finishOrders)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();
            var affectedSalesOrderIds = new List<KeyValuePair<string, string>>();

            foreach (var orderToFinish in finishOrders)
            {
                var orderIdModel = new OrderIdModel { UserId = orderToFinish.UserId, OrderId = orderToFinish.OrderId };
                var productionOrderId = orderToFinish.OrderId;
                var ids = new List<string> { productionOrderId.ToString() };
                var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder(ids)).FirstOrDefault();

                if (productionOrder == null)
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderNotExists, productionOrderId);
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, message));
                    continue;
                }

                // Validate finished production orders
                if (productionOrder.Status.Equals(ServiceConstants.Finalizado))
                {
                    successfuly.Add(orderIdModel);
                    continue;
                }

                // Validate completed production orders
                if (!productionOrder.Status.Equals(ServiceConstants.Completed))
                {
                    var message = string.Format(ServiceConstants.ReasonProductionOrderNonCompleted, productionOrderId);
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, message));
                    continue;
                }

                // Update in SAP
                var payload = new List<object> { orderToFinish };
                var result = await this.sapDiApi.PostToSapDiApi(payload, ServiceConstants.FinishFabOrder);

                if (!result.Success)
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, ServiceConstants.ReasonSapConnectionError));
                    continue;
                }

                var resultMessages = JsonConvert.DeserializeObject<Dictionary<int, string>>(result.Response.ToString());

                // Map errors
                foreach (var error in resultMessages.Where(x => x.Key > 0))
                {
                    failed.Add(ServiceUtils.CreateCancellationFail(orderIdModel, error.Value));
                }

                // Update production order status
                if (!resultMessages.Keys.Any(x => x.Equals(productionOrderId)))
                {
                    productionOrder.CloseUserId = orderToFinish.UserId;
                    productionOrder.CloseDate = DateTime.Now.FormatedDate();
                    productionOrder.Status = ServiceConstants.Finalizado;

                    logs.AddRange(ServiceUtils.CreateOrderLog(orderToFinish.UserId, new List<int> { productionOrderId }, string.Format(ServiceConstants.OrderFinished, productionOrderId), ServiceConstants.OrdenFab));
                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { productionOrder });
                    successfuly.Add(orderIdModel);

                    if (!productionOrder.IsIsolatedProductionOrder)
                    {
                        affectedSalesOrderIds.Add(KeyValuePair.Create(orderToFinish.UserId, productionOrder.Salesorderid));
                    }
                }
            }

            // Validate affected sales order
            foreach (var salesOrderToValidate in affectedSalesOrderIds.Distinct())
            {
                var userId = salesOrderToValidate.Key;
                var salesOrderIdAsInt = int.Parse(salesOrderToValidate.Value);

                var (salesOrder, productionOrders) = await this.GetRelatedOrdersToSalesOrder(salesOrderIdAsInt, ServiceConstants.Cancelled);
                var preProductionOrders = await ServiceUtils.GetPreProductionOrdersFromSap(salesOrder, this.sapAdapter);

                if (productionOrders.All(x => x.Status.Equals(ServiceConstants.Finalizado)) && !preProductionOrders.Any())
                {
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
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(new List<string> { updateOrderSignature.FabricationOrderId.ToString() })).FirstOrDefault();
            orders = orders == null ? new UserOrderModel() : orders;

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

            if (!string.IsNullOrEmpty(orders.Salesorderid))
            {
                var allOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { orders.Salesorderid })).ToList();
                var saleOrder = allOrders.FirstOrDefault(x => string.IsNullOrEmpty(x.Productionorderid));
                var areInvalidOrders = allOrders.Any(x => !string.IsNullOrEmpty(x.Productionorderid) && x.Productionorderid != orders.Productionorderid && !ServiceConstants.ValidStatusTerminar.Contains(x.Status));
                var preProdOrderSap = await ServiceUtils.GetPreProductionOrdersFromSap(saleOrder, this.sapAdapter);

                saleOrder.Status = areInvalidOrders || preProdOrderSap.Any() ? saleOrder.Status : ServiceConstants.Terminado;
                listToUpdate.Add(saleOrder);
            }

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
        public async Task<ResultModel> GetFabOrders(Dictionary<string, string> parameters)
        {
            var localFilterOrders = await GetFabOrderUtils.GetOrdersByFilter(parameters, this.pedidosDao);
            var ordersId = localFilterOrders.Where(y => !string.IsNullOrEmpty(y.Productionorderid)).Select(x => int.Parse(x.Productionorderid)).Distinct().ToList();
            parameters.Add(ServiceConstants.NeedsLargeDsc, ServiceConstants.NeedsLargeDsc);

            var sapResponse = await this.sapAdapter.PostSapAdapter(new GetOrderFabModel { Filters = parameters, OrdersId = ordersId }, ServiceConstants.GetFabOrdersByFilter);
            var sapOrders = JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(sapResponse.Response.ToString());

            if (!sapOrders.Any())
            {
                return ServiceUtils.CreateResult(true, 200, null, new List<FabricacionOrderModel>(), null);
            }

            var sapOrdersId = sapOrders.Select(x => x.OrdenId.ToString()).ToList();
            var userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(sapOrdersId)).ToList();
            var usersId = userOrders.Select(x => x.Userid).ToList();

            var userService = await this.userService.PostSimpleUsers(usersId, ServiceConstants.GetUsersById);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userService.Response.ToString());

            var orderToReturn = GetFabOrderUtils.CreateModels(sapOrders, userOrders, users).OrderBy(o => o.DocNum).ToList();
            orderToReturn = orderToReturn.OrderBy(x => x.FabOrderId).ToList();
            var total = sapResponse.Comments == null ? "0" : sapResponse.Comments.ToString();
            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, total);
        }

        /// <summary>
        /// Gets the completed batch.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> CompletedBatches(int orderId)
        {
            var sapAdapterResponse = await this.sapAdapter.GetSapAdapter(string.Format(ServiceConstants.GetComponentsWithBatches, orderId));
            var components = JsonConvert.DeserializeObject<List<BatchesComponentModel>>(sapAdapterResponse.Response.ToString());

            if (components.Any(x => !x.LotesAsignados.Any()))
            {
                throw new CustomServiceException(ServiceConstants.BatchesAreMissingError, System.Net.HttpStatusCode.BadRequest);
            }

            return ServiceUtils.CreateResult(true, 200, null, null, null, null);
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
        /// Get related orders to sales order.
        /// </summary>
        /// <param name="salesOrderId">Sales order id.</param>
        /// <param name="ignoredProductionOrderStatus">Status to ignore in production orders.</param>
        /// <returns>Sales order, production orders.</returns>
        private async Task<(UserOrderModel salesOrder, List<UserOrderModel> productionOrders)> GetRelatedOrdersToSalesOrder(int salesOrderId, params string[] ignoredProductionOrderStatus)
        {
            var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { salesOrderId.ToString() })).ToList();
            var productionOrders = relatedOrders.Where(x => x.IsProductionOrder).Where(x => !ignoredProductionOrderStatus.Contains(x.Status));

            return (relatedOrders.FirstOrDefault(x => x.IsSalesOrder), productionOrders.ToList());
        }
    }
}