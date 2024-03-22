// <summary>
// <copyright file="ServiceLayerAuthResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// ServiceLayerAuthResponseDto class.
    /// </summary>
    public class ServiceLayerAuthResponseDto
    {
        /// <summary>
        /// Gets or sets SessionId.
        /// </summary>
        /// <value>
        /// String SessionId.
        /// </value>
        [JsonProperty("SessionId")]
        public string SessionId { get; set; }
    }
}
