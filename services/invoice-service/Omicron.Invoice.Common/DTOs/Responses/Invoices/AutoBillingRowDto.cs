// <summary>
// <copyright file="AutoBillingRowDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Common.DTOs.Responses.Invoices
{
    using System.Collections.Generic;
    using Omicron.Invoice.Model.Entities;

    /// <summary>
    /// Represents a data transfer object (DTO) used to display automatic billing
    /// (AutoBilling) information in the frontend grid. Each record corresponds to
    /// an invoice and contains its basic details, retry count, and related
    /// SAP orders and remissions objects.
    /// </summary>
    public class AutoBillingRowDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the invoice.
        /// Typically corresponds to the internal document reference in the billing system.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the SAP invoice identifier.
        /// This value represents the identifier assigned to the document in SAP after successful billing.
        /// </summary>
        public string IdFacturaSap { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the invoice.
        /// The value is formatted as a string in the pattern "dd/MM/yy HH:mm:ss" for display purposes.
        /// </summary>
        public string InvoiceCreateDate { get; set; }

        /// <summary>
        /// Gets or sets the invoice type.
        /// Common examples include “Normal” or “Credit Note” depending on the transaction nature.
        /// </summary>
        public string TypeInvoice { get; set; }

        /// <summary>
        /// Gets or sets the billing mode used during invoice creation.
        /// Possible values include "Automatic" or "Manual".
        /// </summary>
        public string BillingType { get; set; }

        /// <summary>
        /// Gets or sets the warehouse user’s friendly name who initiated or is responsible for the invoice.
        /// The name is composed from the user’s first and last names obtained from the external user service.
        /// </summary>
        public string AlmacenUser { get; set; }

        /// <summary>
        /// Gets or sets the DXP order identifier associated with the invoice.
        /// This field links the invoice to its originating DXP (digital exchange platform) order.
        /// </summary>
        public string DxpOrderId { get; set; }

        /// <summary>
        /// Gets or sets the shop transaction identifier.
        /// Currently used as a placeholder until the transaction column becomes available in the database.
        /// </summary>
        public string ShopTransaction { get; set; }

        /// <summary>
        /// Gets or sets the total number of SAP orders related to this invoice.
        /// This count provides a quick reference for the number of SAP documents generated.
        /// </summary>
        public int SapOrdersCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of remissions related to this invoice.
        /// This value indicates how many remission documents were generated for this billing record.
        /// </summary>
        public int RemissionsCount { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts executed for this invoice.
        /// A higher value typically indicates previous failures or reprocessing attempts in the billing workflow.
        /// </summary>
        public int RetryNumber { get; set; }

        /// <summary>
        /// Gets or sets the complete list of SAP order entities linked to this invoice.
        /// Each <see cref="InvoiceSapOrderModel"/> object contains details about the corresponding SAP order.
        /// </summary>
        public List<InvoiceSapOrderModel> SapOrders { get; set; } = new List<InvoiceSapOrderModel>();

        /// <summary>
        /// Gets or sets the complete list of remission entities linked to this invoice.
        /// Each <see cref="InvoiceRemissionModel"/> object contains detailed remission information.
        /// </summary>
        public List<InvoiceRemissionModel> Remissions { get; set; } = new List<InvoiceRemissionModel>();

        /// <summary>
        /// Gets or sets the last error message generated during invoice creation.
        /// If the error is known, this message should be mapped from the error catalog (OM-6971);
        /// otherwise, it contains the original technical error message.
        /// </summary>
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the invoice status.
        /// This field is updated whenever the status changes or a retry is performed,
        /// formatted as "dd/MM/yy HH:mm:ss".
        /// </summary>
        public string LastUpdateDate { get; set; }
    }
}
