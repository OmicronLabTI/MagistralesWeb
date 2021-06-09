// <summary>
// <copyright file="DoctorProfileModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Entities.Models.Experience
{
    using System;

    /// <summary>
    /// Model for profile info doctor.
    /// </summary>
    public class DoctorProfileModel
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
    }
}
