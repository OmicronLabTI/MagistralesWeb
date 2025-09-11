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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Enums;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Resources.Exceptions;
    using Omicron.Pedidos.Resources.Extensions;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.ProductionOrders;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.Reporting;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// the pedidos service.
    /// </summary>
    public class PedidosService : IPedidosService
    {
        private readonly ISapAdapter sapAdapter;

        private readonly IPedidosDao pedidosDao;

        private readonly IUsersService userService;

        private readonly ISapFileService sapFileService;

        private readonly IConfiguration configuration;

        private readonly IReportingService reportingService;

        private readonly IRedisService redis;

        private readonly IKafkaConnector kafkaConnector;

        private readonly ISapServiceLayerAdapterService serviceLayerAdapterService;

        private readonly ISapDiApi sapDiApi;

        private readonly IProductionOrdersService productionOrdersService;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosService"/> class.
        /// </summary>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">pedidos dao.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="sapFileService">The sap file service.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="reporting"> The reporting service. </param>
        /// <param name="redisService">The redis Service.</param>
        /// <param name="kafkaConnector">The kafka conector.</param>
        /// <param name="sapServiceLayerAdapterService">The sapServiceLayerAdapterService.</param>
        /// <param name="productionOrdersService">The productionOrdersService.</param>
        /// <param name="sapDiApi">the sapdiapi.</param>
        /// <param name="logger">the logger.</param>
        public PedidosService(ISapAdapter sapAdapter, IPedidosDao pedidosDao, IUsersService userService, ISapFileService sapFileService, IConfiguration configuration, IReportingService reporting, IRedisService redisService, IKafkaConnector kafkaConnector, ISapServiceLayerAdapterService sapServiceLayerAdapterService, ISapDiApi sapDiApi, IProductionOrdersService productionOrdersService, ILogger logger)
        {
            this.sapAdapter = sapAdapter.ThrowIfNull(nameof(sapAdapter));
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.userService = userService.ThrowIfNull(nameof(userService));
            this.sapFileService = sapFileService.ThrowIfNull(nameof(sapFileService));
            this.configuration = configuration.ThrowIfNull(nameof(configuration));
            this.reportingService = reporting.ThrowIfNull(nameof(reporting));
            this.redis = redisService.ThrowIfNull(nameof(redisService));
            this.kafkaConnector = kafkaConnector.ThrowIfNull(nameof(kafkaConnector));
            this.sapDiApi = sapDiApi.ThrowIfNull(nameof(sapDiApi));
            this.serviceLayerAdapterService = sapServiceLayerAdapterService.ThrowIfNull(nameof(sapServiceLayerAdapterService));
            this.productionOrdersService = productionOrdersService.ThrowIfNull(nameof(productionOrdersService));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderBySaleOrder(listIdString);
            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(orders), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderBySalesOrderWithDetail(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderBySaleOrder(listIdString);

            var productionOrders = orders.Where(x => x.Productionorderid != null).Select(x => int.Parse(x.Productionorderid)).ToList();
            var ordersParent = await this.pedidosDao.GetProductionOrderSeparationByOrderId(productionOrders);
            var listToReturn = new UserOrderSeparationModel
            {
                UserOrders = orders.ToList(),
                ProductionOrderSeparations = ordersParent,
            };
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrderByFabOrder(List<int> listIds)
        {
            var listIdString = listIds.Select(x => x.ToString()).ToList();
            var orders = await this.pedidosDao.GetUserOrderByProducionOrder(listIdString);

            var ordersParent = await this.pedidosDao.GetProductionOrderSeparationByOrderId(listIds);

            var existsProccesWithError = await this.pedidosDao.GetProductionOrderSeparationDetailLogByParentOrderId(listIds.FirstOrDefault());

            var listToReturn = new UserOrderSeparationModel
            {
                UserOrders = orders.ToList(),
                ProductionOrderSeparations = ordersParent,
                OnSplitProcess = existsProccesWithError != null,
            };

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetFabOrderByUserId(string userId)
        {
            var userResponse = await this.userService.PostSimpleUsers(new List<string> { userId }, ServiceConstants.GetUsersById);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
            var userOrders = new List<UserOrderModel>();
            var isTecnic = users.First().Role.Equals(9);
            if (isTecnic)
            {
                userOrders = (await this.pedidosDao.GetUserOrderByTecnicId(new List<string> { userId }))
                    .Where(x => x.StatusForTecnic == ServiceConstants.Asignado ||
                                x.StatusForTecnic == ServiceConstants.Pendiente ||
                                x.StatusForTecnic == ServiceConstants.Reasignado)
                    .ToList();

                var usersqfb = await this.userService.PostSimpleUsers(userOrders.Select(x => x.Userid).ToList(), ServiceConstants.GetUsersById);
                var allUsersQfb = JsonConvert.DeserializeObject<List<UserModel>>(usersqfb.Response.ToString());
                UserModel user;
                foreach (var order in userOrders)
                {
                    user = allUsersQfb.Any(x => x.Id == order.Userid) ? allUsersQfb.First(x => x.Id == order.Userid) : new UserModel();
                    order.QfbName = string.Concat(user.FirstName, " ", user.LastName);
                    order.Status = order.StatusForTecnic;
                }
            }
            else
            {
                userOrders = (await this.pedidosDao.GetUserOrderByUserId(new List<string> { userId }))
                    .Where(x => x.Status != ServiceConstants.Finalizado && x.Status != ServiceConstants.Almacenado)
                    .ToList();
            }

            var resultFormula = await this.GetSapOrders(userOrders);
            var groups = ServiceUtils.GroupUserOrder(resultFormula, userOrders, isTecnic);
            groups.RequireTechnical = users.First().TechnicalRequire;
            await this.ValidateFabOrders(groups.Status);
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
            var ugpCodesResponse = await this.sapAdapter.PostSapAdapter(updateFormula.Components.Select(x => x.ProductId).ToList(), ServiceConstants.ProductUnit);
            var ugpData = JsonConvert.DeserializeObject<List<ProductUnitDto>>(ugpCodesResponse.Response.ToString());

            updateFormula.Components.ForEach(item =>
            {
                var selectedUnit = ugpData.FirstOrDefault(x => x.ProductoId == item.ProductId) ?? new ProductUnitDto() { Id = 0 };
                item.UnitCode = selectedUnit.Id;
            });

            var resultSapApi = await this.serviceLayerAdapterService.PostAsync(updateFormula, ServiceConstants.UpdateFormula);

            if (ServiceShared.CalculateAnd(resultSapApi.Success, !string.IsNullOrEmpty(updateFormula.Comments)))
            {
                await this.UpdateFabOrderComments(updateFormula.FabOrderId, updateFormula.Comments);
            }

            var utils = new PedidosUtils(this.redis);
            var listComponents = updateFormula.Components.Where(x => x.Action == ServiceConstants.Insert).Select(y => y.ProductId).ToList();
            listComponents = listComponents.Where(x => ServiceConstants.ListComponentsMostAssigned.Any(y => x.Contains(y))).ToList();
            await utils.UpdateMostUsedComponents(listComponents, ServiceConstants.RedisComponents);

            return ServiceUtils.CreateResult(resultSapApi.Success, resultSapApi.Code, null, JsonConvert.SerializeObject(resultSapApi.Response), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateStatusOrder(List<UpdateStatusOrderModel> updateStatusOrder)
        {
            var orders = updateStatusOrder.Select(x => x.OrderId.ToString()).ToList();
            var ordersList = (await this.pedidosDao.GetUserOrderByProducionOrder(orders)).ToList();
            var hasPedidosType = ordersList.Any(order => !string.IsNullOrEmpty(order.Salesorderid));
            var isTecnicUser = updateStatusOrder.All(x => (UserRoleType)x.UserRoleType == UserRoleType.Tecnic);
            var (isValidQfbs, message) = await this.ValidateQfbConfiguration(updateStatusOrder, isTecnicUser);

            if (ServiceShared.CalculateAnd(!isValidQfbs, hasPedidosType))
            {
                return ServiceUtils.CreateResult(false, 400, message, null, null);
            }

            var listOrderLogToInsert = new List<SalesLogs>();

            ordersList.ForEach(x =>
            {
                var order = updateStatusOrder.FirstOrDefault(y => y.OrderId.ToString().Equals(x.Productionorderid));
                order ??= new UpdateStatusOrderModel();
                x.Status = order.Status ?? x.Status;
                x.TecnicId = ServiceShared.CalculateTernary(isTecnicUser, order.UserId ?? x.TecnicId, x.TecnicId);
                x.Userid = ServiceShared.CalculateTernary(isTecnicUser, x.Userid, order.UserId ?? x.Userid);
                x.StatusForTecnic = ServiceShared.CalculateTernary(x.Status == ServiceConstants.Proceso, ServiceConstants.Asignado, x.Status);
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(x.Userid, new List<UserOrderModel> { x }));
            });

            await this.pedidosDao.UpdateUserOrders(ordersList);
            if (updateStatusOrder.Any(x => x.Status == ServiceConstants.Entregado))
            {
                var saleOrderId = ordersList.FirstOrDefault().Salesorderid;
                ordersList = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { saleOrderId })).ToList();
                var allDelivered = ordersList.Where(x => x.IsProductionOrder && x.Status != ServiceConstants.Cancelled).All(y => y.Status == ServiceConstants.Entregado);
                var saleOrder = ordersList.FirstOrDefault(x => x.IsSalesOrder);
                saleOrder.Status = ServiceShared.CalculateTernary(allDelivered, ServiceConstants.Entregado, saleOrder.Status);
                saleOrder.StatusForTecnic = ServiceShared.CalculateTernary(saleOrder.Status == ServiceConstants.Proceso, ServiceConstants.Asignado, saleOrder.Status);
                var userId = isTecnicUser ? ordersList.FirstOrDefault().TecnicId : ordersList.FirstOrDefault().Userid;
                if (allDelivered)
                {
                    listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(userId, new List<UserOrderModel> { saleOrder }));
                }

                await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });
            }

            _ = this.kafkaConnector.PushMessage(listOrderLogToInsert, ServiceConstants.KafkaInsertLogsConfigName);

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
            var failed = new List<ProductionOrderFailedResultModel>();
            var listSaleOrdersToFinish = finishOrders.Select(x => x.OrderId.ToString()).ToList();

            var productionOrders = await this.pedidosDao.GetUserOrderBySalesOrderIdAndNotInStatus(listSaleOrdersToFinish, new List<string> { ServiceConstants.Cancelled });

            var fnalizeProductionOrder = finishOrders
                .SelectMany(fo =>
                {
                    return productionOrders.Where(po => !string.IsNullOrWhiteSpace(po.Productionorderid) && int.TryParse(po.Salesorderid, out var salesId) && salesId == fo.OrderId)
                        .Select(po => new FinalizeProductionOrderModel
                        {
                            UserId = fo.UserId,
                            ProductionOrderId = int.Parse(po.Productionorderid),
                            SourceProcess = ServiceConstants.SalesOrders,
                            Batches = new List<BatchesConfigurationModel>(),
                        });
                }).ToList();

            var request = await ServiceUtils.IsProductionOrderBeingProcessed(fnalizeProductionOrder, failed, this.pedidosDao, this.redis, this.logger);

            var result = await this.productionOrdersService.FinalizeProductionOrdersAsync(request);
            var response = result.Response as FinalizeProductionOrdersResult;

            var allFailed = response.Failed.Concat(failed).Distinct().ToList();

            var finalResult = new FinalizeProductionOrdersResult
            {
                Successful = response.Successful,
                Failed = allFailed,
            };

            return ServiceUtils.CreateResult(true, 200, null, finalResult, null);
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
            _ = this.kafkaConnector.PushMessage(listOrderLogToInsert, ServiceConstants.KafkaInsertLogsConfigName);

            var resultAsesors = await this.sapAdapter.PostSapAdapter(succesfuly.Select(x => int.Parse(x.Salesorderid)).Distinct().ToList(), ServiceConstants.GetAsesorsMail);
            var resultAsesorEmail = JsonConvert.DeserializeObject<List<AsesorModel>>(JsonConvert.SerializeObject(resultAsesors.Response));
            var asesorsToReportingEmail = new List<object>();

            foreach (var asesor in resultAsesorEmail)
            {
                asesorsToReportingEmail.Add(new
                {
                    customerName = asesor.Cliente,
                    destinyEmail = asesor.Email,
                    salesOrders = asesor.OrderId.ToString(),
                    comments = rejectOrders.Comments,
                });
            }

            // send Emails
            _ = this.reportingService.PostReportingService(new { rejectedOrder = asesorsToReportingEmail }, ServiceConstants.SendEmailToRejectedOrders);
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
            var userOrdersFab = (await this.pedidosDao.GetUserOrderByUserIdAndStatusAndTecnic(new List<string> { userId }, new List<string> { status })).Where(x => x.IsProductionOrder).ToList();
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
            var failed = new List<ProductionOrderFailedResultModel>();

            var fnalizeProductionOrder = finishOrders.Select(fo => new FinalizeProductionOrderModel
                {
                    UserId = fo.UserId,
                    ProductionOrderId = fo.OrderId,
                    SourceProcess = ServiceConstants.FabOrders,
                    Batches = fo.Batches,
                }).ToList();

            var request = await ServiceUtils.IsProductionOrderBeingProcessed(fnalizeProductionOrder, failed, this.pedidosDao, this.redis, this.logger);

            var result = await this.productionOrdersService.FinalizeProductionOrdersAsync(request);
            var response = result.Response as FinalizeProductionOrdersResult;

            var allFailed = response.Failed.Concat(failed).Distinct().ToList();

            var finalResult = new FinalizeProductionOrdersResult
            {
                Successful = response.Successful,
                Failed = allFailed,
            };

            return ServiceUtils.CreateResult(true, 200, null, finalResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateBatches(List<AssignBatchModel> assignBatches)
        {
            var resultSapServiceLayer = await this.serviceLayerAdapterService.PatchAsync(ServiceConstants.UpdateProductionOrderBatchesServiceLayer, JsonConvert.SerializeObject(assignBatches));
            var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultSapServiceLayer.Response.ToString());
            var listWithError = ServiceUtils.GetValuesContains(dictResult, ServiceConstants.ErrorUpdateFabOrd);
            var listErrorId = ServiceUtils.GetErrorsFromSapDiDic(listWithError);
            var userError = listErrorId.Any() ? ServiceConstants.ErroAlAsignar : null;

            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(
                assignBatches
                .Select(x => x.OrderId.ToString()).ToList())).ToList();
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
            var newQfbSignatureAsByte = Convert.FromBase64String(updateOrderSignature.QfbSignature.ValidateIfNull());
            var newTechSignatureAsByte = Convert.FromBase64String(updateOrderSignature.TechnicalSignature.ValidateIfNull());

            var listSignatureToInsert = new List<UserOrderSignatureModel>();
            var listToUpdate = new List<UserOrderSignatureModel>();
            var listOrderLogToInsert = new List<SalesLogs>();
            orders.ForEach(o =>
            {
                var signature = orderSignatures.FirstOrDefault(x => x.UserOrderId == o.Id);
                this.GetOrdersSignatures(signature, listSignatureToInsert, listToUpdate, o, newTechSignatureAsByte, newQfbSignatureAsByte, string.IsNullOrEmpty(o.TecnicId));
                o.FinishDate = DateTime.Now;
                o.Status = ServiceConstants.Terminado;
                o.StatusForTecnic = ServiceConstants.Terminado;
                o.PackingDate = ServiceShared.CalculateTernary(o.PackingDate.HasValue, o.PackingDate, DateTime.Now);
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
                var preProdOrderSap = await ServiceUtils.GetOrdersDetailsForMagistral(this.sapAdapter, saleIds);
                var parentOrders = preProdOrderSap.SelectMany(pre => pre.Detalle).Where(pre => pre.OrderRelationType == "Y").ToList();
                var parentOrdersSeparationDetail = await this.pedidosDao.GetProductionOrderSeparationByOrderId(parentOrders.Select(x => x.OrdenFabricacionId).ToList());

                saleOrder.ForEach(sale =>
                {
                    var orderBySale = allOrders.Where(x => x.Salesorderid == sale.Salesorderid).ToList();
                    var productionOrders = orderBySale.Where(x => x.IsProductionOrder).Select(x => int.Parse(x.Productionorderid)).ToList();
                    var hasPendingToSeparate = productionOrders.Any(x => parentOrdersSeparationDetail.Any(y => x == y.OrderId && y.AvailablePieces > 0));
                    var areInvalidOrders = orderBySale.Any(x => x.IsProductionOrder && !listProductionOrders.Contains(x.Productionorderid) && !ServiceConstants.ValidStatusTerminar.Contains(x.Status));
                    var tupleValues = preProdOrderSap.FirstOrDefault(x => x.Order.DocNum == int.Parse(sale.Salesorderid));
                    tupleValues ??= new OrderWithDetailModel { Detalle = new List<CompleteDetailOrderModel>() };
                    var previousStatus = sale.Status;
                    sale.Status = ServiceShared.CalculateTernary(areInvalidOrders || tupleValues.Detalle.Any(x => string.IsNullOrEmpty(x.Status)) || hasPendingToSeparate, sale.Status, ServiceConstants.Terminado);
                    sale.StatusForTecnic = sale.Status;

                    /** add logs**/
                    if (previousStatus != sale.Status)
                    {
                        listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(updateOrderSignature.UserId, new List<UserOrderModel> { sale }));
                    }

                    listorderToUpdate.Add(sale);
                });
            }

            await this.pedidosDao.UpdateUserOrders(listorderToUpdate);
            _ = this.kafkaConnector.PushMessage(listOrderLogToInsert, ServiceConstants.KafkaInsertLogsConfigName);
            return ServiceUtils.CreateResult(true, 200, null, updateOrderSignature, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderModel isolatedFabOrder)
        {
            var listOrderLogToInsert = new List<SalesLogs>();
            var payload = new { isolatedFabOrder.ProductCode };
            var diapiResult = await this.serviceLayerAdapterService.PostAsync(payload, ServiceConstants.CreateIsolatedFabOrder);

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

                UserOrderModel newProductionOrder = new UserOrderModel
                {
                    Salesorderid = string.Empty,
                    Productionorderid = productionOrderId.ToString(),
                    CreatorUserId = isolatedFabOrder.UserId,
                    CreationDate = DateTime.Now.FormatedLargeDate(),
                    Status = ServiceConstants.Planificado,
                    PlanningDate = DateTime.Now,
                };

                /** add logs**/
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(isolatedFabOrder.UserId, new List<UserOrderModel> { newProductionOrder }));
                await this.pedidosDao.InsertUserOrder(new List<UserOrderModel> { newProductionOrder });
                _ = this.kafkaConnector.PushMessage(listOrderLogToInsert, ServiceConstants.KafkaInsertLogsConfigName);
                await this.SaveCommonComponents(isolatedFabOrder.ProductCode);
            }

            if (ServiceShared.CalculateAnd(!string.IsNullOrEmpty(resultMessage.Key), isolatedFabOrder.IsFromQfbProfile))
            {
                await AsignarLogic.AssignOrder(
                    new ManualAssignModel { DocEntry = new List<int> { productionOrderId }, UserId = isolatedFabOrder.UserId, UserLogistic = isolatedFabOrder.UserId },
                    new Dtos.Models.QfbTecnicInfoDto(),
                    this.pedidosDao,
                    this.serviceLayerAdapterService,
                    this.sapAdapter,
                    this.kafkaConnector);
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

            sapOrders = FilterByClassifications(sapOrders, parameters);
            var sapOrdersId = sapOrders.Select(x => x.OrdenId.ToString()).ToList();
            var userOrders = (await this.pedidosDao.GetUserOrderByProducionOrder(sapOrdersId)).ToList();
            var usersId = userOrders.Select(x => x.Userid).ToList();

            var userResponse = await this.userService.PostSimpleUsers(usersId, ServiceConstants.GetUsersById);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());

            var orderToReturn = (await GetFabOrderUtils.CreateModels(sapOrders, userOrders, users, this.redis)).OrderBy(o => o.DocNum).ToList();

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
            var result = await SendToGeneratePdfUtils.CreateModelGeneratePdf(ordersId, new List<int>(), this.sapAdapter, this.pedidosDao, this.sapFileService, this.userService, false);
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
            var userError = ServiceShared.CalculateTernary(listWithError.Any(), ServiceConstants.ErrorCrearPdf, null);
            listErrorId = ServiceShared.CalculateTernary(listErrorId.Any(), listErrorId, listRoutes);
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
            saleOrder.FinishedLabel = ServiceShared.CalculateTernary(allChecked, 1, 0);
            saleOrder.FinalizedDate = ServiceShared.CalculateTernary(allChecked, DateTime.Now, saleOrder.FinalizedDate);

            await this.pedidosDao.UpdateUserOrders(new List<UserOrderModel> { saleOrder });

            return ServiceUtils.CreateResult(true, 200, null, updateDesignerLabels, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateSaleOrderPdf(List<CreateOrderPdfDto> orders)
        {
            var listRoutes = new List<string>();
            var responseFile = await this.sapFileService.PostSimple(orders, ServiceConstants.CreateSalePdf);

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

        /// <inheritdoc/>
        public async Task<ResultModel> SignOrdersByTecnic(FinishOrderModel tecnicOrderSignature)
        {
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(tecnicOrderSignature.FabricationOrderId.Select(ts => ts.ToString()).ToList())).ToList();
            var orderSignatures = (await this.pedidosDao.GetSignaturesByUserOrderId(orders.Select(x => x.Id).Distinct().ToList())).ToList();
            var tecnicSignatureAsByte = Convert.FromBase64String(tecnicOrderSignature.TechnicalSignature.ValidateIfNull());
            var qfbSignatureAsByte = Convert.FromBase64String(tecnicOrderSignature.QfbSignature.ValidateIfNull());
            var listSignatureToInsert = new List<UserOrderSignatureModel>();
            var listToUpdate = new List<UserOrderSignatureModel>();
            var listOrderLogToInsert = new List<SalesLogs>();
            orders.ForEach(order =>
            {
                var signature = orderSignatures.FirstOrDefault(x => x.UserOrderId == order.Id);
                this.GetOrdersSignatures(signature, listSignatureToInsert, listToUpdate, order, tecnicSignatureAsByte, qfbSignatureAsByte, true);
                order.StatusForTecnic = ServiceConstants.SignedStatus;
                order.PackingDate = DateTime.Now;
                listOrderLogToInsert.AddRange(ServiceUtils.AddSalesLog(tecnicOrderSignature.UserId, new List<UserOrderModel> { order }));
            });

            await this.pedidosDao.InsertOrderSignatures(listSignatureToInsert);
            await this.pedidosDao.SaveOrderSignatures(listToUpdate);
            await this.pedidosDao.UpdateUserOrders(orders);
            _ = this.kafkaConnector.PushMessage(listOrderLogToInsert, ServiceConstants.KafkaInsertLogsConfigName);
            return ServiceUtils.CreateResult(true, 200, null, tecnicOrderSignature, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetInvalidOrdersByMissingTecnicSign(List<string> productionOrderIds)
        {
            var orders = (await this.pedidosDao.GetUserOrderByProducionOrder(productionOrderIds))
                .Where(order => !string.IsNullOrEmpty(order.TecnicId)).ToList();

            var orderSignatures = (await this.pedidosDao.GetSignaturesByUserOrderId(orders.Select(x => x.Id).Distinct().ToList())).ToList();
            var invalidProductionOrderIds = new List<string>();
            var orderSign = new UserOrderSignatureModel();

            orders.ForEach(order =>
            {
                orderSign = orderSignatures.FirstOrDefault(sign => order.Id == sign.UserOrderId);
                orderSign ??= new UserOrderSignatureModel { TechnicalSignature = null };

                if (orderSign.TechnicalSignature == null)
                {
                    invalidProductionOrderIds.Add(order.Productionorderid);
                }
            });

            return ServiceUtils.CreateResult(true, 200, null, invalidProductionOrderIds, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetUserOrdersByInvoiceId(List<int> invoicesid, string type)
        {
            var result = await this.pedidosDao.GetUserOrderByInvoiceTypeAndId(new List<string> { type }, invoicesid);

            return ServiceUtils.CreateResult(true, 200, null, result, null);
        }

        private static List<FabricacionOrderModel> FilterByClassifications(List<FabricacionOrderModel> orders, Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey(ServiceConstants.Classifications) || string.IsNullOrWhiteSpace(parameters[ServiceConstants.Classifications]) || parameters[ServiceConstants.Classifications] == ServiceConstants.AllClassifications)
            {
                return orders;
            }

            var allowedClassifications = parameters[ServiceConstants.Classifications]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return orders
                .Where(order => allowedClassifications.Contains(order.OrderType) || string.IsNullOrWhiteSpace(order.OrderType))
                .ToList();
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
                x.FinishedLabel = ServiceShared.CalculateTernary(orderToUpdate.Checked, 1, 0);
                x.FinalizedDate = ServiceShared.CalculateTernary(orderToUpdate.Checked, DateTime.Now, x.FinalizedDate);

                if (ServiceShared.CalculateAnd(orderSignatureToUpdate == null, orderToUpdate.Checked))
                {
                    var newSignature = new UserOrderSignatureModel
                    {
                        DesignerSignature = signature,
                        UserOrderId = x.Id,
                        DesignerId = updateDesignerLabels.UserId,
                    };

                    listNewSignatures.Add(newSignature);
                }
                else if (ServiceShared.CalculateAnd(orderSignatureToUpdate != null, orderToUpdate.Checked))
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
            var resultFormula = new ConcurrentBag<CompleteFormulaWithDetalle>();
            var listsOfData = ServiceUtils.GetGroupsOfList(userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).ToList(), 20);

            await Task.WhenAll(listsOfData.Select(async x =>
            {
                var listIds = x.Select(y => int.Parse(y.Productionorderid)).ToList();
                var result = await this.sapAdapter.PostSapAdapter(listIds, ServiceConstants.GetFormula);

                var formula = JsonConvert.DeserializeObject<List<CompleteFormulaWithDetalle>>(JsonConvert.SerializeObject(result.Response));
                foreach (var item in formula)
                {
                    resultFormula.Add(item);
                }
            }));

            return resultFormula.ToList();
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

        private async Task<List<object>> ValidateQuantityConsumedFinishOrders(List<CloseProductionOrderModel> finishOrders, List<int> successfullyOrdersfab)
        {
            var sapResponse = await this.sapAdapter.PostSapAdapter(successfullyOrdersfab, ServiceConstants.EndPointToValidateQuatitiesOrdersFormula);
            var orders = JsonConvert.DeserializeObject<List<CompleteDetalleFormulaModel>>(sapResponse.Response.ToString());
            var ordersInvalid = orders
                .GroupBy(o => o.OrderFabId)
                .Select(ord =>
                {
                    var user = finishOrders.FirstOrDefault(fo => fo.OrderId == ord.Key);
                    user ??= new CloseProductionOrderModel { UserId = string.Empty };
                    var error = string.Format(ServiceConstants.FailConsumedQuantity, ord.Key);
                    return ServiceUtils.CreateCancellationFail(new OrderIdModel { OrderId = ord.Key, UserId = user.UserId }, error);
                })
                .ToList();
            return ordersInvalid;
        }

        private void GetOrdersSignatures(
                                 UserOrderSignatureModel signature,
                                 List<UserOrderSignatureModel> listSignatureToInsert,
                                 List<UserOrderSignatureModel> listToUpdate,
                                 UserOrderModel order,
                                 byte[] tecnicSignatureAsByte,
                                 byte[] qfbSignatureAsByte,
                                 bool updateLastSignature)
        {
            if (signature == null)
            {
                listSignatureToInsert.Add(new UserOrderSignatureModel
                {
                    TechnicalSignature = ServiceShared.CalculateTernary(
                        ServiceShared.CalculateAnd(tecnicSignatureAsByte.Length > 0, updateLastSignature),
                        tecnicSignatureAsByte,
                        null),
                    UserOrderId = order.Id,
                    QfbSignature = ServiceShared.CalculateTernary(qfbSignatureAsByte.Length > 0, qfbSignatureAsByte, null),
                });
            }
            else
            {
                signature.TechnicalSignature = ServiceShared.CalculateTernary(
                        ServiceShared.CalculateAnd(tecnicSignatureAsByte.Length > 0, updateLastSignature),
                        tecnicSignatureAsByte,
                        signature.TechnicalSignature);
                signature.QfbSignature = ServiceShared.CalculateTernary(qfbSignatureAsByte.Length > 0, qfbSignatureAsByte, signature.QfbSignature);
                listToUpdate.Add(signature);
            }
        }

        private async Task<(bool isValidQfbs, string message)> ValidateQfbConfiguration(List<UpdateStatusOrderModel> updateStatusOrder, bool isTecnicUser)
        {
            var isValidQfbs = true;
            var message = string.Empty;

            if (isTecnicUser)
            {
                return (isValidQfbs, message);
            }

            var qfbInfoValidated = (await ServiceUtils.GetQfbInfoById(updateStatusOrder.Select(x => x.UserId).ToList(), this.userService)).ToList();
            var invalidQfbs = qfbInfoValidated.Where(qfb => !qfb.IsValidTecnic);

            if (invalidQfbs.Any())
            {
                message = string.Format(ServiceConstants.QfbWithoutTecnic, string.Join(",", invalidQfbs.Select(x => $"{x.QfbFirstName} {x.QfbLastName}")));
                isValidQfbs = false;
            }

            return (isValidQfbs, message);
        }

        private async Task SaveCommonComponents(string productCode)
        {
            var utils = new PedidosUtils(this.redis);
            var listComponents = new List<string> { productCode };
            await utils.UpdateMostUsedComponents(listComponents, ServiceConstants.RedisBulkOrderKey);
        }

        private async Task ValidateFabOrders(List<QfbOrderDetail> fabOrders)
        {
            var fabOrderParentIds = fabOrders.SelectMany(q => q.Orders).Where(o => o.OrderRelationType == ServiceConstants.ParentOrder).Select(o => o.ProductionOrderId).ToList();
            var productionOrdersRecord = await this.pedidosDao.GetProductionOrderSeparationByOrderId(fabOrderParentIds);

            var completelyDividedOrders = new HashSet<int>(productionOrdersRecord.Where(o => o.Status == ServiceConstants.CompletelyDivided).Select(o => o.OrderId));

            var statusesToFilter = new[] { 2, 5 };

            foreach (var fabOrder in fabOrders)
            {
                if (statusesToFilter.Contains(fabOrder.StatusId))
                {
                    fabOrder.Orders = fabOrder.Orders.Where(order => !completelyDividedOrders.Contains(order.ProductionOrderId)).ToList();

                    fabOrder.Orders = fabOrder.Orders.OrderBy(order =>
                    {
                        var key = (order.AreBatchesComplete, order.OrderRelationType);
                        return ServiceConstants.OrderPriorities.TryGetValue(key, out int priority) ? priority : 5;
                    }).ToList();
                }
            }
        }
    }
}