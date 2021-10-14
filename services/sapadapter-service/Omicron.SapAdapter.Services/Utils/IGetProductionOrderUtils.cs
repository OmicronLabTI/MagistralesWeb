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
    using Omicron.SapAdapter.Entities.Model.BusinessModels;

    /// <summary>
    /// Interface for the producion utils.
    /// </summary>
    public interface IGetProductionOrderUtils
    {
        /// <summary>
        /// Gets the orders on local data.
        /// </summary>
        /// <param name="parameters">the paramd.</param>
        /// <param name="orders">the orders.</param>
        /// <returns>the data.</returns>
        Task<List<OrdenFabricacionModel>> GetSapLocalProdOrders(Dictionary<string, string> parameters, List<OrdenFabricacionModel> orders);

        /// <summary>
        /// Completes the order.
        /// </summary>
        /// <param name="listOrders">the data.</param>
        /// <returns>the data to return.</returns>
        Task<List<OrdenFabricacionModel>> CompleteOrder(List<OrdenFabricacionModel> listOrders);

        /// <summary>
        /// Gets the batches component.
        /// </summary>
        /// <param name="ordersId">the ids.</param>
        /// <returns>the data.</returns>
        Task<List<string>> GetIncompleteProducts(List<int> ordersId);
    }
}
