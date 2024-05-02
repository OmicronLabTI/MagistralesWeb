// <summary>
// <copyright file="BatchInventoryGenEntryDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryGenEntry
{
    /// <summary>
    /// The class BatchInventoryGenEntryDto.
    /// </summary>
    public class BatchInventoryGenEntryDto : BatchNumbersDto
    {
        /// <summary>
        /// Gets or sets the ManufacturingDate.
        /// </summary>
        /// <value>ManufacturingDate.</value>
        [JsonProperty("ManufacturingDate")]
        public DateTime ManufacturingDate { get; set; }

        /// <summary>
        /// Gets or sets the ExpiryDate.
        /// </summary>
        /// <value>ExpiryDate.</value>
        [JsonProperty("ExpiryDate")]
        public DateTime ExpiryDate { get; set; }
    }
}