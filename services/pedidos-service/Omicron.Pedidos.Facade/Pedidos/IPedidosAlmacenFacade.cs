// <summary>
// <copyright file="IPedidosAlmacenFacade.cs" company="Axity">
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
    public interface IPedidosAlmacenFacade
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForAlmacen();

        /// <summary>
        /// Updates the user orders.
        /// </summary>
        /// <param name="userOrders">The orders.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateUserOrders(List<UserOrderDto> userOrders);

        /// <summary>
        /// Get the orders for delivery.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForDelivery();

        /// <summary>
        /// Gets the deliveries for the invoices.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForInvoice();

        /// <summary>
        /// Gets the order for the packages by type.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForPackages(Dictionary<string, string> parameters);

        /// <summary>
        /// Updates whe the package is sent.
        /// </summary>
        /// <param name="usersToUpdate">the data to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateSentOrders(List<UserOrderDto> usersToUpdate);

        /// <summary>
        /// Gets the totals for graphs.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetAlmacenGraphData(Dictionary<string, string> parameters);
    }
}
