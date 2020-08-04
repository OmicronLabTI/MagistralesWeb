// <summary>
// <copyright file="ISapService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;

    /// <summary>
    /// The interface for sap.
    /// </summary>
    public interface ISapService
    {
        /// <summary>
        /// get the orders.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="docId">the doc id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetOrderDetails(int docId);

        /// <summary>
        /// Gets the orders with their detail.
        /// </summary>
        /// <param name="pedidosIds">the detail.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetPedidoWithDetail(List<int> pedidosIds);
    }
}
