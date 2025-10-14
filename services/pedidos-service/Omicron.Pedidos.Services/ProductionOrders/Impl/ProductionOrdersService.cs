// <summary>
// <copyright file="ProductionOrdersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.ProductionOrders.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using AutoMapper;
    using global::MediatR;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.ProductionOrders;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// class for ProductionOrdersService.
    /// </summary>
    public class ProductionOrdersService : IProductionOrdersService
    {
        private readonly IPedidosDao pedidosDao;

        private readonly ISapServiceLayerAdapterService serviceLayerAdapterService;

        private readonly IRedisService redisService;

        private readonly IKafkaConnector kafkaConnector;

        private readonly ILogger logger;

        private readonly IMapper mapper;

        private readonly ISapAdapter sapAdapter;

        private readonly ISapFileService sapFileService;

        private readonly IUsersService userService;

        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrdersService"/> class.
        /// </summary>
        /// <param name="pedidosDao">Interface for dao.</param>
        /// <param name="serviceLayerAdapterService">Interface for Service Layer Adapter Service.</param>
        /// <param name="redisService">Interface for Redis Service.</param>
        /// <param name="kafkaConnector">Interface for Kafka Connector.</param>
        /// <param name="sapAdapter">Interface for SAP adapter.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="sapFileService">The sap file service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="mapper">Mapper.</param>
        /// <param name="mediator">Mediator.</param>
        public ProductionOrdersService(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            IRedisService redisService,
            IKafkaConnector kafkaConnector,
            ISapAdapter sapAdapter,
            IUsersService userService,
            ISapFileService sapFileService,
            ILogger logger,
            IMapper mapper,
            IMediator mediator)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.kafkaConnector = kafkaConnector.ThrowIfNull(nameof(kafkaConnector));
            this.sapAdapter = sapAdapter.ThrowIfNull(nameof(sapAdapter));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.mapper = mapper.ThrowIfNull(nameof(mapper));
            this.userService = userService.ThrowIfNull(nameof(userService));
            this.sapFileService = sapFileService.ThrowIfNull(nameof(sapFileService));
            this.mediator = mediator.ThrowIfNull(nameof(mediator));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrdersAsync(List<FinalizeProductionOrderModel> productionOrdersToFinalize)
        {
            try
            {
                var failed = new List<ProductionOrderFailedResultModel>();
                var successfuly = new List<FinalizeProductionOrderModel>();
                this.logger.Information(LogsConstants.StartFinalizeAllProductionOrders, JsonConvert.SerializeObject(productionOrdersToFinalize));
                var productionOrderProcessingStatus = new List<ProductionOrderProcessingStatusModel>();
                foreach (var productionOrder in productionOrdersToFinalize)
                {
                    var processId = Guid.NewGuid().ToString();
                    var logBase = string.Format(LogsConstants.FinalizeProductionOrdersAsync, processId);
                    var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrder.ProductionOrderId);
                    this.logger.Information(LogsConstants.StartValidatePrimaryRules, logBase, productionOrder.ProductionOrderId);
                    var isValidProductionOrder = await this.ValidatePrimaryRules(
                        productionOrder,
                        failed,
                        logBase,
                        processId);

                    if (!isValidProductionOrder)
                    {
                        await this.redisService.DeleteKey(redisKey);
                        continue;
                    }

                    var finalizeProductionOrderPayload = new FinalizeProductionOrderPayload
                    {
                        FinalizeProductionOrder = productionOrder,
                    };

                    var productionOrderProcessing = CreateProductionOrderProcessingStatusModel(
                        processId,
                        productionOrder.ProductionOrderId,
                        ServiceConstants.StepPrimaryValidations,
                        ServiceConstants.FinalizeProcessInProgressStatus,
                        JsonConvert.SerializeObject(finalizeProductionOrderPayload),
                        DateTime.Now);

                    successfuly.Add(productionOrder);
                    productionOrderProcessingStatus.Add(productionOrderProcessing);
                }

                this.logger.Information(LogsConstants.InsertAllProductionOrderProcessingStatus, JsonConvert.SerializeObject(productionOrderProcessingStatus));
                await this.pedidosDao.InsertProductionOrderProcessingStatus(productionOrderProcessingStatus);

                await this.SendKafkaMessagesAsync(productionOrderProcessingStatus);

                var validationsResult = new FinalizeProductionOrdersResult
                {
                    Successful = successfuly.Distinct().ToList(),
                    Failed = failed.Distinct().ToList(),
                };

                this.logger.Information(LogsConstants.EndFinalizeAllProductionOrders, JsonConvert.SerializeObject(productionOrdersToFinalize));
                return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, validationsResult, null);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndFinalizeAllProductionOrdersWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, LogsConstants.AnUnexpectedErrorOccurred, null, null);
            }
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrdersOnSapAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
            this.logger.Information(LogsConstants.StartFinalizeProductionOrderInSap, JsonConvert.SerializeObject(productionOrderProcessingPayload));
            var logBase = string.Format(LogsConstants.FinalizeProductionOrdersOnSapAsync, productionOrderProcessingPayload.Id);

            var (productionOrderUpdated, isProcessSuccesfully) = await this.FinalizeProductionOrdersOnSapProcess(productionOrderProcessingPayload, logBase);
            this.logger.Information(LogsConstants.UpdateProductionOrderProcessingStatus, logBase, JsonConvert.SerializeObject(productionOrderUpdated));

            if (!isProcessSuccesfully)
            {
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, null, null, null);
            }

            this.logger.Information(LogsConstants.SendKafkaMessageFinalizeProductionOrderPostgresql, logBase, JsonConvert.SerializeObject(productionOrderUpdated));

            await this.SendMessageToKafka(
               productionOrderUpdated,
               ServiceConstants.KafkaFinalizeProductionOrderPostgresqlConfigName,
               logBase);

            this.logger.Information(LogsConstants.EndFinalizeProductionOrderInSap, JsonConvert.SerializeObject(productionOrderProcessingPayload));
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrdersOnPostgresqlAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
            this.logger.Information(LogsConstants.StartFinalizeProductionOrderInPostgres, JsonConvert.SerializeObject(productionOrderProcessingPayload));
            var logBase = string.Format(LogsConstants.UpdateProductionOrdersOnPostgresAsync, productionOrderProcessingPayload.Id);

            var (productionOrderUpdated, isProcessSuccesfully) = await this.UpdateProductionOrdersOnPostgresProcess(productionOrderProcessingPayload, logBase);
            this.logger.Information(LogsConstants.UpdateProductionOrderProcessingStatus, logBase, JsonConvert.SerializeObject(productionOrderUpdated));
            await this.pedidosDao.UpdatesProductionOrderProcessingStatus([productionOrderUpdated]);

            if (!isProcessSuccesfully)
            {
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, null, null, null);
            }

            await this.SendMessageToKafka(
                productionOrderUpdated,
                ServiceConstants.KafkaProductionOrderPdfGenerationConfigName,
                logBase);

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> ProductionOrderPdfGenerationAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
            this.logger.Information(LogsConstants.StartCreationPdf, JsonConvert.SerializeObject(productionOrderProcessingPayload));
            var logBase = string.Format(LogsConstants.GeneratePdfOfProductionOrdersAsync, productionOrderProcessingPayload.Id);

            var (productionOrderUpdated, isProcessSuccesfully) = await this.CreateProductionOrderPdf(productionOrderProcessingPayload, logBase);
            this.logger.Information(LogsConstants.UpdateProductionOrderProcessingStatus, logBase, JsonConvert.SerializeObject(productionOrderUpdated));
            await this.pedidosDao.UpdatesProductionOrderProcessingStatus([productionOrderUpdated]);
            await this.DeleteRedisControlKeyToFinalizeProductionOrder(productionOrderUpdated.ProductionOrderId);

            if (!isProcessSuccesfully)
            {
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, null, null, null);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetFailedProductionOrders()
        {
            var allFailedProductionOrders =
                await this.pedidosDao.GetAllProductionOrderProcessingStatusByStatus([ServiceConstants.FinalizeProcessFailedStatus]);

            var productionOrdersToRetry = new List<ProductionOrderProcessingStatusModel>();
            foreach (var po in allFailedProductionOrders)
            {
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, po.ProductionOrderId);
                var existingValue = await this.redisService.GetRedisKey(redisKey);

                if (string.IsNullOrEmpty(existingValue))
                {
                    productionOrdersToRetry.Add(po);
                }
            }

            if (productionOrdersToRetry.Count != 0)
            {
                await this.redisService.StoreListAsync(
                ServiceConstants.ProductionOrderFinalizingToProcessKey,
                productionOrdersToRetry.OrderBy(x => x.LastUpdated),
                ServiceConstants.DefaultRedisValueTimeToLive);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, (int)productionOrdersToRetry.Count, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> RetryFailedProductionOrderFinalization(RetryFailedProductionOrderFinalizationDto payloadRetry)
        {
            var logBase = string.Format(LogsConstants.RetryFailedProductionOrderFinalization, payloadRetry.BatchProcessId);
            await this.UpdateProductionOrdersToRetryStatusInProgress(payloadRetry);

            foreach (var payloadRequest in payloadRetry.ProductionOrderProcessingPayload)
            {
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, payloadRequest.ProductionOrderId);
                await this.redisService.WriteToRedis(redisKey, JsonConvert.SerializeObject(payloadRequest), new TimeSpan(12, 0, 0));
                var payload = this.mapper.Map<ProductionOrderProcessingStatusModel>(payloadRequest);
                await this.RetryFailedProductionOrderFinalizationProcess(payload, logBase);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SeparateOrder(SeparateProductionOrderDto request)
        {
            var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, request.ProductionOrderId);
            var redisValue = await this.redisService.GetRedisKey(redisKey);
            var detailLog = await this.pedidosDao.GetProductionOrderSeparationDetailLogByParentOrderId(request.ProductionOrderId);

            if (!string.IsNullOrEmpty(redisValue) || detailLog != null)
            {
                return ServiceUtils.CreateResult(
                    false,
                    (int)HttpStatusCode.InternalServerError,
                    ServiceConstants.ProductionOrderSeparationProcessMessage,
                    null,
                    null);
            }

            await this.redisService.WriteToRedis(redisKey, JsonConvert.SerializeObject(request));
            await this.SeparateProductionOrderProcessAsync(
                request.ProductionOrderId,
                request.Pieces,
                request.UserId,
                request.DxpOrder,
                request.SapOrder,
                request.TotalPieces);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetFailedDivisionOrders()
        {
            var allFailedDivisionOrders = (await this.pedidosDao.GetAllFailedDivisionOrders()).ToList();

            if (allFailedDivisionOrders.Count != 0)
            {
                await this.redisService.StoreListAsync(
                    ServiceConstants.DivisionOrdersToProcessKey,
                    allFailedDivisionOrders.OrderBy(x => x.LastUpdated),
                    ServiceConstants.DefaultRedisValueTimeToLive);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, allFailedDivisionOrders.Count, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> RetryFailedProductionOrderDivision(RetryFailedProductionOrderDivisionDto payloadRetry)
        {
            var logBase = string.Format(LogsConstants.RetryFailedDivisionOrder, payloadRetry.BatchProcessId);

            foreach (var item in payloadRetry.ProductionOrderProcessingPayload.OrderBy(x => x.LastUpdated))
            {
                var model = this.mapper.Map<ProductionOrderSeparationDetailLogsModel>(item);
                await this.RetryFailedProductionOrderDivisionProcess(model, logBase);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetParentOrderDetail(int fabOrder)
        {
            var ordersToReturn = await this.pedidosDao.GetParentOrderDetailByOrderId(fabOrder);
            var usersId = ordersToReturn.SelectMany(x => new[] { x.UserCreate, x.Qfb }).Distinct().ToList();

            var userResponse = await this.userService.PostSimpleUsers(usersId, ServiceConstants.GetUsersById);
            var users = JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());

            var usersDict = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

            ordersToReturn.ForEach(x =>
            {
                x.UserCreate = usersDict.TryGetValue(x.UserCreate, out var userCreateName) ? userCreateName : string.Empty;
                x.Qfb = usersDict.TryGetValue(x.Qfb, out var qfbName) ? qfbName : string.Empty;
            });

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, ordersToReturn, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOpenOrderProdutions(Dictionary<string, string> parameters)
        {
            try
            {
                var qfbId = parameters[ServiceConstants.QfbId];
                var offset = int.Parse(parameters[ServiceConstants.Offset]);
                var limit = int.Parse(parameters[ServiceConstants.Limit]);

                parameters.TryGetValue(ServiceConstants.Orders, out var orders);
                var logBase = string.Format(LogsConstants.GetOpenOrderProductions, orders ?? qfbId);
                var listOrders = ServiceUtils.SeparateOrders(orders);
                this.logger.Information(LogsConstants.GetOpenOrderProductionsStart, logBase, string.Join(",", listOrders), qfbId, listOrders.Count);

                if (listOrders.Count == 0)
                {
                    this.logger.Information(LogsConstants.GetOpenOrderProductionsCallGetOpenParents, logBase, qfbId);
                    return await this.GetOpenParentsForQfb(qfbId, offset, limit, ServiceConstants.PartiallyDivided);
                }

                var existsParents = await this.pedidosDao.FindExistingParentIds(listOrders);
                var childToParent = await this.pedidosDao.FindParentsByChildIds(listOrders);
                var allParentIds = existsParents.Union(childToParent.Values).ToList();

                if (allParentIds.Count == 0)
                {
                    this.logger.Information(LogsConstants.GetOpenOrderProductionsNoData, logBase, string.Join(",", listOrders));
                    return ServiceUtils.CreateResult(true, 200, null, new List<OpenOrderProductionModel>(), null, null);
                }

                var parents = await this.pedidosDao.GetParentsAssignedToQfbByIds(allParentIds, qfbId, ServiceConstants.PartiallyDivided);

                if (parents.Count == 0)
                {
                    this.logger.Information(LogsConstants.GetOpenOrderProductionsNoData, logBase, string.Join(",", listOrders));
                    return ServiceUtils.CreateResult(true, 200, null, new List<OpenOrderProductionModel>(), null, null);
                }

                var parentIds = parents
                    .Select(p => int.TryParse(p.OrderProductionId, out var n) ? n : -1)
                    .Where(n => n >= 0)
                    .Distinct()
                    .ToList();

                var childrenMap = await this.GetChildrenMapByParents(parentIds, excludeCanceled: true);
                var requestedChildIds = childToParent.GroupBy(kv => kv.Value, kv => kv.Key.ToString())
                                                    .ToDictionary(g => g.Key, g => g.ToHashSet());

                var result = parents
                    .Where(p => int.TryParse(p.OrderProductionId, out _))
                    .Select(parent =>
                    {
                        var parentId = int.Parse(parent.OrderProductionId);
                        var allVisibleChildren = childrenMap.GetValueOrDefault(parentId, new List<OpenOrderProductionDetailModel>())
                            .OrderBy(c => int.TryParse(c.OrderProductionDetailId, out var n) ? n : int.MaxValue)
                            .ToList();

                        var hasRequestedChildren = requestedChildIds.ContainsKey(parentId);
                        var isParentRequested = existsParents.Contains(parentId);

                        if (hasRequestedChildren)
                        {
                            var filteredChildren = allVisibleChildren
                                .Where(c => requestedChildIds[parentId].Contains(c.OrderProductionDetailId))
                                .ToList();

                            parent.OrderProductionDetail = filteredChildren.Count > 0 ? filteredChildren :
                                                           (isParentRequested ? allVisibleChildren : new List<OpenOrderProductionDetailModel>());
                            parent.AutoExpandOrderDetail = filteredChildren.Count > 0;
                        }
                        else
                        {
                            parent.OrderProductionDetail = allVisibleChildren;
                            parent.AutoExpandOrderDetail = false;
                        }

                        return parent;
                    })
                    .Where(p =>
                    {
                        var hasChildren = (p.OrderProductionDetail?.Count ?? 0) > 0;
                        if (!int.TryParse(p.OrderProductionId, out var pid))
                        {
                            return hasChildren;
                        }

                        return hasChildren || existsParents.Contains(pid);
                    })
                    .OrderBy(p => int.TryParse(p.OrderProductionId, out var n) ? n : int.MaxValue)
                    .ToList();

                var total = result.Count;
                var page = result.Skip(offset).Take(limit).ToList();
                await this.FullName(page);
                this.logger.Information(LogsConstants.GetOpenOrderProductionsSuccess, logBase, string.Join(",", result.Select(r => r.OrderProductionId)), total, page.Count);

                return ServiceUtils.CreateResult(true, 200, null, page, null, $"{total}");
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, LogsConstants.GetOpenOrderProductionsError);
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, LogsConstants.AnUnexpectedErrorOccurred, null, null, null);
            }
        }

        private static ProductionOrderFailedResultModel CreateFinalizedFailedResponse(FinalizeProductionOrderModel orderToFinish, string reason)
        {
            return new ProductionOrderFailedResultModel
            {
                OrderId = orderToFinish.ProductionOrderId,
                UserId = orderToFinish.UserId,
                Reason = reason,
            };
        }

        private static ProductionOrderProcessingStatusModel CreateProductionOrderProcessingStatusModel(
            string processId,
            int productionOrderId,
            string lastStep,
            string status,
            string payload,
            DateTime createdAt)
        {
            return new ProductionOrderProcessingStatusModel
            {
                Id = processId,
                ProductionOrderId = productionOrderId,
                LastStep = lastStep,
                Status = status,
                Payload = payload,
                CreatedAt = createdAt,
                LastUpdated = DateTime.Now,
            };
        }

        private static ProductionOrderProcessingStatusModel UpdateProcessingStatusAsync(
            ProductionOrderProcessingStatusModel model,
            string status,
            string errorMessage = null,
            string lastStep = null)
        {
            model.Status = status;
            model.ErrorMessage = errorMessage;
            model.LastStep = lastStep;
            model.LastUpdated = DateTime.Now;
            return model;
        }

        private async Task<Dictionary<int, List<OpenOrderProductionDetailModel>>> GetChildrenMapByParents(IEnumerable<int> parentIds, bool excludeCanceled = true)
        {
            var rawData = await this.pedidosDao.GetChildrenByParentIds(parentIds, excludeCanceled);
            var culture = CultureInfo.GetCultureInfo("es-MX");
            var dict = new Dictionary<int, List<OpenOrderProductionDetailModel>>();

            foreach (var x in rawData)
            {
                if (!dict.TryGetValue(x.OrderId, out List<OpenOrderProductionDetailModel> list))
                {
                    list = new List<OpenOrderProductionDetailModel>();
                    dict[x.OrderId] = list;
                }

                list.Add(new OpenOrderProductionDetailModel
                {
                    OrderProductionDetailId = x.DetailOrderId.ToString(),
                    AssignedPieces = x.AssignedPieces,
                    AssignedQfb = x.AssignedQfb,
                    DateCreated = (x.CreatedAt ?? DateTime.MinValue)
                        .ToLocalTime()
                        .ToString("dd/MM/yyyy HH:mm:ss", culture),
                });
            }

            return dict;
        }

        private async Task FullName(List<OpenOrderProductionModel> page)
        {
            var userIds = page
                .SelectMany(p => new[] { p.QfbWhoSplit }
                    .Concat(p.OrderProductionDetail?.Select(d => d.AssignedQfb) ?? Enumerable.Empty<string>()))
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (userIds.Count > 0)
            {
                var usersResp = await this.userService.PostSimpleUsers(userIds, ServiceConstants.GetUsersById);
                var users = JsonConvert.DeserializeObject<List<UserModel>>(usersResp.Response?.ToString() ?? "[]")
                                ?? new List<UserModel>();

                var fullNameById = users
                    .Where(u => !string.IsNullOrWhiteSpace(u.Id))
                    .ToDictionary(
                        u => u.Id,
                        u => string.Join(" ", new[] { u.FirstName, u.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim(),
                        StringComparer.OrdinalIgnoreCase);

                foreach (var p in page)
                {
                    if (!string.IsNullOrWhiteSpace(p.QfbWhoSplit) &&
                        fullNameById.TryGetValue(p.QfbWhoSplit, out var splitFullName) &&
                        !string.IsNullOrWhiteSpace(splitFullName))
                    {
                        p.QfbWhoSplit = splitFullName;
                    }

                    foreach (var d in p.OrderProductionDetail ?? new List<OpenOrderProductionDetailModel>())
                    {
                        if (!string.IsNullOrWhiteSpace(d.AssignedQfb) &&
                            fullNameById.TryGetValue(d.AssignedQfb, out var assigneeFullName) &&
                            !string.IsNullOrWhiteSpace(assigneeFullName))
                        {
                            d.AssignedQfb = assigneeFullName;
                        }
                    }
                }
            }
        }

        private async Task<ResultModel> GetOpenParentsForQfb(
        string qfbId,
        int offset,
        int limit,
        string partiallyDivided)
        {
            var allParents = await this.pedidosDao.GetAllOpenParentOrdersByQfb(qfbId, partiallyDivided);
            var ordered = allParents
                .OrderBy(p => int.TryParse(p.OrderProductionId, out var n) ? n : int.MaxValue)
                .ToList();

            var total = ordered.Count;
            var page = ordered.Skip(offset).Take(limit).ToList();

            if (page.Count > 0)
            {
                var parentIds = page
                    .Where(p => int.TryParse(p.OrderProductionId, out _))
                    .Select(p => int.Parse(p.OrderProductionId))
                    .Distinct()
                    .ToList();

                var childrenMap = await this.GetChildrenMapByParents(parentIds, excludeCanceled: true);

                page.ForEach(parent =>
                {
                    parent.OrderProductionDetail = int.TryParse(parent.OrderProductionId, out var pid) &&
                    childrenMap.TryGetValue(pid, out var details) ? details : new List<OpenOrderProductionDetailModel>();
                    parent.AutoExpandOrderDetail = false;
                });

                await this.FullName(page);
            }

            return ServiceUtils.CreateResult(true, 200, null, page, null, $"{total}");
        }

        private async Task SeparateProductionOrderProcessAsync(
            int productionOrderId,
            int pieces,
            string userId,
            string dxpOrder,
            int? sapOrder,
            int totalPieces)
        {
            var separationId = Guid.NewGuid().ToString();
            var command = new StartProductionOrderSeparationCommand(
                productionOrderId,
                pieces,
                separationId,
                userId,
                dxpOrder,
                sapOrder,
                totalPieces);
            await this.mediator.Send(command);
        }

        private async Task<ProductionOrderProcessingStatusModel> UpdateProcessStatus(
            string id,
            string status,
            string payload,
            string errorMessage = null,
            string lastStep = null)
        {
            var modelToUpdate = await this.pedidosDao.GetFirstProductionOrderProcessingStatusById(id);
            modelToUpdate.Status = status;
            modelToUpdate.ErrorMessage = errorMessage;
            modelToUpdate.LastStep = lastStep;
            modelToUpdate.LastUpdated = DateTime.Now;
            modelToUpdate.Payload = payload;
            await this.pedidosDao.UpdatesProductionOrderProcessingStatus([modelToUpdate]);

            if (status == ServiceConstants.FinalizeProcessFailedStatus)
            {
                await this.DeleteRedisControlKeyToFinalizeProductionOrder(modelToUpdate.ProductionOrderId);
            }

            return modelToUpdate;
        }

        private async Task SendKafkaMessagesAsync(List<ProductionOrderProcessingStatusModel> productionOrderProcessingStatusList)
        {
            foreach (var productionOrderProcessing in productionOrderProcessingStatusList)
            {
                var logBase = string.Format(LogsConstants.FinalizeProductionOrdersAsync, productionOrderProcessing.Id);
                this.logger.Information(LogsConstants.SendKafkaMessageFinalizeProductionOrderSap, logBase, JsonConvert.SerializeObject(productionOrderProcessing));

                await this.SendMessageToKafka(
                    productionOrderProcessing,
                    ServiceConstants.KafkaFinalizeProductionOrderSapConfigName,
                    logBase);
            }
        }

        private async Task RetryFailedProductionOrderDivisionProcess(ProductionOrderSeparationDetailLogsModel payload, string logBase)
        {
            var redisKey = string.Format(ServiceConstants.ProductionOrderSeparationProcessKey, payload.ParentProductionOrderId);
            await this.redisService.WriteToRedis(redisKey, JsonConvert.SerializeObject(payload));

            try
            {
                var p = new SeparateProductionOrderDto { ProductionOrderId = payload.ParentProductionOrderId };
                if (!string.IsNullOrWhiteSpace(payload.Payload))
                {
                    var orderSeparation = JsonConvert.DeserializeObject<SeparateProductionOrderDto>(payload.Payload);
                    if (orderSeparation != null)
                    {
                        p = orderSeparation;

                        p.ProductionOrderId = p.ProductionOrderId == 0
                        ? payload.ParentProductionOrderId
                        : p.ProductionOrderId;
                    }
                }

                var last = payload.LastStep?.Trim();
                switch (last)
                {
                    case null:
                    case ServiceConstants.EmptyValue:
                    case ServiceConstants.StartStep:
                    case ServiceConstants.UpdateCancelParentOrderStep:
                    case ServiceConstants.CancelSapStep:
                    case ServiceConstants.CancelPostgresStep:
                        {
                            var cmd = new CancelProductionOrderCommand(
                                p.ProductionOrderId, p.Pieces, payload.Id, p.UserId, p.DxpOrder, p.SapOrder, p.TotalPieces)
                            { LastStep = last };

                            await this.mediator.Send(cmd);
                            break;
                        }

                    case ServiceConstants.SaveHistoryStep:
                    case ServiceConstants.StepCreateChildOrderSap:
                    case ServiceConstants.StepCreateChildOrderWithComponentsSap:
                    case ServiceConstants.StepCreateChildOrderPostgres:
                    case ServiceConstants.StepSaveChildOrderHistory:
                        {
                            var cmd = new CreateChildOrdersSapCommand(
                                p.ProductionOrderId, p.Pieces, payload.Id, p.UserId, p.DxpOrder, p.SapOrder, p.TotalPieces, last);

                            if (payload.ChildProductionOrderId.HasValue)
                            {
                                cmd.ProductionOrderChildId = payload.ChildProductionOrderId.Value;
                            }

                            await this.mediator.Send(cmd);
                            break;
                        }

                    default:
                        this.logger.Error(LogsConstants.StepNotRecognized, logBase, payload.Id, payload.ParentProductionOrderId, last);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, $"{logBase} - Error reintentando división - Id={payload.Id}, Parent={payload.ParentProductionOrderId}, Step={payload.LastStep}");
            }
        }

        private async Task RetryFailedProductionOrderFinalizationProcess(
            ProductionOrderProcessingStatusModel payload, string logBase)
        {
            try
            {
                switch (payload.LastStep?.Trim())
                {
                    case ServiceConstants.StepPrimaryValidations:
                    case ServiceConstants.StepCreateInventoryStep:
                    case ServiceConstants.StepCreateReceiptStep:
                        this.logger.Information(
                            LogsConstants.RetryFinalizingProductionOrder,
                            logBase,
                            payload.Id,
                            payload.ProductionOrderId,
                            LogsConstants.CloseInSapNextStep);

                        await this.FinalizeProductionOrdersOnSapAsync(payload);
                        break;
                    case ServiceConstants.StepSuccessfullyClosedInSapStep:
                        this.logger.Information(
                            LogsConstants.RetryFinalizingProductionOrder,
                            logBase,
                            payload.Id,
                            payload.ProductionOrderId,
                            LogsConstants.CloseInPostgresqlNextStep);

                        await this.FinalizeProductionOrdersOnPostgresqlAsync(payload);
                        break;
                    case ServiceConstants.StepUpdatePostgres:
                        this.logger.Information(
                            LogsConstants.RetryFinalizingProductionOrder,
                            logBase,
                            payload.Id,
                            payload.ProductionOrderId,
                            LogsConstants.GeneratePdfNextStep);

                        await this.ProductionOrderPdfGenerationAsync(payload);
                        break;
                    default:
                        this.logger.Error(LogsConstants.StepNotRecognized, logBase, payload.Id, payload.ProductionOrderId, payload.LastStep);
                        break;
                }
            }
            catch (Exception ex)
            {
                var error = string.Format(
                    LogsConstants.RetryFinalizingProductionOrderEndWithError,
                    logBase,
                    payload.Id,
                    payload.ProductionOrderId,
                    ex.Message,
                    ex.InnerException);

                await this.DeleteRedisControlKeyToFinalizeProductionOrder(payload.ProductionOrderId);
                this.logger.Error(ex, error);
            }
        }

        private async Task<(ProductionOrderProcessingStatusModel, bool)> FinalizeProductionOrdersOnSapProcess(
            ProductionOrderProcessingStatusModel productionOrderProcessingPayload,
            string logBase)
        {
            var payload = JsonConvert.DeserializeObject<FinalizeProductionOrderPayload>(productionOrderProcessingPayload.Payload);

            try
            {
                this.logger.Information(
                    LogsConstants.PostSapFinalizeProductionOrderProcess, logBase, JsonConvert.SerializeObject(payload.FinalizeProductionOrder));

                var result = await this.serviceLayerAdapterService.PostAsync(
                    new List<ValidateProductionOrderModel>
                    {
                        new ValidateProductionOrderModel
                        {
                            FinalizeProductionOrder = payload.FinalizeProductionOrder,
                            ProcessId = productionOrderProcessingPayload.Id,
                            LastStep = productionOrderProcessingPayload.LastStep,
                        },
                    },
                    ServiceConstants.SapFinalizeProductionOrdersEndpoint);

                if (!result.Success)
                {
                    this.logger.Error(LogsConstants.ErrorOnSAP, logBase, payload.FinalizeProductionOrder.ProductionOrderId);
                    productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        productionOrderProcessingPayload.Payload,
                        ServiceConstants.ErrorOccurredWhileCommunicatingWithServiceLayerAdapter,
                        productionOrderProcessingPayload.LastStep);
                    return (productionOrderProcessingPayload, false);
                }

                var resultMessages = JsonConvert.DeserializeObject<List<ValidationsToFinalizeProductionOrdersResultModel>>(result.Response.ToString());
                var finalizeProcessResult = resultMessages.First(x => x.FinalizeProductionOrder.ProductionOrderId == productionOrderProcessingPayload.ProductionOrderId);
                productionOrderProcessingPayload.Payload =
                JsonConvert.SerializeObject(new FinalizeProductionOrderPayload
                {
                    FinalizeProductionOrder = finalizeProcessResult.FinalizeProductionOrder,
                });

                if (!string.IsNullOrEmpty(finalizeProcessResult.ErrorMessage))
                {
                    this.logger.Error(LogsConstants.ErrorInSAPWhileFinalizingTheOrder, logBase, productionOrderProcessingPayload.ProductionOrderId);
                    productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        productionOrderProcessingPayload.Payload,
                        finalizeProcessResult.ErrorMessage,
                        finalizeProcessResult.LastStep);
                    return (productionOrderProcessingPayload, false);
                }

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessInProgressStatus,
                        productionOrderProcessingPayload.Payload,
                        productionOrderProcessingPayload.ErrorMessage,
                        finalizeProcessResult.LastStep);
                return (productionOrderProcessingPayload, true);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndFinalizeProductionOrderOnSapWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        productionOrderProcessingPayload.Payload,
                        string.Format(LogsConstants.GenericErrorLog, ex.Message, ex.InnerException),
                        productionOrderProcessingPayload.LastStep);
                return (productionOrderProcessingPayload, false);
            }
        }

        private async Task<bool> ValidatePrimaryRules(
            FinalizeProductionOrderModel orderToFinish,
            List<ProductionOrderFailedResultModel> failed,
            string logBase,
            string processId)
        {
            this.logger.Information(LogsConstants.PostSapValidationFinalization, logBase, JsonConvert.SerializeObject(orderToFinish));
            var result = await this.serviceLayerAdapterService.PostAsync(
                new List<ValidateProductionOrderModel>
                {
                    new ValidateProductionOrderModel
                    {
                        FinalizeProductionOrder = orderToFinish,
                        ProcessId = processId,
                    },
                },
                ServiceConstants.SapValidationFinalizationEndpoint);

            if (!result.Success)
            {
                this.logger.Error(LogsConstants.ErrorOnSAP, logBase, orderToFinish.ProductionOrderId);
                failed.Add(CreateFinalizedFailedResponse(orderToFinish, ServiceConstants.ReasonSapConnectionError));
                return false;
            }

            var resultMessages = JsonConvert.DeserializeObject<List<ValidationsToFinalizeProductionOrdersResultModel>>(result.Response.ToString());
            var orderValidationResult = resultMessages.FirstOrDefault(x => x.FinalizeProductionOrder.ProductionOrderId == orderToFinish.ProductionOrderId);
            if (orderValidationResult == null)
            {
                this.logger.Error(LogsConstants.ErrorOnSAPResponseNull, logBase, orderToFinish.ProductionOrderId);
                return false;
            }

            if (!string.IsNullOrEmpty(orderValidationResult.ErrorMessage))
            {
                this.logger.Error(LogsConstants.ValidationErrorOnSAP, logBase, orderToFinish.ProductionOrderId);
                failed.Add(CreateFinalizedFailedResponse(orderToFinish, orderValidationResult.ErrorMessage));
                return false;
            }

            return true;
        }

        private async Task<(ProductionOrderProcessingStatusModel, bool)> UpdateProductionOrdersOnPostgresProcess(ProductionOrderProcessingStatusModel productionOrderProcessingPayload, string logBase)
        {
            var payloadJson = JsonConvert.DeserializeObject<FinalizeProductionOrderPayload>(productionOrderProcessingPayload.Payload);
            try
            {
                this.logger.Information(
                    LogsConstants.UpdateProductionOrdersOnPostgres, logBase, JsonConvert.SerializeObject(payloadJson.FinalizeProductionOrder));
                var productionOrderId = productionOrderProcessingPayload.ProductionOrderId.ToString();
                var (salesOrders, productionOrder) = await this.GetRelatedOrdersToProducionOrder(new List<string> { productionOrderId });
                var salesOrder = salesOrders != null ? salesOrders.FirstOrDefault(x => x.IsSalesOrder) : null;
                salesOrders?.Remove(salesOrder);
                var preProductionOrders = salesOrder != null ? await ServiceUtils.GetPreProductionOrdersFromSap(salesOrder, this.sapAdapter) : (new List<CompleteDetailOrderModel>(), new List<OrderWithDetailModel>());
                var payload = payloadJson.FinalizeProductionOrder;
                var userOrdersToUpdate = new List<UserOrderModel>();
                var ordersToProcess = payload.SourceProcess == ServiceConstants.SalesOrders ? salesOrders : new List<UserOrderModel> { productionOrder };

                foreach (var userOrder in ordersToProcess)
                {
                    this.logger.Information(LogsConstants.FinalizingFabOrder, logBase, JsonConvert.SerializeObject(userOrder));

                    if (userOrder.Status.Equals(ServiceConstants.Finalizado))
                    {
                        this.logger.Information(LogsConstants.IfFinalizingFabOrder, userOrder.Status);
                        continue;
                    }

                    userOrder.CloseUserId = payload.UserId;
                    userOrder.CloseDate = DateTime.Now;
                    userOrder.Status = ServiceShared.CalculateTernary(userOrder.Status != ServiceConstants.Almacenado, ServiceConstants.Finalizado, userOrder.Status);
                    userOrder.FinalizedDate = DateTime.Now;

                    var batch = payload.Batches != null && payload.Batches.Any() ? payload.Batches.FirstOrDefault() : new BatchesConfigurationModel { BatchCode = string.Empty };
                    userOrder.BatchFinalized = batch.BatchCode;

                    userOrdersToUpdate.Add(userOrder);
                }

                var shouldUpdateSalesOrder = false;

                if (!productionOrder.IsIsolatedProductionOrder)
                {
                    var hasNoPreProductionOrders = !preProductionOrders.Item1.Any();
                    var parentFabOrders = preProductionOrders.Item2.SelectMany(x => x.Detalle).Where(detail => detail.OrderRelationType == "Y").ToList();
                    var parentOrdersSeparationDetail = await this.pedidosDao.GetProductionOrderSeparationByOrderId(parentFabOrders.Select(x => x.OrdenFabricacionId).ToList());
                    var hasPendingParentOrdersToSplit = parentOrdersSeparationDetail.Any(x => x.AvailablePieces > 0);
                    var productionOrdersCount = salesOrders.Count(x => x.IsProductionOrder);
                    var otherProductionOrdersFinalized = salesOrders
                        .Where(x => x.IsProductionOrder && x.Productionorderid != productionOrderId)
                        .All(x => ServiceConstants.ValidStatusFinalizar.Contains(x.Status));

                    shouldUpdateSalesOrder = payload.SourceProcess == ServiceConstants.SalesOrders
                        ? !hasPendingParentOrdersToSplit
                        : salesOrder != null && hasNoPreProductionOrders && (productionOrdersCount == 1 || otherProductionOrdersFinalized) && !hasPendingParentOrdersToSplit;
                }

                if (shouldUpdateSalesOrder)
                {
                    this.logger.Information(LogsConstants.FinalizingSalesOrder, logBase, salesOrder.Salesorderid);

                    salesOrder.CloseUserId = payload.UserId;
                    salesOrder.CloseDate = DateTime.Now;
                    salesOrder.Status = ServiceConstants.Finalizado;
                    salesOrder.FinalizedDate = DateTime.Now;

                    userOrdersToUpdate.Add(salesOrder);
                }

                this.logger.Information(LogsConstants.ListToUpdate, JsonConvert.SerializeObject(userOrdersToUpdate));
                await this.pedidosDao.UpdateUserOrders(userOrdersToUpdate);

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                    productionOrderProcessingPayload.Id,
                    ServiceConstants.FinalizeProcessInProgressStatus,
                    productionOrderProcessingPayload.Payload,
                    productionOrderProcessingPayload.ErrorMessage,
                    ServiceConstants.StepUpdatePostgres);

                return (productionOrderProcessingPayload, true);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndFinalizeProductionOrderOnPostgresWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        productionOrderProcessingPayload.Payload,
                        string.Format(LogsConstants.GenericErrorLog, ex.Message, ex.InnerException),
                        productionOrderProcessingPayload.LastStep);
                return (productionOrderProcessingPayload, false);
            }
        }

        private async Task<(ProductionOrderProcessingStatusModel, bool)> CreateProductionOrderPdf(ProductionOrderProcessingStatusModel productionOrderProcessingPayload, string logBase)
        {
            var payloadJson = JsonConvert.DeserializeObject<FinalizeProductionOrderPayload>(productionOrderProcessingPayload.Payload);
            try
            {
                this.logger.Information(
                    LogsConstants.StartGeneratePdfProcess, logBase, JsonConvert.SerializeObject(payloadJson.FinalizeProductionOrder));
                var productionOrderId = productionOrderProcessingPayload.ProductionOrderId.ToString();
                var (salesOrders, productionOrder) = await this.GetRelatedOrdersToProducionOrder(new List<string> { productionOrderId });

                var listSalesOrder = new List<int>();
                var listIsolated = new List<int>();

                if (salesOrders != null)
                {
                    listSalesOrder.Add(int.Parse(salesOrders.First().Salesorderid));
                }
                else
                {
                    listIsolated.Add(int.Parse(productionOrder.Productionorderid));
                }

                this.logger.Information(LogsConstants.GeneratingPdf, logBase, listSalesOrder.FirstOrDefault(), listIsolated.FirstOrDefault());

                await SendToGeneratePdfUtils.CreateModelGeneratePdf(listSalesOrder, listIsolated, this.sapAdapter, this.pedidosDao, this.sapFileService, this.userService, true);

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                    productionOrderProcessingPayload.Id,
                    ServiceConstants.FinalizeProcessInSuccessStatus,
                    productionOrderProcessingPayload.Payload,
                    productionOrderProcessingPayload.ErrorMessage,
                    ServiceConstants.StepCreatePdf);

                return (productionOrderProcessingPayload, true);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndCreatePdfWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                    productionOrderProcessingPayload.Id,
                    ServiceConstants.FinalizeProcessFailedStatus,
                    productionOrderProcessingPayload.Payload,
                    string.Format(LogsConstants.GenericErrorLog, ex.Message, ex.InnerException),
                    productionOrderProcessingPayload.LastStep);
                return (productionOrderProcessingPayload, false);
            }
        }

        private async Task<(List<UserOrderModel> salesOrder, UserOrderModel productionOrders)> GetRelatedOrdersToProducionOrder(List<string> productionOrderIds)
        {
            var productionOrder = (await this.pedidosDao.GetUserOrderByProducionOrder(productionOrderIds)).FirstOrDefault(x => x.IsProductionOrder);
            var salesOrdersId = !string.IsNullOrEmpty(productionOrder.Salesorderid) ? productionOrder.Salesorderid : null;

            if (salesOrdersId == null)
            {
                return (null, productionOrder);
            }

            var salesOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { salesOrdersId })).Where(x => x.Status != ServiceConstants.Cancelled).ToList();

            return (salesOrders, productionOrder);
        }

        private async Task DeleteRedisControlKeyToFinalizeProductionOrder(int productionOrderId)
        {
            var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrderId);
            await this.redisService.DeleteKey(redisKey);
        }

        private async Task UpdateProductionOrdersToRetryStatusInProgress(RetryFailedProductionOrderFinalizationDto payloadRetry)
        {
            var productionOrdersToRetry = payloadRetry.ProductionOrderProcessingPayload;
            var productionOrdersDB = (await this.pedidosDao.GetProductionOrderProcessingStatusByProductionOrderIds(productionOrdersToRetry.Select(x => x.ProductionOrderId))).ToList();

            productionOrdersDB.ForEach(x =>
            {
                x.LastUpdated = DateTime.Now;
                x.Status = ServiceConstants.FinalizeProcessInProgressStatus;
            });

            await this.pedidosDao.UpdatesProductionOrderProcessingStatus(productionOrdersDB);
        }

        private async Task SendMessageToKafka(
            ProductionOrderProcessingStatusModel productionOrderProcessing,
            string queueType,
            string logBase)
        {
            var isSuccesfully = await this.kafkaConnector.PushMessage(productionOrderProcessing, queueType, logBase);
            if (!isSuccesfully)
            {
                await this.DeleteRedisControlKeyToFinalizeProductionOrder(productionOrderProcessing.ProductionOrderId);
            }
        }
    }
}
