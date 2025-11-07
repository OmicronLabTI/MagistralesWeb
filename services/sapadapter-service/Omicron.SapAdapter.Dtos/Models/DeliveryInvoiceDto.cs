// <summary>
// <copyright file="DeliveryInvoiceDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Dtos.Models
{
    /// <summary>
    /// class for searhing the details of header and order.
    /// </summary>
    public class DeliveryInvoiceDto
    {
         /// <summary>
        /// Gets or sets the type of processing (Manual or Automatic).
        /// </summary>
        /// <value>
        /// Type string.
        /// </value>
        public long RemissionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the invoice is currently being processed.
        /// </summary>
        /// <value>
        /// True if processing, otherwise false.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the invoice is currently being processed.
        /// </summary>
        /// <value>
        /// True if processing, otherwise false.
        /// </value>
        public int? InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the invoice is currently being processed.
        /// </summary>
        /// <value>
        /// True if processing, otherwise false.
        /// </value>
        public string ProcessId { get; set; }
    }
}
