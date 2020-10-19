// <summary>
// <copyright file="IGetProductionOrderUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;

    /// <summary>
    /// Interface for the producion utils.
    /// </summary>
    public interface IGetProductionOrderUtils
    {
        /// <summary>
        /// method to look for orders.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <param name="dateFilter">the date filter.</param>
        /// <returns>the orders.</returns>
        Task<List<OrdenFabricacionModel>> GetSapDbProdOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter);

        /// <summary>
        /// Gets the orders on local data.
        /// </summary>
        /// <param name="parameters">the paramd.</param>
        /// <param name="dateFilter">the filter.</param>
        /// <param name="orders">the orders.</param>
        /// <returns>the data.</returns>
        List<OrdenFabricacionModel> GetSapLocalProdOrders(Dictionary<string, string> parameters, Dictionary<string, DateTime> dateFilter, List<OrdenFabricacionModel> orders);

        /// <summary>
        /// Completes the order.
        /// </summary>
        /// <param name="listOrders">the data.</param>
        /// <returns>the data to return.</returns>
        Task<List<OrdenFabricacionModel>> CompleteOrder(List<OrdenFabricacionModel> listOrders);
    }
}
