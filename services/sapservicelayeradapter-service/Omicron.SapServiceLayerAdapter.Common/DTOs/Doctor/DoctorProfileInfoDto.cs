// <summary>
// <copyright file="DoctorProfileInfoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Dto for doctor profile information.
    /// </summary>
    public class DoctorProfileInfoDto
    {
        /// <summary>
        /// Gets or sets Card Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string DoctorId { get; set; }

        /// <summary>
        /// Gets or sets the Birth Date.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the doctor phone number.
        /// </summary>
        /// <value>Phone Number.</value>
        public string PhoneNumber { get; set; }
    }
}
