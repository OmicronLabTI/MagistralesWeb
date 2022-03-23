// <summary>
// <copyright file="DoctorPrescriptionInfoModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    /// <summary>
    /// Class for the doctor prescription info.
    /// </summary>
    public class DoctorPrescriptionInfoModel
    {
        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the doctor name.
        /// </summary>
        /// <value>Doctor name.</value>
        public string DoctorName { get; set; }
    }
}
