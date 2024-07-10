// <summary>
// <copyright file="IDoctorService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Doctors
{
    /// <summary>
    /// Interface for Doctor Service.
    /// </summary>
    public interface IDoctorService
    {
        /// <summary>
        /// Updates or add new addresses.
        /// </summary>
        /// <param name="addresses">the addresses.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorDeliveryAddressDto> addresses);

        /// <summary>
        /// Updates or add new addresses.
        /// </summary>
        /// <param name="addresses">the addresses.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorInvoiceAddressDto> addresses);

        /// <summary>
        /// Update Doctor Profile Info.
        /// </summary>
        /// <param name="doctorProfileInfo">Doctor Profile Info.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDoctorProfileInfo(DoctorProfileInfoDto doctorProfileInfo);

        /// <summary>
        /// Update Doctor Default Address.
        /// </summary>
        /// <param name="doctorDefaultAddress">Doctor Default Address.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDoctorDefaultAddress(DoctorDefaultAddressDto doctorDefaultAddress);
    }
}