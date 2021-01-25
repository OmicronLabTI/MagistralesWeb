// <summary>
// <copyright file="InvoiceQrModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class for the qr for invoices.
    /// </summary>
    public class InvoiceQrModel
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public bool NeedsCooling { get; set; }
    }
}
