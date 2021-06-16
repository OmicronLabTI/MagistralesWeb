// <summary>
// <copyright file="DoctorProfileDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models.Experience
{
    using System;

    /// <summary>
    /// Dto for profile info doctor.
    /// </summary>
    public class DoctorProfileDto
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
