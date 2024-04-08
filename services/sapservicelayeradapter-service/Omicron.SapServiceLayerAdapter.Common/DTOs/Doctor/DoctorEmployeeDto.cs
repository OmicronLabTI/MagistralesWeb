// <summary>
// <copyright file="DoctorEmployeeDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Class DoctorEmployeeDto.
    /// </summary>
    public class DoctorEmployeeDto
    {
        /// <summary>
        /// Gets or sets CardCode.
        /// </summary>
        /// <value>The CardCode.</value>
        [JsonProperty("CardCode")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value>The Name.</value>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        /// <value>The Email.</value>
        [JsonProperty("E_Mail")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets InternalCode.
        /// </summary>
        /// <value>The InternalCode.</value>
        [JsonProperty("InternalCode")]
        public int InternalCode { get; set; }
    }
}