// <summary>
// <copyright file="BatchConstants.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Constants
{
    /// <summary>
    /// Constants.
    /// </summary>
    public static class BatchConstants
    {
        /// <summary>
        /// Gets ProductionOrderFinalizingRetryCronjob.
        /// </summary>
        /// <value>
        /// ProductionOrderFinalizingRetryCronjob.
        /// </value>
        public static string ProductionOrderFinalizingRetryCronjob => "production-order-finalizing-retry-cronjob";

        /// <summary>
        /// Gets ProductionOrderFinalizingToProcessKey.
        /// </summary>
        /// <value>
        /// ProductionOrderFinalizingToProcessKey.
        /// </value>
        public static string ProductionOrderFinalizingToProcessKey => "production-order-finalizing-to-process";

        /// <summary>
        /// Gets GetFailedProductionOrdersEndpoint.
        /// </summary>
        /// <value>
        /// GetFailedProductionOrdersEndpoint.
        /// </value>
        public static string GetFailedProductionOrdersEndpoint => "failedproductionorders";

        /// <summary>
        /// Gets PostRetryFinalizeFailedProductionOrdersEndpoint.
        /// </summary>
        /// <value>
        /// PostRetryFinalizeFailedProductionOrdersEndpoint.
        /// </value>
        public static string PostRetryFinalizeFailedProductionOrdersEndpoint => "retry/finalize/failed/productionorders";

        /// <summary>
        /// Gets StringContentMediaType.
        /// </summary>
        /// <value>
        /// StringContentMediaType.
        /// </value>
        public static string StringContentMediaType => "application/json";

        /// <summary>
        /// Gets DefaultRedisValueTimeToLive.
        /// </summary>
        /// <value>
        /// DefaultRedisValueTimeToLive.
        /// </value>
        public static TimeSpan DefaultRedisValueTimeToLive => new TimeSpan(8, 0, 0);

        /// <summary>
        /// Gets FinalizeProductionOrdersCronJob.
        /// </summary>
        /// <value>
        /// FinalizeProductionOrdersCronJob.
        /// </value>
        public static string FinalizeProductionOrdersCronJob => "{0} - Finalize Production Order Cron Job";

        /// <summary>
        /// Gets StartCronJobProcess.
        /// </summary>
        /// <value>
        /// StartCronJobProcess.
        /// </value>
        public static string StartCronJobProcess => "{LogBase} - Start Cron Job Process";

        /// <summary>
        /// Gets CronJobProcessAlreadyRunning.
        /// </summary>
        /// <value>
        /// CronJobProcessAlreadyRunning.
        /// </value>
        public static string CronJobProcessAlreadyRunning => "{LogBase} - Cron Job Is Already Running";

        /// <summary>
        /// Gets ErrorRetrievingProductionOrders.
        /// </summary>
        /// <value>
        /// ErrorRetrievingProductionOrders.
        /// </value>
        public static string ErrorRetrievingProductionOrders => "{LogBase} - Error Retrieving Production Orders";

        /// <summary>
        /// Gets ThereAreNoProductionOrdersToProcess.
        /// </summary>
        /// <value>
        /// ThereAreNoProductionOrdersToProcess.
        /// </value>
        public static string ThereAreNoProductionOrdersToProcess => "{LogBase} - There Are No Production Orders To Process";

        /// <summary>
        /// Gets NumberOfProductionOrdersToProcess.
        /// </summary>
        /// <value>
        /// NumberOfProductionOrdersToProcess.
        /// </value>
        public static string NumberOfProductionOrdersToProcess => "{LogBase} - Number Of Production Orders To Process: {ProductionOrdersCount}";

        /// <summary>
        /// Gets BatchRangeCount.
        /// </summary>
        /// <value>
        /// BatchRangeCount.
        /// </value>
        public static string BatchRangeCount => "{LogBase} - Batch Range Count: {BatchCount}";

        /// <summary>
        /// Gets ProductionOrdersAreRetrievedFromRedis.
        /// </summary>
        /// <value>
        /// ProductionOrdersAreRetrievedFromRedis.
        /// </value>
        public static string ProductionOrdersAreRetrievedFromRedis => "{LogBase} - Production Orders Are Retrieved From Redis - Offset: {Offset} - Limit: {Limit}";

        /// <summary>
        /// Gets StartProductionOrdersAreSentForRetry.
        /// </summary>
        /// <value>
        /// StartProductionOrdersAreSentForRetry.
        /// </value>
        public static string StartProductionOrdersAreSentForRetry => "{LogBase} - Start - Production Orders Are Sent For Retry - {Object}";

        /// <summary>
        /// Gets EndProductionOrdersAreSentForRetry.
        /// </summary>
        /// <value>
        /// EndProductionOrdersAreSentForRetry.
        /// </value>
        public static string EndProductionOrdersAreSentForRetry => "{LogBase} - End - Production Orders Are Sent For Retry";

        /// <summary>
        /// Gets EndCronJobProcess.
        /// </summary>
        /// <value>
        /// EndCronJobProcess.
        /// </value>
        public static string EndCronJobProcess => "{LogBase} - End Cron Job Process";

        /// <summary>
        /// Gets EndCronJobProcessWithError.
        /// </summary>
        /// <value>
        /// EndCronJobProcessWithError.
        /// </value>
        public static string EndCronJobProcessWithError => "{0} - End Cron Job Process With Error - Message Exception: {1} - Inner Exception: {2}";

        /// <summary>
        /// Gets ErrorSendPedidosRequestInBackgroundAsync.
        /// </summary>
        /// <value>
        /// ErrorSendPedidosRequestInBackgroundAsync.
        /// </value>
        public static string ErrorSendPedidosRequestInBackgroundAsync => "{0} - Error Send Pedidos Request In Background Async - Message Exception: {1} - Inner Exception: {2}";

        /// <summary>
        /// Gets ErrorCallingGetPedidosService.
        /// </summary>
        /// <value>
        /// ErrorCallingGetPedidosService.
        /// </value>
        public static string ErrorCallingGetPedidosService => "{0} - Error calling GET Pedidos Service";

        /// <summary>
        /// Gets ErrorCallingPostPedidosService.
        /// </summary>
        /// <value>
        /// ErrorCallingPostPedidosService.
        /// </value>
        public static string ErrorCallingPostPedidosService => "{0} - Error calling POST Pedidos Service";
    }
}
