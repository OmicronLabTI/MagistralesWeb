// <summary>
// <copyright file="AdviserProfileInfoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.EmployeesInfo
{
    /// <summary>
    /// Advisor profile info dto.
    /// </summary>
    public class AdviserProfileInfoDto
    {
        /// <summary>
        /// Gets or sets the adviser id.
        /// </summary>
        /// <value>The adviser id.</value>
        public string AdviserId { get; set; }

        /// <summary>
        /// Gets or sets the Birth Date.
        /// </summary>
        /// <value>Birth Date.</value>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the adviser phone number.
        /// </summary>
        /// <value>Phone Number.</value>
        public string PhoneNumber { get; set; }
    }
}
