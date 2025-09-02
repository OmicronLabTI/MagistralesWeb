// <summary>
// <copyright file="OrderDivisionProcessHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Handlers.Impl
{
    /// <summary>
    /// OrderDivisionProcessHandler.
    /// </summary>
    public class OrderDivisionProcessHandler : IOrderDivisionProcessHandler
    {
        private readonly ILogger logger;
        private readonly IPedidosService pedidosService;
        private readonly IRedisService redisService;
        private readonly Settings settings;
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDivisionProcessHandler"/> class.
        /// </summary>
        /// <param name="logger">LoggerService.</param>
        /// <param name="pedidosService">PedidosService.</param>
        /// <param name="redisService">redisService.</param>
        /// <param name="settings">Settings.</param>
        /// <param name="serviceScopeFactory">IServiceScopeFactory.</param>
        public OrderDivisionProcessHandler(
          ILogger logger,
          IPedidosService pedidosService,
          IRedisService redisService,
          IOptionsSnapshot<Settings> settings,
          IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.pedidosService = pedidosService;
            this.redisService = redisService;
            this.settings = settings.Value;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        /// <inheritdoc />
        public async Task Handle()
        {
            var batchProcessId = Guid.NewGuid().ToString();
            var logBase = string.Format(BatchConstants.DivisionOrdersCronJob, batchProcessId);

            try
            {
                this.logger.Information(BatchConstants.StartCronJobProcess, logBase);
                var deleteRedisKey = await this.RetryDivisionOrders(batchProcessId, logBase);
                if (deleteRedisKey)
                {
                    await this.redisService.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob);
                    await this.redisService.DeleteKey(BatchConstants.DivisionOrdersToProcessKey);
                }

                this.logger.Information(BatchConstants.EndCronJobProcess, logBase);
            }
            catch (Exception ex)
            {
                var error = string.Format(BatchConstants.EndCronJobProcessWithError, logBase, ex.Message, ex.InnerException);
                this.logger.Error(ex, error);
                await this.redisService.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob);
                await this.redisService.DeleteKey(BatchConstants.DivisionOrdersToProcessKey);
            }
        }

        private async Task<bool> RetryDivisionOrders(string batchProcessId, string logBase)
        {
            var existingValue = await this.redisService.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob);
            if (!string.IsNullOrEmpty(existingValue))
            {
                this.logger.Information(BatchConstants.CronJobProcessAlreadyRunning, logBase);
                return false;
            }

            await this.redisService.WriteToRedis(
                BatchConstants.DivisionOrdersRetryCronjob,
                DateTime.Now.ToString(),
                BatchConstants.DefaultRedisValueTimeToLive);

            var result = await this.pedidosService.GetAsync(
                BatchConstants.GetFailedDivisionOrdersEndpoint,
                logBase);

            if (!result.Success)
            {
                this.logger.Error(BatchConstants.ErrorRetrievingDivisionOrders, logBase);
                return true;
            }

            var ordersToProcess = Convert.ToInt32(result.Response);
            if (ordersToProcess == 0)
            {
                this.logger.Information(BatchConstants.ThereAreNoDivisionOrdersToProcess, logBase);
                return true;
            }

            this.logger.Information(BatchConstants.NumberOfDivisionOrdersToProcess, logBase, ordersToProcess);

            var batchRange = CommonFunctions.GenerateSkipTakeBatches(ordersToProcess, this.settings.BatchSize);
            this.logger.Information(BatchConstants.BatchRangeCount, logBase, batchRange.Count());

            foreach (var batch in batchRange)
            {
                await this.SendToRetryProcess(batch.Offset, batch.Limit, batchProcessId, logBase);
            }

            return true;
        }

        private async Task SendToRetryProcess(int offset, int limit, string batchProcessId, string logBase)
        {
            this.logger.Information(BatchConstants.DivisionOrdersAreRetrievedFromRedis, logBase, offset, limit);

            var paginatedList = await this.GetValueFromRedisByKeyAndOffsetAndLimit<ProductionOrderSeparationDetailLogModel>(
                BatchConstants.DivisionOrdersToProcessKey,
                offset,
                limit);

            this.logger.Information(BatchConstants.StartDivisionOrdersAreSentForRetry, logBase, JsonConvert.SerializeObject(paginatedList));

            var requestPedidos = new RetryFailedOrderDivisionModel
            {
                BatchProcessId = batchProcessId,
                DivisionOrdersPayload = paginatedList,
            };

            this.SendPedidosRequestInBackgroundAsync(requestPedidos, logBase);
            this.logger.Information(BatchConstants.EndDivisionOrdersAreSentForRetry, logBase);
        }

        private void SendPedidosRequestInBackgroundAsync(
            RetryFailedOrderDivisionModel requestPedidos,
            string logBase)
        {
            _ = Task.Run(async () =>
            {
                using var scope = this.serviceScopeFactory.CreateScope();
                var scopedPedidosService = scope.ServiceProvider.GetRequiredService<IPedidosService>();

                try
                {
                    await scopedPedidosService.PostAsync(
                        BatchConstants.PostRetryDivisionFailedProductionOrdersEndpoint,
                        requestPedidos,
                        logBase).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var error = string.Format(BatchConstants.ErrorSendPedidosRequestInBackgroundAsync, logBase, ex.Message, ex.InnerException);
                }
            });
        }

        private async Task<List<T>> GetValueFromRedisByKeyAndOffsetAndLimit<T>(string key, int offset, int limit)
        {
            var page = await this.redisService.ReadListAsync<T>(key, offset, limit);
            if (page.Count < limit)
            {
                await this.redisService.DeleteKey(key);
            }

            return page;
        }
    }
}
