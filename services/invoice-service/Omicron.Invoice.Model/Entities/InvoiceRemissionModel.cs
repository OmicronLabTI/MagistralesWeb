// <summary>
// <copyright file="InvoiceRemissionModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Model.Entities
{
    /// <summary>
    /// InvoiceRemissionModel class.
    /// </summary>
    public class InvoiceRemissionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceRemissionModel"/> class.
        /// </summary>
        public InvoiceRemissionModel()
        {
        }

        /// <summary>
        /// Gets or sets the primary key Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the remission identifier.
        /// </summary>
        public int RemissionId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the invoice.
        /// </summary>
        public string IdInvoice { get; set; }

        /// <summary>
        /// Gets or sets navigation property to the invoice.
        /// </summary>
        [JsonIgnore]
        public InvoiceModel Invoice { get; set; }
    }
}