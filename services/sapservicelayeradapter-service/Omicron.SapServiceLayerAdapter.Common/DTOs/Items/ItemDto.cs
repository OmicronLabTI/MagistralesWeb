// <summary>
// <copyright file="ItemDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Items
{
    /// <summary>
    /// the class ItemDto.
    /// </summary>
    public class ItemDto
    {
        /// <summary>
        /// Gets or sets the ItemCode.
        /// </summary>
        /// <value>ItemCode.</value>
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the ItemName.
        /// </summary>
        /// <value>ItemName.</value>
        [JsonProperty("ItemName")]
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the ManageBatchNumbers.
        /// </summary>
        /// <value>ManageBatchNumbers.</value>
        [JsonProperty("ManageBatchNumbers")]
        public string ManageBatchNumbers { get; set; }

        /// <summary>
        /// Gets or sets the ItemWarehouseInfoCollection.
        /// </summary>
        /// <value>ItemWarehouseInfoCollection.</value>
        public List<ItemWarehouseInfoDto> ItemWarehouseInfoCollection { get; set; }
    }
}