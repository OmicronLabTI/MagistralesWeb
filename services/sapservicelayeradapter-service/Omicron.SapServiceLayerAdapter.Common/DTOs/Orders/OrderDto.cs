// <summary>
// <copyright file="OrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Orders
{
    /// <summary>
    /// The class for the OrderDto.
    /// </summary>
    public class OrderDto
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
        public DateTime DocumentDate { get; set; }

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
        /// Gets or sets the Series.
        /// </summary>
        /// <value>Series.</value>
        [JsonProperty("Series")]
        public int Series { get; set; }

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
        /// Gets or sets the Due Date.
        /// </summary>
        /// <value>Due Date.</value>
        [JsonProperty("DocDueDate")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the Contact Person.
        /// </summary>
        /// <value>Contact Person.</value>
        [JsonProperty("ContactPerson")]
        public int ContactPerson { get; set; }

        /// <summary>
        /// Gets or sets the Sales Person Code.
        /// </summary>
        /// <value>Sales Person Code.</value>
        [JsonProperty("SalesPersonCode")]
        public int SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets the Branch.
        /// </summary>
        /// <value>Branch.</value>
        [JsonProperty("U_Sucursal")]
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the Custom Shipping Code.
        /// </summary>
        /// <value>Custom Shipping Code.</value>
        [JsonProperty("U_CodigoEnvio")]
        public string CustomShippingCode { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value>Is Omigenomics.</value>
        [JsonProperty("U_Omigenomicstp")]
        public string IsOmigenomics { get; set; }

        /// <summary>
        /// Gets or sets the Order Lines.
        /// </summary>
        /// <value>Order Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<OrderLineDto> OrderLines { get; set; }

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
        /// Gets or sets the ShipToCode.
        /// </summary>
        /// <value>Ship To Code.</value>
        [JsonProperty("ShipToCode")]
        public string ShippingCode { get; set; }

        /// <summary>
        /// Gets or sets the JournalMemo.
        /// </summary>
        /// <value>Journal Memo.</value>
        [JsonProperty("JournalMemo")]
        public string JournalMemo { get; set; }

        /// <summary>
        /// Gets or sets the TypeOrder.
        /// </summary>
        /// <value>TypeOrder.</value>
        [JsonProperty("U_TipoPedido")]
        public string? TypeOrder { get; set; }

        /// <summary>
        /// Gets or sets the PayToCode.
        /// </summary>
        /// <value>PayToCode.</value>
        [JsonProperty("PayToCode")]
        public string? PayToCode { get; set; }

        /// <summary>
        /// Gets or sets the TaxId.
        /// </summary>
        /// <value>TaxId.</value>
        [JsonProperty("FederalTaxID")]
        public string? TaxId { get; set; }

        /// <summary>
        /// Gets or sets the DiscountPercent.
        /// </summary>
        /// <value>DiscountPercent.</value>
        [JsonProperty("DiscountPercent")]
        public double DiscountPercent { get; set; }

        /// <summary>
        /// Gets or sets the DxpOrder.
        /// </summary>
        /// <value>DxpOrder.</value>
        [JsonProperty("U_Pedido_DXP")]
        public string DxpOrder { get; set; }

        /// <summary>
        /// Gets or sets the EcommerceComments.
        /// </summary>
        /// <value>EcommerceComments.</value>
        [JsonProperty("U_Comentarios_Ecommerce")]
        public string? EcommerceComments { get; set; }

        /// <summary>
        /// Gets or sets the BXPPaymentMethod.
        /// </summary>
        /// <value>BXPPaymentMethod.</value>
        [JsonProperty("U_BXP_METPAGO33")]
        public string BXPPaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the BXPWatToPay.
        /// </summary>
        /// <value>BXPFormaPago33.</value>
        [JsonProperty("U_BXP_FORMAPAGO33")]
        public string BXPWayToPay { get; set; }

        /// <summary>
        /// Gets or sets the OrderPackage.
        /// </summary>
        /// <value>OrderPackage.</value>
        [JsonProperty("U_Pedido_Paquete")]
        public string OrderPackage { get; set; }

        /// <summary>
        /// Gets or sets the DXPNeedsShipCost.
        /// </summary>
        /// <value>DXPNeedsShipCost.</value>
        [JsonProperty("U_DXPNEEDSSHIPCOST")]
        public string DXPNeedsShipCost { get; set; }

        /// <summary>
        /// Gets or sets the SampleOrder.
        /// </summary>
        /// <value>SampleOrder.</value>
        [JsonProperty("U_PedidoMuestra")]
        public string SampleOrder { get; set; }

        /// <summary>
        /// Gets or sets the AttachmentEntry.
        /// </summary>
        /// <value>AttachmentEntry.</value>
        [JsonProperty("AttachmentEntry")]
        public int? AttachmentEntry { get; set; }

        /// <summary>
        /// Gets or sets the CFDIProvisional.
        /// </summary>
        /// <value>CFDIProvisional.</value>
        [JsonProperty("U_CFDI_Provisional")]
        public string? CFDIProvisional { get; set; }
    }
}
