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
                this.logger.Information("Pedidos Service - Start Finalize All Production Orders - {ProductionOrdersToFinalize}", JsonConvert.SerializeObject(productionOrdersToFinalize));
                var productionOrderProcessingStatusInBD = await this.pedidosDao.GetProductionOrderProcessingStatusByProductionOrderIds(productionOrdersToFinalize.Select(po => po.ProductionOrderId));
                var productionOrderProcessingStatus = new List<ProductionOrderProcessingStatusModel>();
                foreach (var productionOrder in productionOrdersToFinalize)
                {
                    var processId = Guid.NewGuid().ToString();
                    var logBase = string.Format("{0} - Pedidos Service - Finalize Production Orders Async", processId);
                    var redisKey = string.Format(ServiceConstants.ProductionOrderFinalizingKey, productionOrder.ProductionOrderId);
                    var productionOrderInBD = productionOrderProcessingStatusInBD.FirstOrDefault(x => x.ProductionOrderId == productionOrder.ProductionOrderId);
                    this.logger.Information("{LogBase} - Start Validate Primary Rules - {ProductionOrderId}", logBase, productionOrder.ProductionOrderId);
                    var (isValidProductionOrder, deleteRedisKey) = await this.ValidatePrimaryRules(
                        productionOrder,
                        productionOrderInBD,
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
                    this.logger.Information("{LogBase} - Send Kafka Message Finalize Production Order Sap - {ProductionOrderProcessing}", logBase, JsonConvert.SerializeObject(productionOrderProcessing));
                    _ = this.kafkaConnector.PushMessage(productionOrderProcessing, ServiceConstants.KafkaFinalizeProductionOrderSapConfigName);
                }

                this.logger.Information("Pedidos Service - Insert All ProductionOrder Processing Status - {ProductionOrderProcessingStatus}", JsonConvert.SerializeObject(productionOrderProcessingStatus));
                _ = this.pedidosDao.InsertProductionOrderProcessingStatus(productionOrderProcessingStatus);

                var validationsResult = new
                {
                    success = successfuly.Distinct(),
                    failed = failed.Distinct(),
                };

                this.logger.Information("Pedidos Service - End Finalize All Production Orders - {ProductionOrdersToFinalize}", JsonConvert.SerializeObject(productionOrdersToFinalize));
                return ServiceUtils.CreateResult(true, 200, null, validationsResult, null);
            }
            catch (Exception ex)
            {
                var error = string.Format("Pedidos Service - End Finalize All Production Orders With Error - Message Exception: {0} - Inner Exception: {1}", ex.Message, ex.InnerException);
                this.logger.Error(ex, error);
                return ServiceUtils.CreateResult(false, 400, "Internal Error", null, null);
            }
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrdersOnSapAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<ResultModel> FinalizeProductionOrdersOnPostgresqlAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload)
        {
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
            string step,
            string status,
            string payload,
            DateTime createdAt)
        {
            return new ProductionOrderProcessingStatusModel
            {
                Id = processId,
                ProductionOrderId = productionOrderId,
                Step = step,
                Status = status,
                Payload = payload,
                CreatedAt = createdAt,
                LastUpdated = DateTime.Now,
            };
        }

        private async Task<(bool, bool)> ValidatePrimaryRules(
            FinalizeProductionOrderModel orderToFinish,
            ProductionOrderProcessingStatusModel productionOrderInBD,
            List<ProductionOrderFailedResultModel> failed,
            string redisKey,
            string logBase,
            string processId)
        {
            var isProductionOrderFinalizing = await this.CheckOrSetProductionOrderFinalizing(redisKey, JsonConvert.SerializeObject(orderToFinish), productionOrderInBD);
            if (isProductionOrderFinalizing)
            {
                this.logger.Error("{LogBase} - ProductionOrderIs Already Beign Processed - {ProductionOrderId}", logBase, orderToFinish.ProductionOrderId);
                failed.Add(CreateFinalizedFailedResponse(orderToFinish, ServiceConstants.ProductionOrderIsAlreadyBeignProcessed));
                return (false, false);
            }

            this.logger.Information("{LogBase} - Post Sap Validation Finalization - {OrderToFinish}", logBase, JsonConvert.SerializeObject(orderToFinish));
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
                this.logger.Error("{LogBase} - Error On SAP - {ProductionOrderId}", logBase, orderToFinish.ProductionOrderId);
                failed.Add(CreateFinalizedFailedResponse(orderToFinish, ServiceConstants.ReasonSapConnectionError));
                return (false, true);
            }

            var resultMessages = JsonConvert.DeserializeObject<List<ValidationsToFinalizeProductionOrdersResultModel>>(result.Response.ToString());
            var orderValidationResult = resultMessages.FirstOrDefault(x => x.ProductionOrderId == orderToFinish.ProductionOrderId);
            if (orderValidationResult == null)
            {
                this.logger.Error("{LogBase} - Error On SAP Response Null - {ProductionOrderId}", logBase, orderToFinish.ProductionOrderId);
                return (false, true);
            }

            if (!string.IsNullOrEmpty(orderValidationResult.ErrorMessage))
            {
                this.logger.Error("{LogBase} - Validation Error On SAP - {ProductionOrderId}", logBase, orderToFinish.ProductionOrderId);
                failed.Add(CreateFinalizedFailedResponse(orderToFinish, orderValidationResult.ErrorMessage));
                return (false, true);
            }

            return (true, false);
        }

        private async Task<bool> CheckOrSetProductionOrderFinalizing(string redisKey, string value, ProductionOrderProcessingStatusModel productionOrderInBD)
        {
            var existingValue = await this.redisService.GetRedisKey(redisKey);
            var existsInRedis = !string.IsNullOrEmpty(existingValue);

            if (!existsInRedis)
            {
                await this.redisService.WriteToRedis(redisKey, value, new TimeSpan(12, 0, 0));
            }

            var productionOrderExistsInDatabase = productionOrderInBD != null;
            var isProductionOrderFinalizing = ServiceShared.CalculateOr(existsInRedis, productionOrderExistsInDatabase);
            return isProductionOrderFinalizing;
        }
    }
}
