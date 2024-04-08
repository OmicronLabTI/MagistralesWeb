// <summary>
// <copyright file="EmployeeInfoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.EmployeesInfo
{
    /// <summary>
    /// Clas to mployee Info Dto.
    /// </summary>
    public class EmployeeInfoDto
    {
        /// <summary>
        /// Gets or sets the Birth Date.
        /// </summary>
        /// <value>Birth Date.</value>
        [JsonProperty("DateOfBirth")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the adviser phone number.
        /// </summary>
        /// <value>Phone Number.</value>
        [JsonProperty("OfficePhone")]
        public string PhoneNumber { get; set; }
    }
}
