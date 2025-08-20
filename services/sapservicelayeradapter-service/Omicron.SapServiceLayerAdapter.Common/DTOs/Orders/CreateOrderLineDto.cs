// <summary>
// <copyright file="CreateOrderLineDto.cs" company="Axity">
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
    public class CreateOrderLineDto
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
        /// Gets or sets the PromotionalCode .
        /// </summary>
        /// <value>PromotionalCode .</value>
        [JsonProperty("U_CodigoPromo")]
        public string PromotionalCode { get; set; }
    }
}
