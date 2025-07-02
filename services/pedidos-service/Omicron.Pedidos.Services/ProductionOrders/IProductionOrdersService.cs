// <summary>
// <copyright file="IProductionOrdersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.ProductionOrders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// Interface for ProductionOrdersService.
    /// </summary>
    public interface IProductionOrdersService
    {
        /// <summary>
        /// Finalize Production Orders Async.
        /// </summary>
        /// <param name="productionOrdersToFinalize">Production Orders To Finalize.</param>
        /// <returns>Process Result.</returns>urns>
        Task<ResultModel> FinalizeProductionOrdersAsync(List<FinalizeProductionOrderModel> productionOrdersToFinalize);
    }
}
