// <summary>
// <copyright file="InvoiceDto.cs" company="Axity">
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
    public class InvoiceDto
    {
        /// <summary>
        /// Gets or sets the Document Entry.
        /// </summary>
        /// <value>Document Entry.</value>
        [JsonProperty("DocEntry")]
        public int DocumentEntry { get; set; }

        /// <summary>
        /// Gets or sets the Document Number.
        /// </summary>
        /// <value>Document Number.</value>
        [JsonProperty("DocNum")]
        public int DocumentNumber { get; set; }

        /// <summary>
        /// Gets or sets the DocumentDate.
        /// </summary>
        /// <value>Document Date.</value>
        [JsonProperty("DocDate")]
        public string DocumentDate { get; set; }

        /// <summary>
        /// Gets or sets the Card Code.
        /// </summary>
        /// <value>CardCode.</value>
        [JsonProperty("CardCode")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the Document Type.
        /// </summary>
        /// <value>Document Type.</value>
        [JsonProperty("DocType")]
        public string DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty("Comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty("TransportationCode")]
        public short TransportationCode { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty("TrackingNumber")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the Invoice Lines.
        /// </summary>
        /// <value>Invoice Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<InvoiceLineDto> InvoiceLines { get; set; }

        /// <summary>
        /// Gets or sets the ExtendedTrackingNumbers.
        /// </summary>
        /// <value>ExtendedTrackingNumbers.</value>
        [JsonProperty("U_Comentarios")]
        public string ExtendedTrackingNumbers { get; set; }
    }
}
