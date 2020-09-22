// <summary>
// <copyright file="IReportingService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Interface User Service.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Method for submit request.
        /// </summary>
        /// <param name="request">Request to submit.</param>
        /// <returns>User list.</returns>
        Task<bool> SubmitRequest(RawMaterialRequestModel request);
    }
}
