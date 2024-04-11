// <summary>
// <copyright file="BusinessParterDefaultAddressDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Dto for BusinessParterDefaultAddressDto.
    /// </summary>
    public class BusinessPartnerDefaultAddressDto
    {
        /// <summary>
        /// Gets or sets ShipToDefault.
        /// </summary>
        /// <value>The ShipToDefault.</value>
        [JsonProperty("ShipToDefault")]
        public string ShipToDefault { get; set; }

        /// <summary>
        /// Gets or sets ShipToDefault.
        /// </summary>
        /// <value>The ShipToDefault.</value>
        [JsonProperty("BilltoDefault")]
        public string BillToDefault { get; set; }
    }
}
