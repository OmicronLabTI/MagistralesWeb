// <summary>
// <copyright file="InventoryGenExitLineDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryGenExit
{
    /// <summary>
    /// The class for the InvoiceLineDto.
    /// </summary>
    public class InventoryGenExitLineDto
    {
        /// <summary>
        /// Gets or sets BaseEntry.
        /// </summary>
        /// <value>BaseEntry.</value>
        [JsonProperty("BaseEntry")]
        public int? BaseEntry { get; set; }

        /// <summary>
        /// Gets or sets thePrice.
        /// </summary>
        /// <value>Price.</value>
        [JsonProperty("BaseType")]
        public int BaseType { get; set; }

        /// <summary>
        /// Gets or sets thePrice.
        /// </summary>
        /// <value>Price.</value>
        [JsonProperty("BaseLine")]
        public int? BaseLine { get; set; }

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
        /// Gets or sets the Account Code.
        /// </summary>
        /// <value>Account Code.</value>
        [JsonProperty("AccountCode")]
        public string AccountCode { get; set; }

        /// <summary>
        /// Gets or sets the Item Code.
        /// </summary>
        /// <value>Item Code.</value>
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the Source.
        /// </summary>
        /// <value>Source.</value>
        [JsonProperty("BatchNumbers")]
        public List<BatchNumbersDto> BatchNumbers { get; set; }
    }
}
