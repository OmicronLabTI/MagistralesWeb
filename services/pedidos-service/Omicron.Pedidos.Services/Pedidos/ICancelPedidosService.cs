// <summary>
// <copyright file="ICancelPedidosService.cs" company="Axity">
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
    /// Contract for order cancellations.
    /// </summary>
    public interface ICancelPedidosService
    {
        /// <summary>
        /// Change sales order status to cancel.
        /// </summary>
        /// <param name="ordersToCancel">Update orders info.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CancelSalesOrder(List<OrderIdModel> ordersToCancel);

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="ordersToCancel">Orders to cancel.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CancelFabricationOrders(List<OrderIdModel> ordersToCancel);

        /// <summary>
        /// Cancels the delivery.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="deliveryIds">the delivery.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CancelDelivery(string type, List<CancelDeliveryPedidoModel> deliveryIds);
    }
}
