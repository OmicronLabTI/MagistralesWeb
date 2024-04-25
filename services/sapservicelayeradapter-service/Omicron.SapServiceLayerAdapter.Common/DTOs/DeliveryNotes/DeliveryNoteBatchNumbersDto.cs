// <summary>
// <copyright file="DeliveryNoteBatchNumbersDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes
{
    /// <summary>
    /// The class for the DeliveryNoteBatchNumbersDto.
    /// </summary>
    public class DeliveryNoteBatchNumbersDto
    {
        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        /// <value>Quantity.</value>
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the BatchNumber.
        /// </summary>
        /// <value>BatchNumber.</value>
        [JsonProperty("BatchNumber")]
        public string BatchNumber { get; set; }
    }
}