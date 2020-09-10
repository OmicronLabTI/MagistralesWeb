// <summary>
// <copyright file="IClosePedidosService.cs" company="Axity">
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
    /// Contract for close order.
    /// </summary>
    public interface IClosePedidosService
    {
        /// <summary>
        /// Change sales order status to close.
        /// </summary>
        /// <param name="ordersToClose">Orders to close.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CloseSalesOrder(List<OrderIdModel> ordersToClose);

        /// <summary>
        /// Close fabrication orders.
        /// </summary>
        /// <param name="ordersToClose">Orders to close.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CloseFabricationOrders(List<CloseProductionOrderModel> ordersToClose);
    }
}
