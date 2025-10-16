// <summary>
// <copyright file="ServiceLayerGenericMultipleResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    /// <summary>
    /// ServiceLayerGenericMultipleResultDto class.
    /// </summary>
    /// <typeparam name="T">The Type parameter.</typeparam>
    public class ServiceLayerGenericMultipleResultDto<T>
    {
        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        /// <value>
        /// ServiceLayerErrorDetailDto Error.
        /// </value>
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }
    }
}