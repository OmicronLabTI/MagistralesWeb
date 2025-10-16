// <summary>
// <copyright file="CreateInvoiceDocumentLinesDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices
{
    /// <summary>
    /// The class for the CreateInvoiceDocumentLinesDto.
    /// </summary>
    public class CreateInvoiceDocumentLinesDto
    {
        /// <summary>
        /// Gets or sets the DocumentBaseType.
        /// </summary>
        /// <value>DocumentBaseType.</value>
        [JsonProperty("BaseType")]
        public int DocumentBaseType { get; set; }

        /// <summary>
        /// Gets or sets the DocumentBaseEntry.
        /// </summary>
        /// <value>DocumentBaseEntry.</value>
        [JsonProperty("BaseEntry")]
        public int DocumentBaseEntry { get; set; }

        /// <summary>
        /// Gets or sets the DocumentBaseLine.
        /// </summary>
        /// <value>DocumentBaseLine.</value>
        [JsonProperty("BaseLine")]
        public int DocumentBaseLine { get; set; }
    }
}
