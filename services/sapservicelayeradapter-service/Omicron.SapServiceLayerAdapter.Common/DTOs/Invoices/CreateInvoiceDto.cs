// <summary>
// <copyright file="CreateInvoiceDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices
{
    /// <summary>
    /// The class for the CreateInvoiceDto.
    /// </summary>
    public class CreateInvoiceDto
    {
        /// <summary>
        /// Gets or sets the Card Code.
        /// </summary>
        /// <value>CardCode.</value>
        [JsonProperty(nameof(CardCode))]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the DocumentDate.
        /// </summary>
        /// <value>Document Date.</value>
        [JsonProperty("DocDate")]
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Gets or sets the DocumentDueDate.
        /// </summary>
        /// <value>DocumentDueDate.</value>
        [JsonProperty("DocDueDate")]
        public DateTime DocumentDueDate { get; set; }

        /// <summary>
        /// Gets or sets the DocumentTaxDate.
        /// </summary>
        /// <value>DocumentTaxDate.</value>
        [JsonProperty("TaxDate")]
        public DateTime DocumentTaxDate { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty(nameof(Comments))]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty("U_BXP_CFDIVER")]
        public string CfdiDriverVersion { get; set; }

        /// <summary>
        /// Gets or sets the Invoice Lines.
        /// </summary>
        /// <value>Invoice Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<CreateInvoiceDocumentLinesDto> InvoiceDocumentLines { get; set; }
    }
}
