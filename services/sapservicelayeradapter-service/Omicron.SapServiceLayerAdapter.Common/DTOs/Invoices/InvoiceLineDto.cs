// <summary>
// <copyright file="InvoiceLineDto.cs" company="Axity">
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
    public class InvoiceLineDto
    {
        /// <summary>
        /// Gets or sets the Item Code.
        /// </summary>
        /// <value>Item Code.</value>
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        /// <value>Quantity.</value>
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the Warehouse Code.
        /// </summary>
        /// <value>Warehouse Code.</value>
        [JsonProperty("WarehouseCode")]
        public string WarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets the Price.
        /// </summary>
        /// <value>Price.</value>
        [JsonProperty("Price")]
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the Currency.
        /// </summary>
        /// <value>Currency.</value>
        [JsonProperty("Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the Tax Code.
        /// </summary>
        /// <value>Tax Code.</value>
        [JsonProperty("TaxCode")]
        public string TaxCode { get; set; }

        /// <summary>
        /// Gets or sets the Discount Percent.
        /// </summary>
        /// <value>Discount Percent.</value>
        [JsonProperty("DiscountPercent")]
        public double DiscountPercent { get; set; }

        /// <summary>
        /// Gets or sets the Unit Price.
        /// </summary>
        /// <value>Unit Price.</value>
        [JsonProperty("UnitPrice")]
        public double UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the Reason.
        /// </summary>
        /// <value>Reason.</value>
        [JsonProperty("U_Motivo")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the Source.
        /// </summary>
        /// <value>Source.</value>
        [JsonProperty("U_Fuente")]
        public string Source { get; set; }
    }
}
