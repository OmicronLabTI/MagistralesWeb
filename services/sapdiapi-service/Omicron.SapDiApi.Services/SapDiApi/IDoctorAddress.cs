// <summary>
// <copyright file="IDoctorAddress.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>


namespace Omicron.SapDiApi.Services.SapDiApi
{
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Entities.Models.Experience;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for the doctor addresses.
    /// </summary>
    public interface IDoctorAddress
    {
        /// <summary>
        /// Updates or add new addresses.
        /// </summary>
        /// <param name="addresses">the addresses.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorDeliveryAddressModel> addresses);

        /// <summary>
        /// Updates or add new addresses.
        /// </summary>
        /// <param name="addresses">the addresses.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorInvoiceAddressModel> addresses);
    }
}
