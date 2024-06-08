// <summary>
// <copyright file="ShippingTypesResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ShippingTypes
{
    /// <summary>
    /// The class for the ShippingTypesResponseDto.
    /// </summary>
    public class ShippingTypesResponseDto
    {
        /// <summary>
        /// Gets or sets the Transport Code.
        /// </summary>
        /// <value>Transport Code.</value>
        [JsonProperty("Code")]
        public short TransportCode { get; set; }

        /// <summary>
        /// Gets or sets the Transport Name.
        /// </summary>
        /// <value>Transport Name.</value>
        [JsonProperty("Name")]
        public string TransportName { get; set; }
    }
}
