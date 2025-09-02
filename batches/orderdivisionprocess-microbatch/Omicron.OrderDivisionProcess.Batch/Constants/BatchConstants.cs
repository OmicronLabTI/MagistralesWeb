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
        /// Gets DivisionOrdersRetryCronjob.
        /// </summary>
        /// <value>
        /// DivisionOrdersRetryCronjob.
        /// </value>
        public static string DivisionOrdersRetryCronjob => "division-orders-retry-cronjob";

        /// <summary>
        /// Gets DivisionOrdersToProcessKey .
        /// </summary>
        /// <value>
        /// DivisionOrdersToProcessKey .
        /// </value>
        public static string DivisionOrdersToProcessKey => "division-orders-to-process";

        /// <summary>
        /// Gets GetFailedDivisionOrdersEndpoint.
        /// </summary>
        /// <value>
        /// GetFailedDivisionOrdersEndpoint.
        /// </value>
        public static string GetFailedDivisionOrdersEndpoint => "failed/divisionorders";

        /// <summary>
        /// Gets PostRetryDivisionFailedProductionOrdersEndpoint.
        /// </summary>
        /// <value>
        /// PostRetryDivisionFailedProductionOrdersEndpoint.
        /// </value>
        public static string PostRetryDivisionFailedProductionOrdersEndpoint => "retry/division/failed/productionorders";

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
        /// Gets DivisionOrdersCronJob .
        /// </summary>
        /// <value>
        /// DivisionOrdersCronJob .
        /// </value>
        public static string DivisionOrdersCronJob => "{0} - Division Orders Retry Cron Job";

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
        /// Gets ErrorRetrievingDivisionOrders.
        /// </summary>
        /// <value>
        /// ErrorRetrievingDivisionOrders.
        /// </value>
        public static string ErrorRetrievingDivisionOrders => "{LogBase} - Error Retrieving Division Orders";

        /// <summary>
        /// Gets ThereAreNoDivisionOrdersToProcess .
        /// </summary>
        /// <value>
        /// ThereAreNoDivisionOrdersToProcess .
        /// </value>
        public static string ThereAreNoDivisionOrdersToProcess => "{LogBase} - There Are No Division Orders To Process";

        /// <summary>
        /// Gets NumberOfDivisionOrdersToProcess.
        /// </summary>
        /// <value>
        /// NumberOfDivisionOrdersToProcess.
        /// </value>
        public static string NumberOfDivisionOrdersToProcess => "{LogBase} - Number Of Division Orders To Process: {DivisionOrdersCount}";

        /// <summary>
        /// Gets BatchRangeCount.
        /// </summary>
        /// <value>
        /// BatchRangeCount.
        /// </value>
        public static string BatchRangeCount => "{LogBase} - Batch Range Count: {BatchCount}";

        /// <summary>
        /// Gets DivisionOrdersAreRetrievedFromRedis.
        /// </summary>
        /// <value>
        /// DivisionOrdersAreRetrievedFromRedis.
        /// </value>
        public static string DivisionOrdersAreRetrievedFromRedis => "{LogBase} - Division Orders Are Retrieved From Redis - Offset: {Offset} - Limit: {Limit}";

        /// <summary>
        /// Gets StartDivisionOrdersAreSentForRetry .
        /// </summary>
        /// <value>
        /// StartDivisionOrdersAreSentForRetry .
        /// </value>
        public static string StartDivisionOrdersAreSentForRetry => "{LogBase} - Start - Division Orders Are Sent For Retry - {Object}";

        /// <summary>
        /// Gets EndDivisionOrdersAreSentForRetry .
        /// </summary>
        /// <value>
        /// EndDivisionOrdersAreSentForRetry .
        /// </value>
        public static string EndDivisionOrdersAreSentForRetry => "{LogBase} - End - Division Orders Are Sent For Retry";

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
