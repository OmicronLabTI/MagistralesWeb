// <summary>
// <copyright file="DoctorAddressModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    /// <summary>
    /// Class for the doctor academic info.
    /// </summary>
    public class DoctorAddressModel
    {
        /// <summary>
        /// Gets or sets the Id Address.
        /// </summary>
        /// <value>Id Address.</value>
        public string AddressId { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Degree Type.</value>
        public string DoctorId { get; set; }

        /// <summary>
        /// Gets or sets the establishment name.
        /// </summary>
        /// <value>Establishment name.</value>
        public string EtablishmentName { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public string BetweenStreets { get; set; }

        /// <summary>
        /// Gets or sets the doctor degree type.
        /// </summary>
        /// <value>Degree Type.</value>
        public string References { get; set; }

        /// <summary>
        /// Gets or sets the doctor degree type.
        /// </summary>
        /// <value>Degree Type.</value>
        public string ResponsibleDoctor { get; set; }
    }
}
