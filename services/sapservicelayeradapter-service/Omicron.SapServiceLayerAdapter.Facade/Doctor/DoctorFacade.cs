// <summary>
// <copyright file="DoctorFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.Doctor
{
    /// <summary>
    /// Class for Doctor Facade.
    /// </summary>
    public class DoctorFacade : IDoctorFacade
    {
        private readonly IMapper mapper;
        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="doctorService">Doctor Service.</param>
        public DoctorFacade(IMapper mapper, IDoctorService doctorService)
        {
            this.mapper = mapper;
            this.doctorService = doctorService.ThrowIfNull(nameof(doctorService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDoctorAddress(List<DoctorDeliveryAddressDto> address)
            => this.mapper.Map<ResultDto>(await this.doctorService.UpdateDoctorDeliveryAddress(address));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDoctorAddress(List<DoctorInvoiceAddressDto> address)
            => this.mapper.Map<ResultDto>(await this.doctorService.UpdateDoctorDeliveryAddress(address));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDoctorDefaultAddress(DoctorDefaultAddressDto doctorDefaultAddress)
            => this.mapper.Map<ResultDto>(await this.doctorService.UpdateDoctorDefaultAddress(doctorDefaultAddress));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDoctorProfileInfo(DoctorProfileInfoDto doctorProfileInfo)
            => this.mapper.Map<ResultDto>(await this.doctorService.UpdateDoctorProfileInfo(doctorProfileInfo));
    }
}