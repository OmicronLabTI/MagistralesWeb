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
