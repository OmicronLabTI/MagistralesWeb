// <summary>
// <copyright file="CreateOrderDto.cs" company="Axity">
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
    public class CreateOrderDto
    {
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
        /// Gets or sets the Reference Number.
        /// </summary>
        /// <value>Reference Number.</value>
        [JsonProperty("NumAtCard")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Due Date.
        /// </summary>
        /// <value>Due Date.</value>
        [JsonProperty("DocDueDate")]
        public DateTime DueDate { get; set; }

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
        public List<CreateOrderLineDto> OrderLines { get; set; }

        /// <summary>
        /// Gets or sets the ShipToCode.
        /// </summary>
        /// <value>Ship To Code.</value>
        [JsonProperty("ShipToCode")]
        public string ShippingCode { get; set; }

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

        /// <summary>
        /// Gets or sets the Sales Person Code.
        /// </summary>
        /// <value>Sales Person Code.</value>
        [JsonProperty("SalesPersonCode")]
        public int? SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets the Owner code.
        /// </summary>
        /// <value>Documents Owner.</value>
        [JsonProperty("DocumentsOwner")]
        public int? DocumentsOwner { get; set; }
    }
}
