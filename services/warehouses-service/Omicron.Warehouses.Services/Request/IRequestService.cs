// <summary>
// <copyright file="IRequestService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Request
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Contract for request service.
    /// </summary>
    public interface IRequestService
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Requests data.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        Task<ResultModel> CreateRawMaterialRequest(string userId, RawMaterialRequestModel request);

        /// <summary>
        /// Update raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Requests data.</param>
        /// <returns>List with successfuly and failed updates.</returns>
        Task<ResultModel> UpdateRawMaterialRequest(string userId, RawMaterialRequestModel request);

        /// <summary>
        /// Get a raw material request for production order id.
        /// </summary>
        /// <param name="productionOrderId">The production order id.</param>
        /// <returns>Raw material request.</returns>
        Task<ResultModel> GetRawMaterialRequestByProductionOrderId(int productionOrderId);
    }
}
