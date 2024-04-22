// <summary>
// <copyright file="ItemWarehouseInfoDto.cs" company="Axity">
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
    public class ItemWarehouseInfoDto
    {
        /// <summary>
        /// Gets or sets the InStock.
        /// </summary>
        /// <value>InStock.</value>
        [JsonProperty("InStock")]
        public double InStock { get; set; }

        /// <summary>
        /// Gets or sets the WarehouseCode.
        /// </summary>
        /// <value>WarehouseCode.</value>
        [JsonProperty("WarehouseCode")]
        public string WarehouseCode { get; set; }
    }
}