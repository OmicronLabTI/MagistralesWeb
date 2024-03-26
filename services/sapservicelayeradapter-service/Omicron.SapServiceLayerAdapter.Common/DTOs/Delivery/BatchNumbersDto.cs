// <summary>
// <copyright file="BatchNumbersDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Delivery
{
    /// <summary>
    /// The class for almacen batch.
    /// </summary>
    public class BatchNumbersDto
    {
        /// <summary>
        /// Gets or sets the BatchNumber.
        /// </summary>
        /// <value>BatchNumber.</value>
        [JsonProperty("BatchNumber")]
        public string BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        /// <value>Comments.</value>
        [JsonProperty("Quantity")]
        public double Quantity { get; set; }
    }
}
