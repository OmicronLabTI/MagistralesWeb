// <summary>
// <copyright file="DeliveryNoteDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes
{
    /// <summary>
    /// The class for the DeliveryNoteDto.
    /// </summary>
    public class DeliveryNoteDto
    {
        /// <summary>
        /// Gets or sets the Due Date.
        /// </summary>
        /// <value>Due Date.</value>
        [JsonProperty("DocDueDate")]
        public string DueDate { get; set; }

        /// <summary>
        /// Gets or sets the CardCode.
        /// </summary>
        /// <value>CardCode.</value>
        [JsonProperty("CardCode")]
        public string CustomerCode { get; set; }

        /// <summary>
        /// Gets or sets the Document Date.
        /// </summary>
        /// <value>Document Date.</value>
        [JsonProperty("DocDate")]
        public string DocumentDate { get; set; }

        /// <summary>
        /// Gets or sets the Document Type.
        /// </summary>
        /// <value>Document Type.</value>
        [JsonProperty("DocType")]
        public string DocumentType { get; set; }

        /// <summary>
        /// Gets or sets the Document Currency.
        /// </summary>
        /// <value>Document Currency.</value>
        [JsonProperty("DocCurrency")]
        public string DocumentCurrency { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty("Comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Address.
        /// </summary>
        /// <value>Shipping Address.</value>
        [JsonProperty("Address2")]
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the document Series.
        /// </summary>
        /// <value>Series.</value>
        [JsonProperty("Series")]
        public int Series { get; set; }

        /// <summary>
        /// Gets or sets the Shipping Code.
        /// </summary>
        /// <value>Shipping Code.</value>
        [JsonProperty("ShipToCode")]
        public string ShippingCode { get; set; }

        /// <summary>
        /// Gets or sets the Tax Date.
        /// </summary>
        /// <value>Tax Date.</value>
        [JsonProperty("TaxDate")]
        public string TaxDate { get; set; }

        /// <summary>
        /// Gets or sets the Reference Number.
        /// </summary>
        /// <value>Reference Number.</value>
        [JsonProperty("NumAtCard")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Billing Address.
        /// </summary>
        /// <value>Billing Address.</value>
        [JsonProperty("Address")]
        public string BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the Sales Person Code.
        /// </summary>
        /// <value>Sales Person Code.</value>
        [JsonProperty("SalesPersonCode")]
        public int SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets the Delivery Note Lines.
        /// </summary>
        /// <value>Delivery Note Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<DeliveryNoteLineDto> DeliveryNoteLines { get; set; }

        /// <summary>
        /// Gets or sets the Owner code.
        /// </summary>
        /// <value>Documents Owner.</value>
        [JsonProperty("DocumentsOwner")]
        public int DocumentsOwner { get; set; }

        /// <summary>
        /// Gets or sets the DocumentSubType.
        /// </summary>
        /// <value>Document SubType.</value>
        [JsonProperty("DocumentSubType")]
        public string DocumentSubType { get; set; }

        /// <summary>
        /// Gets or sets the JournalMemo.
        /// </summary>
        /// <value>Journal Memo.</value>
        [JsonProperty("JournalMemo")]
        public string JournalMemo { get; set; }

        /// <summary>
        /// Gets or sets the remission comment.
        /// </summary>
        /// <value>U_comentario remision.</value>
        [JsonProperty("U_comentarioremision")]
        public string RemissionComment { get; set; }

        /// <summary>
        /// Gets or sets the Order Package.
        /// </summary>
        /// <value>Order Package.</value>
        [JsonProperty("U_Pedido_Paquete")]
        public string OrderPackage { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value>Is Omigenomics.</value>
        [JsonProperty("U_Omigenomicstp")]
        public string IsSecundary { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value>Is Omigenomics.</value>
        [JsonProperty("U_catagomigenomics")]
        public string IsOmigenomics { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value>Is Omigenomics.</value>
        [JsonProperty("U_TipoPedido")]
        public string DeliveryOrderType { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value>Is Omigenomics.</value>
        [JsonProperty("U_Tipo_Facturacion")]
        public string BillingType { get; set; }
    }
}
