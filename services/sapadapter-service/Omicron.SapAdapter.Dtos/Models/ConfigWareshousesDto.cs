// <summary>
// <copyright file="ConfigWareshousesDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Dtos.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Config Wareshouses Dto.
    /// </summary>
    public class ConfigWareshousesDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [JsonProperty("products")]
        public List<string> Products { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [JsonProperty("manufacturers")]
        public List<string> Manufacturers { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [JsonProperty("warehouses")]
        public List<string> Warehouses { get; set; }
    }
}
