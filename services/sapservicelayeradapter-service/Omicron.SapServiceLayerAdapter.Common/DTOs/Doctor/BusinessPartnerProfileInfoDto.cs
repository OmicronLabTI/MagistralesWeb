// <summary>
// <copyright file="BusinessPartnerProfileInfoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Dto for Business Partner Profile Info Dto.
    /// </summary>
    public class BusinessPartnerProfileInfoDto
    {
        /// <summary>
        /// Gets or sets the Birth Date.
        /// </summary>
        /// <value>The user that is assigning.</value>
        [JsonProperty("U_Fecha_Nacimiento")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the doctor phone number.
        /// </summary>
        /// <value>Phone Number.</value>
        [JsonProperty("Phone1")]
        public string PhoneNumber { get; set; }
    }
}
