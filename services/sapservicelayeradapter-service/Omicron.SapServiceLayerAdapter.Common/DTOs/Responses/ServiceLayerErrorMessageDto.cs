// <summary>
// <copyright file="ServiceLayerErrorMessageDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    /// <summary>
    /// ServiceLayerErrorMessageDto class.
    /// </summary>
    public class ServiceLayerErrorMessageDto
    {
        /// <summary>
        /// Gets or sets Lang.
        /// </summary>
        /// <value>
        /// String Lang.
        /// </value>
        [JsonProperty("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        /// <value>
        /// String Value.
        /// </value>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
