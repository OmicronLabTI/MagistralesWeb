// <summary>
// <copyright file="EmployeeInfoFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.EmployeeInfo.Impl
{
    /// <summary>
    /// Interface for Invoice Facade.
    /// </summary>
    public class EmployeeInfoFacade : IEmployeeInfoFacade
    {
        private readonly IMapper mapper;
        private readonly IEmployeeInfoService employeeInfoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeInfoFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="employeeInfoService">Employee info service.</param>
        public EmployeeInfoFacade(IMapper mapper, IEmployeeInfoService employeeInfoService)
        {
            this.mapper = mapper;
            this.employeeInfoService = employeeInfoService.ThrowIfNull(nameof(employeeInfoService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateAdviserProfileInfo(AdviserProfileInfoDto adviserProfileInfo)
            => this.mapper.Map<ResultDto>(await this.employeeInfoService.UpdateAdviserProfileInfo(adviserProfileInfo));
    }
}
