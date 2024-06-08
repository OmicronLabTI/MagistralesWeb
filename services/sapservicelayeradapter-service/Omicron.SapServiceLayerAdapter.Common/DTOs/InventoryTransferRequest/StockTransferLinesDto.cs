// <summary>
// <copyright file="StockTransferLinesDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryTransferRequest
{
    /// <summary>
    /// Class for Stock Transfer Lines Dto.
    /// </summary>
    public class StockTransferLinesDto
    {
        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>The ItemCode.</value>
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        /// <value>The Quantity.</value>
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets FromWarehouseCode.
        /// </summary>
        /// <value>The FromWarehouseCode.</value>
        [JsonProperty("FromWarehouseCode")]
        public string FromWarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>The ItemCode.</value>
        [JsonProperty("WarehouseCode")]
        public string WarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>The ItemCode.</value>
        [JsonProperty("ItemDescription")]
        public string ItemDescription { get; set; }
    }
}
