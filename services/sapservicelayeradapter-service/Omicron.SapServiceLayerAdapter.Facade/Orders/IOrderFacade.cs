// <summary>
// <copyright file="IOrderFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.Orders
{
    /// <summary>
    /// Interface IOrdersFacade.
    /// </summary>
    public interface IOrderFacade
    {
        /// <summary>
        /// Method to get the last generated order.
        /// </summary>
        /// <returns>Last generated order.</returns>
        Task<ResultDto> GetLastGeneratedOrder();

        /// <summary>
        /// Close sample orders.
        /// </summary>
        /// <param name="sampleOrders">Sample orders to close.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CloseSampleOrders(List<CloseSampleOrderDto> sampleOrders);
    }
}
