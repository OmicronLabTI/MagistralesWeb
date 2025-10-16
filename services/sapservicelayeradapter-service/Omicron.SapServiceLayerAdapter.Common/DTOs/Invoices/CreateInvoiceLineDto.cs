// <summary>
// <copyright file="CreateInvoiceLineDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices
{
    /// <summary>
    /// The class for the InvoiceLineDto.
    /// </summary>
    public class CreateInvoiceLineDto
    {
        /// <summary>
        /// Gets or sets the Document Entry.
        /// </summary>
        /// <value>Document Entry.</value>
        [JsonProperty("BaseType")]
        public int BaseType { get; set; }

        /// <summary>
        /// Gets or sets the Document Entry.
        /// </summary>
        /// <value>Document Entry.</value>
        [JsonProperty("BaseEntry")]
        public int BaseEntry { get; set; }

        /// <summary>
        /// Gets or sets the Document Entry.
        /// </summary>
        /// <value>Document Entry.</value>
        [JsonProperty("BaseLine")]
        public int BaseLine { get; set; }
    }
}
