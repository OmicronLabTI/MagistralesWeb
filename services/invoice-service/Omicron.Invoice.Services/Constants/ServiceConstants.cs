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
        /// Constant for Offset.
        /// </summary>
        public const string ErrorDownload = "Formulas - Microservice Invoice - Error to download azure blob {0} Error: {1} --- {2}";

        /// <summary>
        /// the container file.
        /// </summary>
        public const string InvoiceErrorsCatalogsFileUrl = "InvoiceErrorsCatalogsFileUrl";

        /// <summary>
        /// Azure Account Name.
        /// </summary>
        public const string AzureAccountName = "AzureAccountName";

        /// <summary>
        /// Azure Account Key.
        /// </summary>
        public const string AzureAccountKey = "AzureAccountKey";

        /// <summary>
        /// InvoiceErrorsCatalogs.
        /// </summary>
        public const string InvoiceErrorsCatalogs = "InvoiceErrorsCatalogs";

        /// <summary>
        /// LogsInvoiceErrorsCatalogs.
        /// </summary>
        public const string LogsInvoiceErrorsCatalogs = "InvoiceCatalogs - Error processing error catalog";

        /// <summary>
        /// InvoiceErrorsCatalogs.
        /// </summary>
        public const string InternalServerError = "Error saving information to database";

        /// <summary>
        /// Gets Const for offset.
        /// </summary>
        public static string Offset => "offset";

        /// <summary>
        /// Gets Const for the limit.
        /// </summary>
        public static string Limit => "limit";

        /// <summary>
        /// Gets const for default offset.
        /// </summary>
        public static string OffsetDefault => "0";

        /// <summary>
        /// Gets Const for the default limit.
        /// </summary>
        public static string LimitDefault => "10";

        /// <summary>
        /// Gets Const for the date for search.
        /// </summary>
        public static string Date => "date";

        /// <summary>
        /// Gets Const for the date for search.
        /// </summary>
        public static string Status => "status";

        /// <summary>
        /// Gets Const for the date for search.
        /// </summary>
        public static string TypeInvoice => "invoiceType";

        /// <summary>
        /// Gets Const for the date for search.
        /// </summary>
        public static string BillingType => "billingType";

        /// <summary>
        /// Gets error update.
        /// </summary>
        public static string ErrorUpdateInvoice => "No es posible actializar el registro. No requiere ajuste manual o ya se actualizo";

        /// <summary>
        /// Gets users by id.
        /// </summary>
        public static string GetUsersById => "getUsersById";

        /// <summary>
        /// Gets parámetro para obtener la key Redis.
        /// </summary>
        public static string CdfiKey => "Cdfi-Version";

        /// <summary>
        /// Gets parámetro para obtener la default Value.
        /// </summary>
        public static string DefaultVersion => "CFDi40";

        /// <summary>
        /// Gets parámetro para obtener la versión de CFDI.
        /// </summary>
        public static string CfdiVersion => "CdfiVersion";

        /// <summary>
        /// Gets get the params.
        /// </summary>
        public static string GetParams => "params/contains/field";

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
        /// Gets const for the status enviada a crear.
        /// </summary>
        public static string SendToCreateInvoice => "Enviada a crear";

        /// <summary>
        /// Gets const for the status enviada a crear.
        /// </summary>
        public static string ValidateInvoiceUrl => "remissions/invoices";

        /// <summary>
        /// Gets const for the status enviada a crear.
        /// </summary>
        public static string SLCreateInvoiceUrl => "create/invoice";

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
        /// Gets TypeId.
        /// </summary>
        /// <value>
        /// String TypeId.
        /// </value>
        public static string IdType => "idtype";

        /// <summary>
        /// Gets the Order Relation.
        /// </summary>
        /// <value>
        /// the OrderRelation.
        /// </value>
        public static Dictionary<string, int> StatusOrder { get; } = new Dictionary<string, int>
        {
            { SendToCreateInvoice, 1 },
            { CreatingInvoiceStatus, 2 },
            { InvoiceCreationErrorStatus, 3 },
        };

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
