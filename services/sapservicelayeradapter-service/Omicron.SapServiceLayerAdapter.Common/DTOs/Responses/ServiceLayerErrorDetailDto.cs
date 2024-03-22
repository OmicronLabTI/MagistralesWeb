// <summary>
// <copyright file="ServiceLayerErrorDetailDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// ServiceLayerErrorDetailDto class.
    /// </summary>
    public class ServiceLayerErrorDetailDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>
        /// Int Code.
        /// </value>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        /// <value>
        /// ServiceLayerErrorMessageDto Message.
        /// </value>
        [JsonProperty("message")]
        public ServiceLayerErrorMessageDto Message { get; set; }
    }
}
