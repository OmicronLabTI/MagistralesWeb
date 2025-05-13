// <summary>
// <copyright file="InvoiceProductsDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// class for the complete order.
    /// </summary>
    public class InvoiceProductsDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<int> Invoices { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<int> LineNumbers { get; set; }
    }
}
