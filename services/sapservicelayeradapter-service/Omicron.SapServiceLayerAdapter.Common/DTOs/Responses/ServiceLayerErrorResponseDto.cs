// <summary>
// <copyright file="ServiceLayerErrorResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// ServiceLayerErrorResponseDto class.
    /// </summary>
    public class ServiceLayerErrorResponseDto
    {
        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        /// <value>
        /// ServiceLayerErrorDetailDto Error.
        /// </value>
        [JsonProperty("error")]
        public ServiceLayerErrorDetailDto Error { get; set; }
    }
}
