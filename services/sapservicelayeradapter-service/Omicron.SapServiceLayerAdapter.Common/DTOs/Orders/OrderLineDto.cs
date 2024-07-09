// <summary>
// <copyright file="OrderLineDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Orders
{
    /// <summary>
    /// The class for the OrderLineDto.
    /// </summary>
    public class OrderLineDto
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
        /// Gets or sets the SalesPersonCode.
        /// </summary>
        /// <value>Sales Person Code.</value>
        [JsonProperty("SalesPersonCode")]
        public int SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets the OwnerCode.
        /// </summary>
        /// <value>Owner Code.</value>
        [JsonProperty("OwnerCode")]
        public int OwnerCode { get; set; }

        /// <summary>
        /// Gets or sets the Line Total.
        /// </summary>
        /// <value>LineTotal.</value>
        [JsonProperty("LineTotal")]
        public double LineTotal { get; set; }

        /// <summary>
        /// Gets or sets the Container.
        /// </summary>
        /// <value>Container.</value>
        [JsonProperty("U_ENVASE")]
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the Label.
        /// </summary>
        /// <value>Label.</value>
        [JsonProperty("U_ETIQUETA")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the Prescription .
        /// </summary>
        /// <value>Prescription .</value>
        [JsonProperty("U_RECETA")]
        public string Prescription { get; set; }

        /// <summary>
        /// Gets or sets the BaseEntry.
        /// </summary>
        /// <value>Base Entry.</value>
        [JsonProperty("BaseEntry")]
        public int? BaseEntry { get; set; }

        /// <summary>
        /// Gets or sets the BaseLine.
        /// </summary>
        /// <value>Base Line.</value>
        [JsonProperty("BaseLine")]
        public int? BaseLine { get; set; }

        /// <summary>
        /// Gets or sets the LineNum.
        /// </summary>
        /// <value>Line Num.</value>
        [JsonProperty("LineNum")]
        public int LineNum { get; set; }
    }
}
