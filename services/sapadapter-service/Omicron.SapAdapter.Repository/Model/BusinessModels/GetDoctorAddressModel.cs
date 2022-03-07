// <summary>
// <copyright file="GetDoctorAddressModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    /// <summary>
    /// Class to save the payment card info of doctor.
    /// </summary>
    public class GetDoctorAddressModel
    {
        /// <summary>
        /// Gets or sets the nucard code.
        /// </summary>
        /// <value>Card code.</value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the nucard code.
        /// </summary>
        /// <value>Card code.</value>
        public string AddressId { get; set; }
    }
}
