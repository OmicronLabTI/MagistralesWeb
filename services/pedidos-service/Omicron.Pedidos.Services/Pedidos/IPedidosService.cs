// <summary>
// <copyright file="IPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// The pedidos service interface.
    /// </summary>
    public interface IPedidosService
    {
        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosId">the ids of the orders.</param>
        /// <returns>the result.</returns>
        Task<ResultModel> ProcessOrders(ProcessOrderModel pedidosId);

        /// <summary>
        /// returns the orders ids.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds);

        /// <summary>
        /// Gets the QFB orders (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabOrderByUserID(string userId);

        /// <summary>
        /// Gets the list of user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrdersByUserId(List<string> listIds);
    }
}
