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
    using System.Linq;
    using System.Net;
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

                _ = Task.Run(() => this.SendKafkaMessagesAsync(productionOrderProcessingStatus));

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
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrderUpdated.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, null, null, null);
            }

            this.logger.Information(LogsConstants.SendKafkaMessageFinalizeProductionOrderPostgresql, logBase, JsonConvert.SerializeObject(productionOrderUpdated));
            await this.kafkaConnector.PushMessage(
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
            _ = this.pedidosDao.UpdatesProductionOrderProcessingStatus([productionOrderUpdated]);

            if (!isProcessSuccesfully)
            {
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrderUpdated.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, null, null, null);
            }

            await this.kafkaConnector.PushMessage(
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
            _ = this.pedidosDao.UpdatesProductionOrderProcessingStatus([productionOrderUpdated]);

            var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrderUpdated.ProductionOrderId);
            await this.redisService.DeleteKey(redisKey);

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
            await this.InsertOnRedisKeysToProductionOrdersToRetry(payloadRetry.ProductionOrderProcessingPayload);

            foreach (var payloadRequest in payloadRetry.ProductionOrderProcessingPayload)
            {
                var payload = this.mapper.Map<ProductionOrderProcessingStatusModel>(payloadRequest);
                await this.RetryFailedProductionOrderFinalizationProcess(payload, logBase);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> SeparateOrder(SeparateOrderDto request)
        {
            await this.SeparateProductionOrderProcessAsync(request.OrderId, request.Pieces);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
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

        private async Task SeparateProductionOrderProcessAsync(int productionOrderId, int pieces)
        {
            var separationId = Guid.NewGuid().ToString();
            var command = new StartProductionOrderSeparationCommand(productionOrderId, pieces, separationId);
            await this.mediator.Send(command);
        }

        private async Task InsertOnRedisKeysToProductionOrdersToRetry(List<ProductionOrderProcessingStatusDto> productionOrders)
        {
            foreach (var po in productionOrders)
            {
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, po.ProductionOrderId);
                await this.redisService.WriteToRedis(
                    redisKey,
                    JsonConvert.SerializeObject(po),
                    ServiceConstants.DefaultRedisValueTimeToLive);
            }
        }

        private async Task<ProductionOrderProcessingStatusModel> UpdateProcessStatus(
            string id,
            string status,
            string errorMessage = null,
            string lastStep = null)
        {
            var modelToUpdate = await this.pedidosDao.GetFirstProductionOrderProcessingStatusById(id);
            modelToUpdate.Status = status;
            modelToUpdate.ErrorMessage = errorMessage;
            modelToUpdate.LastStep = lastStep;
            modelToUpdate.LastUpdated = DateTime.Now;
            await this.pedidosDao.UpdatesProductionOrderProcessingStatus([modelToUpdate]);
            return modelToUpdate;
        }

        private async Task SendKafkaMessagesAsync(List<ProductionOrderProcessingStatusModel> productionOrderProcessingStatusList)
        {
            foreach (var productionOrderProcessing in productionOrderProcessingStatusList)
            {
                var logBase = string.Format(LogsConstants.FinalizeProductionOrdersAsync, productionOrderProcessing.Id);
                this.logger.Information(LogsConstants.SendKafkaMessageFinalizeProductionOrderSap, logBase, JsonConvert.SerializeObject(productionOrderProcessing));
                await this.kafkaConnector.PushMessage(productionOrderProcessing, ServiceConstants.KafkaFinalizeProductionOrderSapConfigName, logBase);
            }
        }

        private async Task RetryFailedProductionOrderFinalizationProcess(ProductionOrderProcessingStatusModel payload, string logBase)
        {
            try
            {
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, payload.ProductionOrderId);
                await this.redisService.WriteToRedis(redisKey, JsonConvert.SerializeObject(payload), new TimeSpan(12, 0, 0));

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
                            Batches = payload.FinalizeProductionOrder.Batches,
                            ProductionOrderId = payload.FinalizeProductionOrder.ProductionOrderId,
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
                        ServiceConstants.ErrorOccurredWhileCommunicatingWithServiceLayerAdapter,
                        productionOrderProcessingPayload.LastStep);
                    return (productionOrderProcessingPayload, false);
                }

                var resultMessages = JsonConvert.DeserializeObject<List<ValidationsToFinalizeProductionOrdersResultModel>>(result.Response.ToString());
                var finalizeProcessResult = resultMessages.First(x => x.ProductionOrderId == productionOrderProcessingPayload.ProductionOrderId);

                if (!string.IsNullOrEmpty(finalizeProcessResult.ErrorMessage))
                {
                    this.logger.Error(LogsConstants.ErrorInSAPWhileFinalizingTheOrder, logBase, productionOrderProcessingPayload.ProductionOrderId);
                    productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        finalizeProcessResult.ErrorMessage,
                        finalizeProcessResult.LastStep);
                    return (productionOrderProcessingPayload, false);
                }

                productionOrderProcessingPayload = await this.UpdateProcessStatus(
                        productionOrderProcessingPayload.Id,
                        ServiceConstants.FinalizeProcessInProgressStatus,
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
                        Batches = orderToFinish.Batches,
                        ProductionOrderId = orderToFinish.ProductionOrderId,
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
            var orderValidationResult = resultMessages.FirstOrDefault(x => x.ProductionOrderId == orderToFinish.ProductionOrderId);
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
            var productionOrderProcessingStatusInBD = await this.pedidosDao.GetFirstProductionOrderProcessingStatusByProductionOrderId(productionOrderProcessingPayload.ProductionOrderId);
            var payloadJson = JsonConvert.DeserializeObject<FinalizeProductionOrderPayload>(productionOrderProcessingPayload.Payload);
            try
            {
                this.logger.Information(
                    LogsConstants.UpdateProductionOrdersOnPostgres, logBase, JsonConvert.SerializeObject(payloadJson.FinalizeProductionOrder));
                var productionOrderId = productionOrderProcessingPayload.ProductionOrderId.ToString();
                var (salesOrders, productionOrder) = await this.GetRelatedOrdersToProducionOrder(new List<string> { productionOrderId });
                var salesOrder = salesOrders != null ? salesOrders.FirstOrDefault(x => x.IsSalesOrder) : null;
                salesOrders?.Remove(salesOrder);
                var preProductionOrders = salesOrder != null ? await ServiceUtils.GetPreProductionOrdersFromSap(salesOrder, this.sapAdapter) : new List<CompleteDetailOrderModel>();
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
                    var hasNoPreProductionOrders = !preProductionOrders.Any();
                    var productionOrdersCount = salesOrders.Count(x => x.IsProductionOrder);
                    var otherProductionOrdersFinalized = salesOrders
                        .Where(x => x.IsProductionOrder && x.Productionorderid != productionOrderId)
                        .All(x => ServiceConstants.ValidStatusFinalizar.Contains(x.Status));

                    shouldUpdateSalesOrder = payload.SourceProcess == ServiceConstants.SalesOrders
                        ? true
                        : salesOrder != null && hasNoPreProductionOrders && (productionOrdersCount == 1 || otherProductionOrdersFinalized);
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

                productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                    productionOrderProcessingStatusInBD,
                    ServiceConstants.FinalizeProcessInProgressStatus,
                    string.Empty,
                    ServiceConstants.StepUpdatePostgres);

                return (productionOrderProcessingStatusInBD, true);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndFinalizeProductionOrderOnPostgresWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);

                productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                    productionOrderProcessingStatusInBD,
                    ServiceConstants.FinalizeProcessFailedStatus,
                    string.Format(LogsConstants.GenericErrorLog, ex.Message, ex.InnerException),
                    productionOrderProcessingStatusInBD.LastStep);

                return (productionOrderProcessingStatusInBD, false);
            }
        }

        private async Task<(ProductionOrderProcessingStatusModel, bool)> CreateProductionOrderPdf(ProductionOrderProcessingStatusModel productionOrderProcessingPayload, string logBase)
        {
            var productionOrderProcessingStatusInBD = await this.pedidosDao.GetFirstProductionOrderProcessingStatusByProductionOrderId(productionOrderProcessingPayload.ProductionOrderId);
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

                productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                    productionOrderProcessingStatusInBD,
                    ServiceConstants.FinalizeProcessInSuccessStatus,
                    string.Empty,
                    ServiceConstants.StepCreatePdf);

                return (productionOrderProcessingStatusInBD, true);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndCreatePdfWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);

                productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                    productionOrderProcessingStatusInBD,
                    ServiceConstants.FinalizeProcessFailedStatus,
                    string.Format(LogsConstants.GenericErrorLog, ex.Message, ex.InnerException),
                    productionOrderProcessingStatusInBD.LastStep);

                return (productionOrderProcessingStatusInBD, false);
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

            var salesOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(new List<string> { salesOrdersId })).ToList();

            return (salesOrders, productionOrder);
        }
    }
}
