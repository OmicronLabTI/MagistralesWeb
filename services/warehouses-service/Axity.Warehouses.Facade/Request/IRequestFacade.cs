// <summary>
// <copyright file="IRequestFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Axity.Warehouses.Facade.Request
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Axity.Warehouses.Dtos.Model;

    /// <summary>
    /// interfaces for the pedidos.
    /// </summary>
    public interface IRequestFacade
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requests">Requests data.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        Task<ResultDto> CreateRawMaterialRequest(string userId, List<RawMaterialRequestDto> requests);

        /// <summary>
        /// Update raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requests">Requests data.</param>
        /// <returns>List with successfuly and failed updates.</returns>
        Task<ResultDto> UpdateRawMaterialRequest(string userId, List<RawMaterialRequestDto> requests);
    }
}
