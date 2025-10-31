// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Constants
{
    /// <summary>
    /// class ServiceConstants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Gets SuccessfulInvoiceCreationStatus.
        /// </summary>
        /// <value>
        /// String SuccessfulInvoiceCreationStatus.
        /// </value>
        public static string SuccessfulInvoiceCreationStatus => "Creación exitosa";

        /// <summary>
        /// Gets CreatingInvoiceStatus.
        /// </summary>
        /// <value>
        /// String CreatingInvoiceStatus.
        /// </value>
        public static string CreatingInvoiceStatus => "Creando factura";

        /// <summary>
        /// Gets InvoiceCreationErrorStatus.
        /// </summary>
        /// <value>
        /// String InvoiceCreationErrorStatus.
        /// </value>
        public static string InvoiceCreationErrorStatus => "Error al crear";

        /// <summary>
        /// Gets AutomaticExecutionType.
        /// </summary>
        /// <value>
        /// String AutomaticExecutionType.
        /// </value>
        public static string AutomaticExecutionType => "Automatic";

        /// <summary>
        /// Gets InvoiceLockAutomaticRetryKey.
        /// </summary>
        /// <value>
        /// String InvoiceLockAutomaticRetryKey.
        /// </value>
        public static string InvoiceLockAutomaticRetryKey => "invoices:lock:automaticretry";

        /// <summary>
        /// Gets InvoiceToProcessAutomaticRetryKey.
        /// </summary>
        /// <value>
        /// String InvoiceToProcessAutomaticRetryKey.
        /// </value>
        public static string InvoiceToProcessAutomaticRetryKey => "invoices:toprocess:automaticretry";

        /// <summary>
        /// Gets GetRetryInvoiceLockKey.
        /// </summary>
        /// <value>
        /// String GetRetryInvoiceLockKey.
        /// </value>
        /// <param name="id">Id.</param>
        /// <returns>Redis Key.</returns>
        public static string GetRetryInvoiceLockKey(string id) => $"lock:retryinvoice:{id}";

        /// <summary>
        /// Gets GetRetryInvoiceLockValue.
        /// </summary>
        /// <value>
        /// String GetRetryInvoiceLockValue.
        /// </value>
        /// <param name="id">Id.</param>
        /// <param name="executionType">Execution Type.</param>
        /// <returns>Redis Key.</returns>
        public static string GetRetryInvoiceLockValue(string id, string executionType) => $"{id}:{executionType}:{Guid.NewGuid()}";
    }
}
