// <summary>
// <copyright file="DoctorElectronicProtocolDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Class DoctorElectronicProtocolDto.
    /// </summary>
    public class DoctorElectronicProtocolDto
    {
        /// <summary>
        /// Gets or sets ProtocolCode.
        /// </summary>
        /// <value>The ProtocolCode.</value>
        [JsonProperty("ProtocolCode")]
        public string ProtocolCode { get; set; }

        /// <summary>
        /// Gets or sets MappingID.
        /// </summary>
        /// <value>The MappingID.</value>
        [JsonProperty("MappingID")]
        public int? MappingID { get; set; }
    }
}