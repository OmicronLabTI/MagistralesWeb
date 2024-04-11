// <summary>
// <copyright file="IDoctorFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.Doctor
{
    /// <summary>
    /// Interface for Doctor Facade.
    /// </summary>
    public interface IDoctorFacade
    {
        /// <summary>
        /// Updates or add nes addresses.
        /// </summary>
        /// <param name="address">the address.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDoctorAddress(List<DoctorDeliveryAddressDto> address);

        /// <summary>
        /// Updates or add nes addresses.
        /// </summary>
        /// <param name="address">the address.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDoctorAddress(List<DoctorInvoiceAddressDto> address);

        /// <summary>
        /// Update Doctor Profile Info.
        /// </summary>
        /// <param name="doctorProfileInfo">Doctor Profile Info.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDoctorProfileInfo(DoctorProfileInfoDto doctorProfileInfo);

        /// <summary>
        /// Update Doctor Default Address.
        /// </summary>
        /// <param name="doctorDefaultAddress">Doctor Default Address.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDoctorDefaultAddress(DoctorDefaultAddressDto doctorDefaultAddress);
    }
}