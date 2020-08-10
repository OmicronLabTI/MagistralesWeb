// <summary>
// <copyright file="IPedidoFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Dtos.Models;

    /// <summary>
    /// interfaces for the pedidos.
    /// </summary>
    public interface IPedidoFacade
    {
        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="orderDto">the pedidos list.</param>
        /// <returns>the result.</returns>
        Task<ResultDto> ProcessOrders(ProcessOrderDto orderDto);

        /// <summary>
        /// returns the list of userOrder by sales order.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrderBySalesOrder(List<int> listIds);

        /// <summary>
        /// Gets the orders of a specific QFB (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the list to returns.</returns>
        Task<ResultDto> GetFabOrderByUserID(string userId);

        /// <summary>
        /// Gets the user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrdersByUserId(List<string> listIds);
    }
}
