// <summary>
// <copyright file="InventoryGenEntryLineDto.cs" company="Axity">
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
    public class InventoryGenEntryLineDto
    {
        /// <summary>
        /// Gets or sets the BaseEntry.
        /// </summary>
        /// <value>BaseEntry.</value>
        [JsonProperty("BaseEntry")]
        public int BaseEntry { get; set; }

        /// <summary>
        /// Gets or sets the BaseType.
        /// </summary>
        /// <value>BaseType.</value>
        [JsonProperty("BaseType")]
        public int BaseType { get; set; }

        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        /// <value>Quantity.</value>
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the TransactionType.
        /// </summary>
        /// <value>TransactionType.</value>
        [JsonProperty("TransactionType")]
        public string TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the WarehouseCode.
        /// </summary>
        /// <value>WarehouseCode.</value>
        [JsonProperty("WarehouseCode")]
        public string WarehouseCode { get; set; }

        /// <summary>
        /// Gets or sets the BatchNumbers.
        /// </summary>
        /// <value>BatchNumbers.</value>
        [JsonProperty("BatchNumbers")]
        public List<BatchInventoryGenEntryDto> BatchNumbers { get; set; }
    }
}