// <summary>
// <copyright file="FinalizeProductionOrderHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Batch.Handlers.Impl
{
    /// <summary>
    /// FinalizeProductionOrderHandler class.
    /// </summary>
    public class FinalizeProductionOrderHandler : IFinalizeProductionOrderHandler
    {
        private readonly ILogger logger;

        private readonly IPedidosService pedidosService;

        private readonly IRedisService redisService;

        private readonly Settings settings;

        private readonly IKafkaConnector kafkaConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalizeProductionOrderHandler"/> class.
        /// </summary>
        /// <param name="logger">LoggerService.</param>
        /// <param name="pedidosService">PedidosService</param>
        /// <param name="redisService">redisService</param>
        /// <param name="settings">Settings.</param>
        /// <param name="kafkaConnector">Kafka Connector.</param>
        public FinalizeProductionOrderHandler(
            ILogger logger,
            IPedidosService pedidosService,
            IRedisService redisService,
            IOptionsSnapshot<Settings> settings,
            IKafkaConnector kafkaConnector)
        {
            this.logger = logger;
            this.pedidosService = pedidosService;
            this.redisService = redisService;
            this.settings = settings.Value;
            this.kafkaConnector = kafkaConnector;
        }

        /// <inheritdoc />
        public async Task Handle()
        {
            var batchProcessId = Guid.NewGuid().ToString();
            var logBase = string.Format(BatchConstants.FinalizeProductionOrdersCronJob, batchProcessId);

            try
            {
                this.logger.Information(BatchConstants.StartCronJobProcess, logBase);
                await this.RetryFinalizeProductionOrder(batchProcessId, logBase);
                this.logger.Information(BatchConstants.EndCronJobProcess, logBase);
            }
            catch (Exception ex)
            {
                var error = string.Format(BatchConstants.EndCronJobProcessWithError, logBase, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);
                await this.DeleteCronJobProcessRedisKeys();
            }
        }

        private async Task RetryFinalizeProductionOrder(string batchProcessId, string logBase)
        {
            var existingValue = await this.redisService.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob);
            if (!string.IsNullOrEmpty(existingValue))
            {
                this.logger.Information(BatchConstants.CronJobProcessAlreadyRunning, logBase);
                await this.DeleteCronJobProcessRedisKeys();
            }

            await this.redisService.WriteToRedis(
                BatchConstants.ProductionOrderFinalizingRetryCronjob,
                DateTime.Now.ToString(),
                BatchConstants.DefaultRedisValueTimeToLive);

            var result = await this.pedidosService.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                logBase);

            if (!result.Success)
            {
                this.logger.Error(BatchConstants.ErrorRetrievingProductionOrders, logBase);
                await this.DeleteCronJobProcessRedisKeys();
            }

            var ordersToProcess = Convert.ToInt32(result.Response);
            if (ordersToProcess == 0)
            {
                this.logger.Information(BatchConstants.ThereAreNoProductionOrdersToProcess, logBase);
                await this.DeleteCronJobProcessRedisKeys();
            }

            this.logger.Information(BatchConstants.NumberOfProductionOrdersToProcess, logBase, ordersToProcess);
            var batchRange = CommonFunctions.GenerateSkipTakeBatches(ordersToProcess, this.settings.BatchSize);

            this.logger.Information(BatchConstants.BatchRangeCount, logBase, batchRange.Count());
            foreach (var batch in batchRange)
            {
                await this.SendToRetryProcess(batch.Offset, batch.Limit, batchProcessId, logBase);
            }

            await this.DeleteCronJobProcessRedisKeys();
        }

        private async Task SendToRetryProcess(int offset, int limit, string batchProcessId, string logBase)
        {
            this.logger.Information(BatchConstants.ProductionOrdersAreRetrievedFromRedis, logBase, offset, limit);
            var paginatedList = await this.GetValueFromRedisByKeyAndOffsetAndLimit<ProductionOrderProcessingStatusModel>(
                BatchConstants.ProductionOrderFinalizingToProcessKey,
                offset,
                limit);

            this.logger.Information(
                BatchConstants.StartProductionOrdersAreSentForRetry,
                logBase,
                JsonConvert.SerializeObject(paginatedList));

            var requestPedidos = new RetryFailedProductionOrderFinalizationModel
            {
                BatchProcessId = batchProcessId,
                ProductionOrderProcessingPayload = paginatedList,
            };

            await this.kafkaConnector.PushMessage(requestPedidos, logBase);
            this.logger.Information(BatchConstants.EndProductionOrdersAreSentForRetry, logBase);
        }

        private async Task<List<T>> GetValueFromRedisByKeyAndOffsetAndLimit<T>(string key, int offset, int limit)
        {
            List<T> paginatedList = await this.redisService.ReadListAsync<T>(key, offset, limit);
            return paginatedList;
        }

        private async Task DeleteCronJobProcessRedisKeys()
        {
            await this.redisService.DeleteKey(BatchConstants.ProductionOrderFinalizingRetryCronjob);
            await this.redisService.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey);
        }
    }
}
