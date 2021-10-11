// <summary>
// <copyright file="IDoctorProfileService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using System.Threading.Tasks;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Entities.Models.Experience;

    /// <summary>
    /// Interface for the profile service.
    /// </summary>
    public interface IDoctorProfileService
    {
        /// <summary>
        /// Set the profile info by doctor.
        /// </summary>
        /// <param name="profileModel">Profile Info</param>
        /// <returns>The data.</returns>
        Task<ResultModel> UpdateDoctorProfileInfo(DoctorProfileModel profileModel);
    }
}
