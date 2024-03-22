// <summary>
// <copyright file="ServiceLayerResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    /// <summary>
    /// Class fro service layer response.
    /// </summary>
    public class ServiceLayerResponseDto
    {
        /// <summary>
        /// Gets or sets the Metadata.
        /// </summary>
        /// <value>Metadata.</value>
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }

        /// <summary>
        /// Gets or sets the CardCode.
        /// </summary>
        /// <value>CardCode.</value>
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
