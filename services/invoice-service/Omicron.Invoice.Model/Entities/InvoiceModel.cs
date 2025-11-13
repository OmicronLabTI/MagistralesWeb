// <summary>
// <copyright file="InvoiceModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Model.Entities
{
    /// <summary>
    /// InvoiceModel class represents a single invoice with all related data.
    /// including remissions, SAP orders, and error information.
    /// </summary>
    public class InvoiceModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceModel"/> class.
        /// </summary>
        public InvoiceModel()
        {
            this.Remissions = new List<InvoiceRemissionModel>();
            this.SapOrders = new List<InvoiceSapOrderModel>();
        }

        /// <summary>
        /// Gets or sets the primary key Id of the invoice.
        /// </summary>
        /// <value>
        /// Unique identifier for the invoice.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the DXP order identifier associated with this invoice.
        /// </summary>
        /// <value>
        /// DXP order id string.
        /// </value>
        public string DxpOrderId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the invoice error (nullable).
        /// </summary>
        /// <value>
        /// Id of the invoice error if exists.
        /// </value>
        public int? IdInvoiceError { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the invoice.
        /// </summary>
        /// <value>
        /// DateTime when the invoice was created.
        /// </value>
        public DateTime CreateDate { get; set; }

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
        /// Gets or sets the SAP invoice identifier (nullable until created in SAP).
        /// </summary>
        /// <value>
        /// SAP invoice id.
        /// </value>
        public int? IdFacturaSap { get; set; }

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
        /// Gets or sets the invoice creation date.
        /// </summary>
        /// <value>
        /// DateTime when the invoice was generated.
        /// </value>
        public DateTime? InvoiceCreateDate { get; set; }

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
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts for processing this invoice.
        /// </summary>
        /// <value>
        /// Retry count integer.
        /// </value>
        public int RetryNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of processing (Manual or Automatic).
        /// </summary>
        /// <value>
        /// Type string.
        /// </value>
        public string Type { get; set; }

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
        /// Gets or sets the Payload.
        /// </summary>
        /// <value>
        /// Type string.
        /// </value>
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets the user who retried the process. This field is null if the process succeeded on the first attempt.
        /// </summary>
        /// <value>
        /// Username string.
        /// </value>
        public string RetryUser { get; set; }

        /// <summary>
        /// Gets or sets navigation property to invoice error.
        /// </summary>
        /// <value>
        /// The <see cref="InvoiceErrorModel"/> associated with this invoice.
        /// </value>
        public InvoiceErrorModel InvoiceError { get; set; }

        /// <summary>
        /// Gets or sets navigation property to remissions associated with this invoice.
        /// </summary>
        /// <value>
        /// Collection of <see cref="InvoiceRemissionModel"/>.
        /// </value>
        public ICollection<InvoiceRemissionModel> Remissions { get; set; }

        /// <summary>
        /// Gets or sets navigation property to SAP orders associated with this invoice.
        /// </summary>
        /// <value>
        /// Collection of <see cref="InvoiceSapOrderModel"/>.
        /// </value>
        public ICollection<InvoiceSapOrderModel> SapOrders { get; set; }
    }
}
