// <summary>
// <copyright file="LineProductsDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Dtos.Models.Almacen
{
    /// <summary>
    /// Class Line products.
    /// </summary>
    public class LineProductsDto
    {
        /// <summary>
        /// Gets or sets InvoiceId.
        /// </summary>
        /// <value>
        /// int InvoiceId.
        /// </value>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets InvoiceLineNum.
        /// </summary>
        /// <value>
        /// short InvoiceLineNum.
        /// </value>
        public short InvoiceLineNum { get; set; }

        /// <summary>
        /// Gets or sets StatusInvoice.
        /// </summary>
        /// <value>
        /// string StatusInvoice.
        /// </value>
        public string StatusInvoice { get; set; }
    }
}
