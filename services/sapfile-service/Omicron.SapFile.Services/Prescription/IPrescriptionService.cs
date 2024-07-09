// <summary>
// <copyright file="IPrescriptionService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.Prescription
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapFile.Entities.Models;
    using Omicron.SapFile.Dtos.Models;

    /// <summary>
    /// Interface to Prescription Service.
    /// </summary>
    public interface IPrescriptionService
    {
        /// <summary>
        /// Save recipe to server.
        /// </summary>
        /// <param name="prescriptionUrls">Prescription urls to save.</param>
        /// <returns>Result model.</returns>
        Task<ResultModel> SavePresciptionToServer(List<PrescriptionServerRequestDto> prescriptionUrls);
    }
}
