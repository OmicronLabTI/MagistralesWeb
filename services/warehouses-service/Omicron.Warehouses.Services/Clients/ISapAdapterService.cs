// <summary>
// <copyright file="ISapAdapterService.cs" company="Axity">
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
    public interface ISapAdapterService
    {
        /// <summary>
        /// Method for get production orders by criterial.
        /// </summary>
        /// <param name="salesOrderIds">Sales orders.</param>
        /// <param name="productionOrderIds">ProductionOrders.</param>
        /// <returns>Production orders.</returns>
        Task<List<ProductionOrderModel>> GetProductionOrdersByCriterial(List<int> salesOrderIds, List<int> productionOrderIds);
    }
}
