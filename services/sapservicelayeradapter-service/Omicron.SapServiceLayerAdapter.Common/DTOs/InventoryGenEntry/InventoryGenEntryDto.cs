// <summary>
// <copyright file="InventoryGenEntryDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryGenEntry
{
    /// <summary>
    /// the class InventoryGenEntryDto.
    /// </summary>
    public class InventoryGenEntryDto
    {
        /// <summary>
        /// Gets or sets the CardCode.
        /// </summary>
        /// <value>CardCode.</value>
        [JsonProperty("CardCode")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the DocumentLines.
        /// </summary>
        /// <value>DocumentLines.</value>
        [JsonProperty("DocumentLines")]
        public List<InventoryGenEntryLineDto> DocumentLines { get; set; }
    }
}