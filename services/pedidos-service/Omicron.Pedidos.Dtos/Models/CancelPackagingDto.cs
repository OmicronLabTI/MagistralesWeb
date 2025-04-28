// <summary>
// <copyright file="CancelPackagingDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// Class for model.
    /// </summary>
    public class CancelPackagingDto
    {
        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>
        /// String ItemCode.
        /// </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// integer FirstName.
        /// </value>
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets InvoiceId.
        /// </summary>
        /// <value>
        /// integer InvoiceId.
        /// </value>
        public int InvoiceId { get; set; }
    }
}
