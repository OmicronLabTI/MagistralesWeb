// <summary>
// <copyright file="StockTransferLineDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.StockTransfer
{
    /// <summary>
    /// The class for the StockTransferLineDto.
    /// </summary>
    public class StockTransferLineDto
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
        /// Gets or sets the Warehouse Code.
        /// </summary>
        /// <value>Warehouse Code.</value>
        [JsonProperty("FromWarehouseCode")]
        public string FromWarehouseCode { get; set; }
    }
}
