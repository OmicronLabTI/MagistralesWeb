// <summary>
// <copyright file="IEmployeeInfoService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.EmployeesInfo
{
    /// <summary>
    /// Interface to Employee Info Service.
    /// </summary>
    public interface IEmployeeInfoService
    {
        /// <summary>
        /// Update Adviser Profile Info.
        /// </summary>
        /// <param name="adviserProfileInfo">The Adviser Profile Info.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateAdviserProfileInfo(AdviserProfileInfoDto adviserProfileInfo);
    }
}
