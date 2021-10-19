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
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Resources.Extensions;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.Reporting;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
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

        private readonly ISapFileService sapFileService;

        private readonly IConfiguration configuration;

        private readonly IReportingService reportingService;

        private readonly IRedisService redis;

        private readonly IKafkaConnector kafkaConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="sapFileService">The sap file service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="reporting"> The reporting service. </param>
        /// <param name="redisService">The redis Service.</param>
        /// <param name="kafkaConnector">The kafka conector.</param>
        public PedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, ISapDiApi sapDiApi, IUsersService userService, ISapFileService sapFileService, IConfiguration configuration, IReportingService reporting, IRedisService redisService, IKafkaConnector kafkaConnector)
        {
            this.sapAdapter = sapAdapter ?? throw new ArgumentNullException(nameof(sapAdapter));
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.sapDiApi = sapDiApi ?? throw new ArgumentNullException(nameof(sapDiApi));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.sapFileService = sapFileService ?? throw new ArgumentNullException(nameof(sapFileService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.reportingService = reporting ?? throw new ArgumentNullException(nameof(reporting));
            this.redis = redisService ?? throw new ArgumentNullException(nameof(redisService));
            this.kafkaConnector = kafkaConnector ?? throw new ArgumentNullException(nameof(kafkaConnector));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderBySaleOrder(listIdString);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(orders), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderByFabOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderByProducionOrder(listIdString);
            return ServiceUtils.CreateResult(true, 200, null, orders, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetFabOrderByUserId(string userId)
        {
            var userOrders = (await this.pedidosDao.GetUserOrderByUserId(new List<string> { userId })).Where(x => x.Status != ServiceConstants.Finalizado && x.Status != ServiceConstants.Almacenado).ToList();
            var resultFormula = await this.GetSapOrders(userOrders);

            var groups = ServiceUtils.GroupUserOrder(resultFormula, userOrders);
            return ServiceUtils.CreateResult(true, 200, null, groups, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrdersByUserId(List<string> listIds)
        {
            var userOrder = await this.pedidosDao.GetUserOrderByUserIdAndStatus(listIds, ServiceConstants.ListStatusOrdenesForQfbCount);
            return ServiceUtils.CreateResult(true, 200, null, userOrder, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateComponents(UpdateFormulaModel updateFormula)
        {
            var resultSapApi = await this.sapDiApi.PostToSapDiApi(updateFormula, ServiceConstants.UpdateFormula);
            if (resultSapApi.Success && !string.IsNullOrEmpty(updateFormula.Comments))
            {
                await this.UpdateFabOrderComments(updateFormula.FabOrderId, updateFormula.Comments);
            }

            var utils = new PedidosUtils(this.redis);
            var listComponents = updateFormula.Components.Where(x => x.Action == ServiceConstants.Insert).Select(y => y.ProductId).ToList();
            listComponents = listComponents.Where(x => ServiceConstants.ListComponentsMostAssigned.Any(y => x.Contains(y))).ToList();
            await utils.UpdateMostUsedComponents(listComponents);

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(resultSapApi.Response), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateStatusOrder(List<UpdateStatusOrderModel> updateStatusOrder)
        {
            var orders = updateStatusOrder.Select(x => x.OrderId.ToString()).ToList();
            var ordersList = (await this.pedidosDao.GetUserOrderByProducionOrder(orders)).ToList();
            var listOrderLogToInsert = new List<SalesLogs>();
            var listOrderLogs = new List<OrderLogModel>();

            ordersList.ForEach(x =>
            {
                var order = updateStatusOrder.FirstOrDefault(y => y.OrderId.ToString().Equals(x.Productionorderid));
                order = order ?? new UpdateStatusOrderModel();
                x.Status = order.Status ?? x.Status;
                x.Userid = order.UserId ?? x.Userid;
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(x.Userid, new List<UserOrderModel> { x }));
                listOrderLogs.AddRange(ServiceUtils.CreateOrderLog(x.Userid, new List<int> { order.OrderId }, string.Format(ServiceConstants.OrdenProceso, x.Productionorderid), ServiceConstants.OrdenFab));
            });

            await this.pedidosDao.UpdateUserOrders(ordersList);
            await this.pedidosDao.InsertOrderLog(listOrderLogs);
            if (updateStatusOrder.Any(x => x.Status == ServiceConstants.Entregado))
            {
                var saleOrderId = ordersList.FirstOrDefault().Salesorderid;
                ordersList = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { saleOrderId })).ToList();
                var allDelivered = ordersList.Where(x => x.IsProductionOrder && x.Status != ServiceConstants.Cancelled).All(y => y.Status == ServiceConstants.Entregado);
                var saleOrder = ordersList.FirstOrDefault(x => x.IsSalesOrder);
                saleOrder.Status = allDelivered ? ServiceConstants.Entregado : saleOrder.Status;
                var userId = ordersList.FirstOrDefault().Userid;
                if (allDelivered)
                {
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(userId, new List<UserOrderModel> { saleOrder }));
                }

                await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });
            }

            this.kafkaConnector.PushMessage(listOrderLogToInsert);

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(updateStatusOrder), null);
        }

        /// <inheritdoc/>
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

            fabOrderToUpdate.Comments = comments;
            await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { fabOrderToUpdate });
            return fabOrderToUpdate;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> ConnectDiApi()
        {
            var sapResponse = await this.sapDiApi.GetSapDiApi(ServiceConstants.ConnectSapDiApi);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(sapResponse.Response), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CloseSalesOrders(List<OrderIdModel> finishOrders)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();
            var listToGenPdf = new List<int>();
            var listOrderLogToInsert = new List<SalesLogs>();
            var listSaleOrdersToFinish = finishOrders.Select(x => x.OrderId).ToList();

            var (saleOrders, productionOrders) = await this.GetRelatedOrdersToSalesOrder(listSaleOrdersToFinish, ServiceConstants.Cancelled, ServiceConstants.Finalizado);

            foreach (var orderToFinish in finishOrders)
            {
                var localSalesOrder = saleOrders.FirstOrDefault(x => x.Salesorderid == orderToFinish.OrderId.ToString());
                var localProductionOrders = productionOrders.Where(x => x.Salesorderid == orderToFinish.OrderId.ToString());

                // Update in SAP
                var payload = localProductionOrders.Select(x => new { OrderId = x.Productionorderid });
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
                foreach (var userOrder in localProductionOrders)
                {
                    int prodOrderId = int.Parse(userOrder.Productionorderid);
                    if (!resultMessages.Keys.Any(x => x.Equals(prodOrderId)))
                    {
                        userOrder.CloseUserId = orderToFinish.UserId;
                        userOrder.CloseDate = DateTime.Now;
                        userOrder.Status = ServiceConstants.Finalizado;
                        userOrder.FinalizedDate = DateTime.Now;
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(orderToFinish.UserId, new List<UserOrderModel> { userOrder }));
                    }
                }

                await this.pedidosDao.UpdateUserOrders(productionOrders);

                // Update sales order status
                if (resultMessages.Keys.Any(x => x.Equals(0)))
                {
                    var previousStatusSalesOrder = localSalesOrder.Status;
                    localSalesOrder.CloseUserId = orderToFinish.UserId;
                    localSalesOrder.CloseDate = DateTime.Now;
                    localSalesOrder.Status = ServiceConstants.Finalizado;
                    localSalesOrder.FinalizedDate = DateTime.Now;

                    if (previousStatusSalesOrder != localSalesOrder.Status)
                    {
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(orderToFinish.UserId, new List<UserOrderModel> { localSalesOrder }));
                    }

                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { localSalesOrder });
                    successfuly.Add(orderToFinish);
                }

                listToGenPdf.Add(int.Parse(localSalesOrder.Salesorderid));
            }

            this.kafkaConnector.PushMessage(listOrderLogToInsert);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };

            await SendToGeneratePdfUtils.CreateModelGeneratePdf(listToGenPdf, new List<int>(), this.sapAdapter, this.pedidosDao, this.sapFileService, this.userService, true);
            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// reject order (status to reject).
        /// </summary>
        /// <param name="rejectOrders">Orders to reject.</param>
        /// <returns>Order with updated info.</returns>
        public async Task<ResultModel> RejectSalesOrders(RejectOrdersModel rejectOrders)
        {
            var ordersId = rejectOrders.OrdersId.Select(x => x.ToString()).ToList();
            var failedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(ordersId)).Where(x => x.IsSalesOrder).Select(y => y.Salesorderid).ToList();
            var succesfulyOrdersId = ordersId.Where(x => !failedOrders.Contains(x)).ToList();
            var succesfuly = new List<UserOrderModel>();
            var failed = new List<object>();
            var listOrderLogToInsert = new List<SalesLogs>();

            foreach (var orderId in failedOrders)
            {
                var orderFail = new
                {
                    reason = string.Format(ServiceConstants.OrderNotRejectedBecauseExits, orderId),
                };
                failed.Add(orderFail);
            }

            foreach (var orderToRejectedId in succesfulyOrdersId)
            {
                var userOrder = new UserOrderModel
                {
                    Salesorderid = orderToRejectedId,
                    Status = ServiceConstants.Rechazado,
                    Comments = rejectOrders.Comments,
                };
                succesfuly.Add(userOrder);
                /* logs */
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(rejectOrders.UserId, new List<UserOrderModel> { userOrder }));
            }

            await this.pedidosDao.InsertUserOrder(succesfuly);
            this.kafkaConnector.PushMessage(listOrderLogToInsert);

            var resultAsesors = await this.sapAdapter.PostSapAdapter(succesfuly.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList(), ServiceConstants.GetAsesorsMail);
            var resultAsesorEmail = JsonConvert.DeserializeObject<List<AsesorModel>>(JsonConvert.SerializeObject(resultAsesors.Response));
            var asesorsToReportingEmail = new List<object>();

            foreach (var asesor in resultAsesorEmail)
            {
                asesorsToReportingEmail.Add(new
                {
                    customerName = asesor.Cliente,
                    destinyEmail = "tania.dominguez@axity.com", // asesor.Email,
                    salesOrders = asesor.OrderId.ToString(),
                    comments = rejectOrders.Comments,
                });
            }

            // send Emails
            this.reportingService.PostReportingService(new { rejectedOrder = asesorsToReportingEmail }, ServiceConstants.SendEmailToRejectedOrders);
            var results = new
            {
                success = succesfuly.Select(x => new { OrderId = x.Salesorderid }).Distinct(),
                failed = failed.Distinct(),
            };

            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// reject order (status to reject).
        /// </summary>
        /// <param name="status">status.</param>}
        /// <param name="userId">userId.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetQfbOrdersByStatus(string status, string userId)
        {
            var userOrdersFab = (await this.pedidosDao.GetUserOrderByUserId(new List<string> { userId })).Where(x => x.Status == status && x.IsProductionOrder).ToList();
            var ordersFabIds = userOrdersFab.Select(x => x.Productionorderid).ToList();
            return ServiceUtils.CreateResult(true, 200, null, ordersFabIds, null);
        }

        /// <summary>
        /// Finish fabrication orders.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultModel> CloseFabOrders(List<CloseProductionOrderModel> finishOrders)
        {
            var logs = new List<OrderLogModel>();
            var successfuly = new List<object>();
            var failed = new List<object>();
            var affectedSalesOrderIds = new List<KeyValuePair<string, string>>();
            var listIsolated = new List<int>();
            var listSalesOrder = new List<int>();
            var listOrderLogToInsert = new List<SalesLogs>();

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

                if (productionOrder.IsIsolatedProductionOrder)
                {
                    listIsolated.Add(int.Parse(productionOrder.Productionorderid));
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
                    productionOrder.CloseDate = DateTime.Now;
                    productionOrder.Status = ServiceConstants.Finalizado;
                    productionOrder.FinalizedDate = DateTime.Now;

                    var batch = orderToFinish.Batches != null && orderToFinish.Batches.Any() ? orderToFinish.Batches.FirstOrDefault() : new BatchesConfigurationModel { BatchCode = string.Empty };
                    productionOrder.BatchFinalized = batch.BatchCode;

                    /* logs */
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(orderToFinish.UserId, new List<UserOrderModel> { productionOrder }));
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
                listSalesOrder.Add(int.Parse(salesOrder.Salesorderid));

                if (productionOrders.All(x => ServiceConstants.ValidStatusFinalizar.Contains(x.Status)) && !preProductionOrders.Any())
                {
                    salesOrder.CloseUserId = userId;
                    salesOrder.CloseDate = DateTime.Now;
                    salesOrder.Status = ServiceConstants.Finalizado;
                    salesOrder.FinalizedDate = DateTime.Now;
                    /* logs */
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(userId, new List<UserOrderModel> { salesOrder }));
                    logs.AddRange(ServiceUtils.CreateOrderLog(userId, new List<int> { salesOrderIdAsInt }, string.Format(ServiceConstants.OrderFinished, salesOrderIdAsInt), ServiceConstants.OrdenVenta));
                    await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { salesOrder });
                }
            }

            await SendToGeneratePdfUtils.CreateModelGeneratePdf(listSalesOrder, listIsolated, this.sapAdapter, this.pedidosDao, this.sapFileService, this.userService, true);

            await this.pedidosDao.InsertOrderLog(logs);
            this.kafkaConnector.PushMessage(listOrderLogToInsert);

            var results = new
            {
                success = successfuly.Distinct(),
                failed = failed.Distinct(),
            };
            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateBatches(List<AssignBatchModel> assignBatches)
        {
            var resultSapApi = await this.sapDiApi.PostToSapDiApi(assignBatches, ServiceConstants.UpdateBatches);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSapApi.Response.ToString());
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(assignBatches.Select(x => x.OrderId.ToString()).ToList())).ToList();
            orders.ForEach(x => x.AreBatchesComplete = assignBatches.FirstOrDefault().AreBatchesComplete);
            await this.pedidosDao.UpdateUserOrders(orders);

            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateOrderSignature(SignatureType signatureType, UpdateOrderSignatureModel signatureModel)
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
                    case SignatureType.LOGISTICS:
                        orderSignatures.LogisticSignature = newSignatureAsByte;
                        break;
                    case SignatureType.TECHNICAL:
                        orderSignatures.TechnicalSignature = newSignatureAsByte;
                        break;
                    case SignatureType.QFB:
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<ResultModel> FinishOrder(FinishOrderModel updateOrderSignature)
        {
            var listProductionOrders = updateOrderSignature.FabricationOrderId.Select(ts => ts.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(listProductionOrders)).ToList();

            var userModelIds = orders.Select(x => x.Id).Distinct().ToList();
            var orderSignatures = (await this.pedidosDao.GetSignaturesByUserOrderId(userModelIds)).ToList();

            var newQfbSignatureAsByte = Convert.FromBase64String(updateOrderSignature.QfbSignature);
            var newTechSignatureAsByte = Convert.FromBase64String(updateOrderSignature.TechnicalSignature);

            var listSignatureToInsert = new List<UserOrderSignatureModel>();
            var listToUpdate = new List<UserOrderSignatureModel>();
            var listOrderLogToInsert = new List<SalesLogs>();
            orders.ForEach(o =>
            {
                var signature = orderSignatures.FirstOrDefault(x => x.UserOrderId == o.Id);

                if (signature == null)
                {
                    listSignatureToInsert.Add(new UserOrderSignatureModel
                    {
                        TechnicalSignature = newTechSignatureAsByte,
                        QfbSignature = newQfbSignatureAsByte,
                        UserOrderId = o.Id,
                    });
                }
                else
                {
                    signature.TechnicalSignature = newTechSignatureAsByte;
                    signature.QfbSignature = newQfbSignatureAsByte;
                    listToUpdate.Add(signature);
                }

                o.FinishDate = DateTime.Now;
                o.Status = ServiceConstants.Terminado;
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(updateOrderSignature.UserId, new List<UserOrderModel> { o }));
            });

            await this.pedidosDao.InsertOrderSignatures(listSignatureToInsert);
            await this.pedidosDao.SaveOrderSignatures(listToUpdate);
            var listorderToUpdate = new List<UserOrderModel>(orders);

            if (orders.Any(x => !string.IsNullOrEmpty(x.Salesorderid)))
            {
                var saleOrders = orders.Where(x => !string.IsNullOrEmpty(x.Salesorderid)).Select(y => y.Salesorderid).Distinct().ToList();
                var allOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(saleOrders)).ToList();

                var saleOrder = allOrders.Where(x => x.IsSalesOrder).ToList();
                var saleIds = saleOrder.Select(y => int.Parse(y.Salesorderid)).ToList();
                var preProdOrderSap = await ServiceUtils.GetSalesOrdersFromSap(saleIds, this.sapAdapter);

                saleOrder.ForEach(sale =>
                {
                    var orderBySale = allOrders.Where(x => x.Salesorderid == sale.Salesorderid).ToList();
                    var areInvalidOrders = orderBySale.Any(x => x.IsProductionOrder && !listProductionOrders.Contains(x.Productionorderid) && !ServiceConstants.ValidStatusTerminar.Contains(x.Status));
                    var tupleValues = preProdOrderSap.FirstOrDefault(x => x.Order.DocNum == int.Parse(sale.Salesorderid));
                    tupleValues ??= new OrderWithDetailModel { Detalle = new List<CompleteDetailOrderModel>() };
                    var previousStatus = sale.Status;
                    sale.Status = areInvalidOrders || tupleValues.Detalle.Any(x => string.IsNullOrEmpty(x.Status)) ? sale.Status : ServiceConstants.Terminado;
                    /** add logs**/
                    if (previousStatus != sale.Status)
                    {
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(updateOrderSignature.UserId, new List<UserOrderModel> { sale }));
                    }

                    listorderToUpdate.Add(sale);
                });
            }

            await this.pedidosDao.UpdateUserOrders(listorderToUpdate);
            this.kafkaConnector.PushMessage(listOrderLogToInsert);
            return ServiceUtils.CreateResult(true, 200, null, updateOrderSignature, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderModel isolatedFabOrder)
        {
            var logs = new List<OrderLogModel>();
            var listOrderLogToInsert = new List<SalesLogs>();
            var payload = new { isolatedFabOrder.ProductCode };
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
                var route = $"{ServiceConstants.GetLastIsolatedProductionOrderId}?productId={isolatedFabOrder.ProductCode}&uniqueId={resultMessage.Key}";
                var result = await this.sapAdapter.GetSapAdapter(route);
                productionOrderId = int.Parse(result.Response.ToString() ?? throw new InvalidOperationException());

                UserOrderModel newProductionOrder = new UserOrderModel();
                newProductionOrder.Salesorderid = string.Empty;
                newProductionOrder.Productionorderid = productionOrderId.ToString();
                newProductionOrder.CreatorUserId = isolatedFabOrder.UserId;
                newProductionOrder.CreationDate = DateTime.Now.FormatedLargeDate();
                newProductionOrder.Status = ServiceConstants.Planificado;
                newProductionOrder.PlanningDate = DateTime.Now;

                logs.AddRange(ServiceUtils.CreateOrderLog(isolatedFabOrder.UserId, new List<int> { productionOrderId }, string.Format(ServiceConstants.IsolatedProductionOrderCreated, productionOrderId), ServiceConstants.OrdenFab));
                /** add logs**/
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(isolatedFabOrder.UserId, new List<UserOrderModel> { newProductionOrder }));
                await this.pedidosDao.InsertUserOrder(new List<UserOrderModel> { newProductionOrder });
                await this.pedidosDao.InsertOrderLog(logs);
                this.kafkaConnector.PushMessage(listOrderLogToInsert);
            }

            return ServiceUtils.CreateResult(true, 200, resultMessage.Value, productionOrderId, null);
        }

        /// <inheritdoc/>
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

            var userResponse = await this.userService.PostSimpleUsers(usersId, ServiceConstants.GetUsersById);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());

            var orderToReturn = GetFabOrderUtils.CreateModels(sapOrders, userOrders, users).OrderBy(o => o.DocNum).ToList();
            orderToReturn = orderToReturn.OrderBy(x => x.FabOrderId).ToList();
            var total = sapResponse.Comments == null ? "0" : sapResponse.Comments.ToString();
            return ServiceUtils.CreateResult(true, 200, null, orderToReturn, null, total);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CompletedBatches(int orderId)
        {
            var sapAdapterResponse = await this.sapAdapter.GetSapAdapter(string.Format(ServiceConstants.GetComponentsWithBatches, orderId));
            var components = JsonConvert.DeserializeObject<List<BatchesComponentModel>>(sapAdapterResponse.Response.ToString());

            if (components.Any(x => !x.LotesAsignados.Any() || x.TotalNecesario > 0))
            {
                throw new CustomServiceException(ServiceConstants.BatchesAreMissingError, System.Net.HttpStatusCode.BadRequest);
            }

            return ServiceUtils.CreateResult(true, 200, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PrintOrders(List<int> ordersId)
        {
            var result = await await SendToGeneratePdfUtils.CreateModelGeneratePdf(ordersId, new List<int>(), this.sapAdapter, this.pedidosDao, this.sapFileService, this.userService, false);
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Response.ToString());

            var listUrls = ServiceUtils.GetValuesByContainsKeyValue(dictResult, ServiceConstants.Ok.ToUpper());
            var listRoutes = new List<string>();
            listUrls.ForEach(x =>
            {
                var baseRoute = this.configuration["OmicronFilesAddress"];

                var pathArray = x.Split(@"\").Where(x => x.ToUpper() != "C:").ToList();
                var completePath = new StringBuilder();
                completePath.Append(baseRoute);
                pathArray.ForEach(x => completePath.Append($"{x}/"));
                var path = completePath.ToString().Remove(completePath.ToString().Length - 1);
                listRoutes.Add(path);
            });

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreatePdf);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listWithError.Any() ? ServiceConstants.ErrorCrearPdf : null;
            listErrorId = listErrorId.Any() ? listErrorId : listRoutes;
            return ServiceUtils.CreateResult(true, 200, userError, listErrorId, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateSaleOrders(UpdateOrderCommentsModel updateOrder)
        {
            var saleOrders = await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { updateOrder.OrderId.ToString() });
            var saleOrder = saleOrders.FirstOrDefault(x => x.IsSalesOrder);
            saleOrder.Comments = updateOrder.Comments;
            await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });
            return ServiceUtils.CreateResult(true, 200, null, updateOrder, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateDesignerLabel(UpdateDesignerLabelModel updateDesignerLabels)
        {
            var ordersIdString = updateDesignerLabels.Details.Select(x => x.OrderId.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(ordersIdString)).ToList();
            var signatureOrders = (await this.pedidosDao.GetSignaturesByUserOrderId(orders.Select(x => x.Id).ToList())).ToList();

            var dataReturned = this.GetModelsToUpdate(orders, signatureOrders, updateDesignerLabels);
            orders = dataReturned.Item1;
            var listNewSignatures = dataReturned.Item2;
            var listToUpdate = dataReturned.Item3;

            await this.pedidosDao.InsertOrderSignatures(listNewSignatures);
            await this.pedidosDao.SaveOrderSignatures(listToUpdate);
            await this.pedidosDao.UpdateUserOrders(orders);

            var saleOrderId = orders.FirstOrDefault().Salesorderid;
            orders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { saleOrderId })).ToList();

            var saleOrder = orders.FirstOrDefault(x => x.IsSalesOrder);
            var allChecked = orders.Where(x => x.IsProductionOrder && x.Status != ServiceConstants.Cancelled).All(y => y.FinishedLabel == 1);
            saleOrder.FinishedLabel = allChecked ? 1 : 0;
            saleOrder.FinalizedDate = allChecked ? DateTime.Now : saleOrder.FinalizedDate;

            await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });

            return ServiceUtils.CreateResult(true, 200, null, updateDesignerLabels, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateSaleOrderPdf(List<int> ordersId)
        {
            var listRoutes = new List<string>();
            var responseFile = await this.sapFileService.PostSimple(ordersId, ServiceConstants.CreateSalePdf);

            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseFile.Response.ToString());
            ServiceUtils.GetValuesContains(dictResult, ServiceConstants.Ok)
            .ForEach(x =>
            {
                var targetPath = dictResult[x].Replace("Ok-", string.Empty);
                var baseRoute = this.configuration["OmicronFilesAddress"];

                var pathArray = targetPath.Split(@"\").Where(x => x.ToUpper() != "C:").ToList();
                var completePath = new StringBuilder();
                completePath.Append(baseRoute);
                pathArray.ForEach(x => completePath.Append($"{x}/"));
                var path = completePath.ToString().Remove(completePath.ToString().Length - 1);
                listRoutes.Add(path);
            });

            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorCreatePdf);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);

            return ServiceUtils.CreateResult(true, 200, JsonConvert.SerializeObject(listErrorId), listRoutes, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> DeleteFiles()
        {
            var response = await this.sapFileService.PostSimple(null, ServiceConstants.DeleteFiles);
            return ServiceUtils.CreateResult(true, 200, null, response, null, null);
        }

        /// <summary>
        /// Gets the order updated and the signatures to insert or update.
        /// </summary>
        /// <param name="orders">the orders.</param>
        /// <param name="signatureOrders">the signatues.</param>
        /// <param name="updateDesignerLabels">the data to insert.</param>
        /// <returns>the values.</returns>
        private Tuple<List<UserOrderModel>, List<UserOrderSignatureModel>, List<UserOrderSignatureModel>> GetModelsToUpdate(List<UserOrderModel> orders, List<UserOrderSignatureModel> signatureOrders, UpdateDesignerLabelModel updateDesignerLabels)
        {
            var listNewSignatures = new List<UserOrderSignatureModel>();
            var listToUpdate = new List<UserOrderSignatureModel>();
            var signature = updateDesignerLabels.DesignerSignature != null ? Convert.FromBase64String(updateDesignerLabels.DesignerSignature) : new byte[0];

            foreach (var x in orders)
            {
                var orderToUpdate = updateDesignerLabels.Details.FirstOrDefault(y => y.OrderId.ToString() == x.Productionorderid);
                var orderSignatureToUpdate = signatureOrders.FirstOrDefault(y => y.UserOrderId == x.Id);
                x.FinishedLabel = orderToUpdate.Checked ? 1 : 0;
                x.FinalizedDate = orderToUpdate.Checked ? DateTime.Now : x.FinalizedDate;

                if (orderSignatureToUpdate == null && orderToUpdate.Checked)
                {
                    var newSignature = new UserOrderSignatureModel
                    {
                        DesignerSignature = signature,
                        UserOrderId = x.Id,
                        DesignerId = updateDesignerLabels.UserId,
                    };

                    listNewSignatures.Add(newSignature);
                }
                else if (orderSignatureToUpdate != null && orderToUpdate.Checked)
                {
                    orderSignatureToUpdate.DesignerSignature = signature;
                    orderSignatureToUpdate.DesignerId = updateDesignerLabels.UserId;
                    listToUpdate.Add(orderSignatureToUpdate);
                }
            }

            return new Tuple<List<UserOrderModel>, List<UserOrderSignatureModel>, List<UserOrderSignatureModel>>(orders, listNewSignatures, listToUpdate);
        }

        /// <summary>
        /// gets the order from sap.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <returns>tje data.</returns>
        private async Task<List<CompleteFormulaWithDetalle>> GetSapOrders(List<UserOrderModel> userOrders)
        {
            var resultFormula = new List<CompleteFormulaWithDetalle>();
            var listsOfData = ServiceUtils.GetGroupsOfList(userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList(), 20);

            await Task.WhenAll(listsOfData.Select(async x =>
            {
                var route = $"{ServiceConstants.GetFormula}";
                var listIds = x.Select(y => int.Parse(y.Productionorderid)).ToList();
                var result = await this.sapAdapter.PostSapAdapter(listIds, route);

                lock (resultFormula)
                {
                    var formula = JsonConvert.DeserializeObject<List<CompleteFormulaWithDetalle>>(JsonConvert.SerializeObject(result.Response));
                    resultFormula.AddRange(formula);
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

        private async Task<(List<UserOrderModel> salesOrder, List<UserOrderModel> productionOrders)> GetRelatedOrdersToSalesOrder(List<int> salesOrderId, params string[] ignoredProductionOrderStatus)
        {
            var relatedOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(salesOrderId.Select(x => x.ToString()).ToList())).ToList();
            var productionOrders = relatedOrders.Where(x => x.IsProductionOrder).Where(x => !ignoredProductionOrderStatus.Contains(x.Status));

            return (relatedOrders.Where(x => x.IsSalesOrder).ToList(), productionOrders.ToList());
        }
    }
}