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
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.ProductionOrders;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrdersService"/> class.
        /// </summary>
        /// <param name="pedidosDao">Interface for dao.</param>
        /// <param name="serviceLayerAdapterService">Interface for Service Layer Adapter Service.</param>
        /// <param name="redisService">Interface for Redis Service.</param>
        /// <param name="kafkaConnector">Interface for Kafka Connector.</param>
        /// <param name="logger">Logger.</param>
        public ProductionOrdersService(
            IPedidosDao pedidosDao,
            ISapServiceLayerAdapterService serviceLayerAdapterService,
            IRedisService redisService,
            IKafkaConnector kafkaConnector,
            ILogger logger)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
            this.serviceLayerAdapterService = serviceLayerAdapterService.ThrowIfNull(nameof(serviceLayerAdapterService));
            this.redisService = redisService.ThrowIfNull(nameof(redisService));
            this.kafkaConnector = kafkaConnector.ThrowIfNull(nameof(kafkaConnector));
            this.logger = logger.ThrowIfNull(nameof(logger));
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
                    var (isValidProductionOrder, deleteRedisKey) = await this.ValidatePrimaryRules(
                        productionOrder,
                        failed,
                        redisKey,
                        logBase,
                        processId);

                    if (deleteRedisKey)
                    {
                        await this.redisService.DeleteKey(redisKey);
                    }

                    if (!isValidProductionOrder)
                    {
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
                    this.logger.Information(LogsConstants.SendKafkaMessageFinalizeProductionOrderSap, logBase, JsonConvert.SerializeObject(productionOrderProcessing));
                    _ = this.kafkaConnector.PushMessage(productionOrderProcessing, ServiceConstants.KafkaFinalizeProductionOrderSapConfigName);
                }

                this.logger.Information(LogsConstants.InsertAllProductionOrderProcessingStatus, JsonConvert.SerializeObject(productionOrderProcessingStatus));
                _ = this.pedidosDao.InsertProductionOrderProcessingStatus(productionOrderProcessingStatus);

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
            _ = this.pedidosDao.UpdatesProductionOrderProcessingStatus([productionOrderUpdated]);

            if (!isProcessSuccesfully)
            {
                var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrderUpdated.ProductionOrderId);
                await this.redisService.DeleteKey(redisKey);
                return ServiceUtils.CreateResult(false, (int)HttpStatusCode.InternalServerError, null, null, null);
            }

            this.logger.Information(LogsConstants.SendKafkaMessageFinalizeProductionOrderPostgresql, logBase, JsonConvert.SerializeObject(productionOrderUpdated));
            _ = this.kafkaConnector.PushMessage(productionOrderUpdated, ServiceConstants.KafkaFinalizeProductionOrderPostgresqlConfigName);
            this.logger.Information(LogsConstants.EndFinalizeProductionOrderInSap, JsonConvert.SerializeObject(productionOrderProcessingPayload));
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrdersOnPostgresqlAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
            // _ = this.kafkaConnector.PushMessage([], ServiceConstants.KafkaProductionOrderPdfGenerationConfigName);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<ResultModel> ProductionOrderPdfGenerationAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
            throw new NotImplementedException();
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

        private async Task<(ProductionOrderProcessingStatusModel, bool)> FinalizeProductionOrdersOnSapProcess(
            ProductionOrderProcessingStatusModel productionOrderProcessingPayload,
            string logBase)
        {
            var payload = JsonConvert.DeserializeObject<FinalizeProductionOrderPayload>(productionOrderProcessingPayload.Payload);
            var productionOrderProcessingStatusInBD = await this.pedidosDao.GetFirstProductionOrderProcessingStatusByProductionOrderId(productionOrderProcessingPayload.ProductionOrderId);
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
                    ServiceConstants.SapValidationFinalizationEndpoint);

                if (!result.Success)
                {
                    this.logger.Error(LogsConstants.ErrorOnSAP, logBase, payload.FinalizeProductionOrder.ProductionOrderId);
                    productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                        productionOrderProcessingStatusInBD,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        ServiceConstants.ErrorOccurredWhileCommunicatingWithServiceLayerAdapter);

                    return (productionOrderProcessingStatusInBD, false);
                }

                var resultMessages = JsonConvert.DeserializeObject<List<ValidationsToFinalizeProductionOrdersResultModel>>(result.Response.ToString());
                var finalizeProcessResult = resultMessages.First(x => x.ProductionOrderId == productionOrderProcessingPayload.ProductionOrderId);

                if (!string.IsNullOrEmpty(finalizeProcessResult.ErrorMessage))
                {
                    this.logger.Error(LogsConstants.ErrorInSAPWhileFinalizingTheOrder, logBase, productionOrderProcessingPayload.ProductionOrderId);
                    productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                        productionOrderProcessingStatusInBD,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        finalizeProcessResult.ErrorMessage,
                        finalizeProcessResult.LastStep);

                    return (productionOrderProcessingStatusInBD, false);
                }

                productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                        productionOrderProcessingStatusInBD,
                        ServiceConstants.FinalizeProcessInProgressStatus,
                        productionOrderProcessingStatusInBD.ErrorMessage,
                        finalizeProcessResult.LastStep);

                return (productionOrderProcessingStatusInBD, true);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.EndFinalizeProductionOrderOnSapWithError, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);

                productionOrderProcessingStatusInBD = UpdateProcessingStatusAsync(
                        productionOrderProcessingStatusInBD,
                        ServiceConstants.FinalizeProcessFailedStatus,
                        string.Format(LogsConstants.GenericErrorLog, ex.Message, ex.InnerException),
                        productionOrderProcessingStatusInBD.LastStep);

                return (productionOrderProcessingStatusInBD, false);
            }
        }

        private async Task<(bool, bool)> ValidatePrimaryRules(
            FinalizeProductionOrderModel orderToFinish,
            List<ProductionOrderFailedResultModel> failed,
            string redisKey,
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
                return (false, true);
            }

            var resultMessages = JsonConvert.DeserializeObject<List<ValidationsToFinalizeProductionOrdersResultModel>>(result.Response.ToString());
            var orderValidationResult = resultMessages.FirstOrDefault(x => x.ProductionOrderId == orderToFinish.ProductionOrderId);
            if (orderValidationResult == null)
            {
                this.logger.Error(LogsConstants.ErrorOnSAPResponseNull, logBase, orderToFinish.ProductionOrderId);
                return (false, true);
            }

            if (!string.IsNullOrEmpty(orderValidationResult.ErrorMessage))
            {
                this.logger.Error(LogsConstants.ValidationErrorOnSAP, logBase, orderToFinish.ProductionOrderId);
                failed.Add(CreateFinalizedFailedResponse(orderToFinish, orderValidationResult.ErrorMessage));
                return (false, true);
            }

            return (true, false);
        }

        private async Task<bool> CheckOrSetProductionOrderFinalizing(string redisKey, string value, ProductionOrderProcessingStatusModel productionOrderInBD)
        {
            var existingValue = await this.redisService.GetRedisKey(redisKey);
            var existsInRedis = !string.IsNullOrEmpty(existingValue);

            var productionOrderExistsInDatabase = productionOrderInBD != null;
            var isProductionOrderFinalizing = ServiceShared.CalculateOr(existsInRedis, productionOrderExistsInDatabase);
            return false;
        }
    }
}
