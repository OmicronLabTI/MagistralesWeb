// <summary>
// <copyright file="IRequestFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Facade.Request
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Warehouses.Dtos.Model;

    /// <summary>
    /// interfaces for the pedidos.
    /// </summary>
    public interface IRequestFacade
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Request data.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        Task<ResultDto> CreateRawMaterialRequest(string userId, RawMaterialRequestDto request);

        /// <summary>
        /// Update raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Requests data.</param>
        /// <returns>List with successfuly and failed updates.</returns>
        Task<ResultDto> UpdateRawMaterialRequest(string userId, RawMaterialRequestDto request);

        /// <summary>
        /// Get a raw material request.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>The material request.</returns>
        Task<ResultDto> GetRawMaterialRequest(int productionOrderId);

        /// <summary>
        /// Get a raw material pre-request.
        /// </summary>
        /// <param name="salesOrders">the sales order ids.</param>
        /// <param name="productionOrders">the production order ids.</param>
        /// <returns>The material pre-request.</returns>
        Task<ResultDto> GetRawMaterialPreRequest(List<int> salesOrders, List<int> productionOrders);
    }
}
