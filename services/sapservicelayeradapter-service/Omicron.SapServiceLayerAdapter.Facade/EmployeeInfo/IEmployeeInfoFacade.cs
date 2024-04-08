// <summary>
// <copyright file="IEmployeeInfoFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.EmployeeInfo
{
    /// <summary>
    /// Interface for Invoice Facade.
    /// </summary>
    public interface IEmployeeInfoFacade
    {
        /// <summary>
        /// Class for Update Adviser Profile Info.
        /// </summary>
        /// <param name="adviserProfileInfo">Adviser profile info.</param>
        /// <returns>Result.</returns>
        Task<ResultDto> UpdateAdviserProfileInfo(AdviserProfileInfoDto adviserProfileInfo);
    }
}
