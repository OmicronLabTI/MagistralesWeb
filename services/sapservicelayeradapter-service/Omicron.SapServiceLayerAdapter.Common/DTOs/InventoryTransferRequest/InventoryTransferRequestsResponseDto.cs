// <summary>
// <copyright file="InventoryTransferRequestsResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryTransferRequest
{
    /// <summary>
    /// Class for Inventory Transfer Requests Response Dto.
    /// </summary>
    public class InventoryTransferRequestsResponseDto : InventoryTransferRequestsDto
    {
        /// <summary>
        /// Gets or sets DocEntry.
        /// </summary>
        /// <value>The DocEntry.</value>
        [JsonProperty("DocEntry")]
        public int DocEntry { get; set; }
    }
}
