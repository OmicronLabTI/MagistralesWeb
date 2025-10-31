// <summary>
// <copyright file="LogsConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Services.Constants
{
    /// <summary>
    /// Class LogsConstants.
    /// </summary>
    public static class LogsConstants
    {
        /// <summary>
        /// Gets GetDataToRetryCreateInvoicesAsyncLogBase.
        /// </summary>
        /// <value>
        /// String GetDataToRetryCreateInvoicesAsyncLogBase.
        /// </value>
        /// <param name="processId">Process Id.</param>
        /// <returns>Redis Key.</returns>
        public static string GetDataToRetryCreateInvoicesAsyncLogBase(string processId) => $"{processId} - Retry Create Invoice - GetDataToRetryCreateInvoicesAsync";

        /// <summary>
        /// Gets AutomaticProcessAlreadyRunning.
        /// </summary>
        /// <value>
        /// String AutomaticProcessAlreadyRunning.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <returns>Redis Key.</returns>
        public static string AutomaticProcessAlreadyRunning(string logBase) => $"{logBase} - The automatic process is already running";

        /// <summary>
        /// Gets RetryWillProcessRecords.
        /// </summary>
        /// <value>
        /// String RetryWillProcessRecords.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <param name="recordCount">Record Count.</param>
        /// <returns>Redis Key.</returns>
        public static string RetryWillProcessRecords(string logBase, int recordCount) => $"{logBase} - The retry will process {recordCount} records.";

        /// <summary>
        /// Gets RetryCreateInvoicesAsync.
        /// </summary>
        /// <value>
        /// String RetryCreateInvoicesAsync.
        /// </value>
        /// <param name="processId">Process Id.</param>
        /// <returns>Redis Key.</returns>
        public static string RetryCreateInvoicesAsync(string processId) => $"{processId} - Retry Create Invoice - GetDataToRetryCreateInvoicesAsync";

        /// <summary>
        /// Gets ThereIsNoInformationToProcess.
        /// </summary>
        /// <value>
        /// String ThereIsNoInformationToProcess.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <returns>Redis Key.</returns>
        public static string ThereIsNoInformationToProcess(string logBase) => $"{logBase} - There is no information to process";

        /// <summary>
        /// Gets TheIdIsAlreadyBeingProcessed.
        /// </summary>
        /// <value>
        /// String TheIdIsAlreadyBeingProcessed.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <param name="id">Id.</param>
        /// <returns>Redis Key.</returns>
        public static string TheIdIsAlreadyBeingProcessed(string logBase, string id) => $"{logBase} - The id {id} is already being processed";

        /// <summary>
        /// Gets AlreadyInARetryProcessOrTheInvoiceWasSuccessfullyCreated.
        /// </summary>
        /// <value>
        /// String AlreadyInARetryProcessOrTheInvoiceWasSuccessfullyCreated.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <param name="id">Id.</param>
        /// <param name="isProcessed">Is Processed.</param>
        /// <param name="status">Status.</param>
        /// <param name="invoiceHasValue">Invoice Has Value.</param>
        /// <returns>Redis Key.</returns>
        public static string AlreadyInARetryProcessOrTheInvoiceWasSuccessfullyCreated(string logBase, string id, bool isProcessed, string status, bool invoiceHasValue) => $"{logBase} - The id {id} is already in a retry process or the invoice was successfully created. IsProcessed: {isProcessed} - Status: {status} - InvoiceHasValue: {invoiceHasValue}";

        /// <summary>
        /// Gets InvoicesToBeRetried.
        /// </summary>
        /// <value>
        /// String InvoicesToBeRetried.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <param name="invoices">Invoices.</param>
        /// <returns>Redis Key.</returns>
        public static string InvoicesToBeRetried(string logBase, string invoices) => $"{logBase} - Invoices to be retried: {invoices}";

        /// <summary>
        /// Gets RedisKeysAreDeletedForRetryControl.
        /// </summary>
        /// <value>
        /// String RedisKeysAreDeletedForRetryControl.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <returns>Redis Key.</returns>
        public static string RedisKeysAreDeletedForRetryControl(string logBase) => $"{logBase} - Redis keys are deleted for retry control";

        /// <summary>
        /// Gets RetryProcessCompletedSuccessfully.
        /// </summary>
        /// <value>
        /// String RetryProcessCompletedSuccessfully.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <param name="result">Result.</param>
        /// <returns>Redis Key.</returns>
        public static string RetryProcessCompletedSuccessfully(string logBase, string result) => $"{logBase} - Process completed successfully - Result: {result}";

        /// <summary>
        /// Gets RetryProcessCompletedWithAnError.
        /// </summary>
        /// <value>
        /// String RetryProcessCompletedWithAnError.
        /// </value>
        /// <param name="logBase">Log Base.</param>
        /// <param name="id">Id.</param>
        /// <returns>Redis Key.</returns>
        public static string RetryProcessCompletedWithAnError(string logBase, string id) => $"{logBase} - Process completed with an error for id: {id}";

         /// <summary>
        /// Gets the QueuedHostedServiceRunning.
        /// </summary>
        public static string QueuedHostedServiceRunning => "Invoices - Queued Hosted Service is running.";

        /// <summary>
        /// Gets the ErrorOccurredExecutingBackgroundTask.
        /// </summary>
        public static string ErrorOccurredExecutingBackgroundTask => "Invoices - Error occurred executing a background task. - Error: {0}";

        /// <summary>
        /// Gets the CreateInvoicceLogPushMediatRError.
        /// </summary>
        public static string CreateInvoiceLogPushMediatRError => "Invoices - Error In Process Handle CreateInvoiceHandler - TransactionId: {0} - Object: {1} - Error: {2}";

        /// <summary>
        /// Gets the CreateInvoiceLogPushMediatRStart.
        /// </summary>
        public static string CreateInvoiceLogPushMediatRStart => "Invoices - Start To Process Handle CreateInvoiceHandler - TransactionId: {0} - Object: {1}";

        /// <summary>
        /// Gets the CreateInvoiceLogPushMediatRFinish.
        /// </summary>
        public static string CreateInvoiceLogPushMediatRFinish => "Invoices - Finish To Process Handle CreateInvoiceHandler - TransactionId: {0}";

         /// <summary>
        /// Gets the PublishProcessInvoiceToMediatrQueueError.
        /// </summary>
        public static string PublishProcessInvoiceToMediatrQueueError => "Invoices - Error en PublishProcessToMediatR al encolar la tarea - TransactionId: {0} - Error: {1}";
    }
}
