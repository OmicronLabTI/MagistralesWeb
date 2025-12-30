// <summary>
// <copyright file="OrderCommentDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices
{
    /// <summary>
    /// The class for the InvoiceLineDto.
    /// </summary
    public class OrderCommentDto
    {
        /// <summary>
        /// Gets or sets Document Entry.
        /// </summary>
        [JsonProperty("DocEntry")]
        public int DocEntry { get; set; }

        /// <summary>
        /// Gets or sets Comments.
        /// </summary>
        [JsonProperty("Comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets Canceled status.
        /// </summary>
        [JsonProperty("CANCELED")]
        public string Canceled { get; set; }
    }
}
