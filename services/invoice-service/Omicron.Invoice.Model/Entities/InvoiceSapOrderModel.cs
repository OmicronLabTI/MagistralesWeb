// <summary>
// <copyright file="InvoiceSapOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Model.Entities
{
    /// <summary>
    /// InvoiceSapOrderModel class.
    /// </summary>
    public class InvoiceSapOrderModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceSapOrderModel"/> class.
        /// </summary>
        public InvoiceSapOrderModel()
        {
        }

        /// <summary>
        /// Gets or sets the primary key Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the SAP order identifier.
        /// </summary>
        public int SapOrderId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the invoice.
        /// </summary>
        public string IdInvoice { get; set; }

        /// <summary>
        /// Gets or sets navigation property to the invoice.
        /// </summary>
        public InvoiceModel Invoice { get; set; }
    }
}