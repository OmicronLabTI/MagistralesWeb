// <summary>
// <copyright file="InvoiceErrorDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Common.DTOs.Responses.Invoices
{
    /// <summary>
    /// InvoiceErrorDto class.
    /// </summary>
    public class InvoiceErrorDto
    {
        /// <summary>
        /// Gets or sets the primary key Id of the invoice.
        /// </summary>
        /// <value>
        /// Unique identifier for the invoice.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the invoice.
        /// </summary>
        /// <value>
        /// DateTime when the invoice was created.
        /// </value>
        public string CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the user who processed the invoice in the warehouse.
        /// </summary>
        /// <value>
        /// Username string.
        /// </value>
        public string AlmacenUser { get; set; }

        /// <summary>
        /// Gets or sets the status of the invoice.
        /// </summary>
        /// <value>
        /// Status string.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the DXP order identifier associated with this invoice.
        /// </summary>
        /// <value>
        /// DXP order id string.
        /// </value>
        public string DxpOrderId { get; set; }

        /// <summary>
        /// Gets or sets the type of the invoice (generic or non-generic).
        /// </summary>
        /// <value>
        /// Invoice type string.
        /// </value>
        public string TypeInvoice { get; set; }

        /// <summary>
        /// Gets or sets the billing type (complete or partial).
        /// </summary>
        /// <value>
        /// Billing type string.
        /// </value>
        public string BillingType { get; set; }

        /// <summary>
        /// Gets or sets the error message if any issue occurred.
        /// </summary>
        /// <value>
        /// Error message string.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the invoice (nullable).
        /// </summary>
        /// <value>
        /// DateTime of last update if any.
        /// </value>
        public string UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts for processing this invoice.
        /// </summary>
        /// <value>
        /// Retry count integer.
        /// </value>
        public int RetryNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether manual changes have been applied.
        /// </summary>
        /// <value>
        /// True if manual changes applied, otherwise false.
        /// </value>
        public bool? ManualChangeApplied { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the invoice is currently being processed.
        /// </summary>
        /// <value>
        /// True if processing, otherwise false.
        /// </value>
        public bool IsProcessing { get; set; }

        /// <summary>
        /// Gets or sets the remission identifier.
        /// </summary>
        public List<int> RemissionId { get; set; }

        /// <summary>
        /// Gets or sets the SAP order identifier.
        /// </summary>
        public List<int> SapOrderId { get; set; }
    }
}