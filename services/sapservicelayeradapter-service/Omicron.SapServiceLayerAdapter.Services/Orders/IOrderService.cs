// <summary>
// <copyright file="IOrdersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Orders
{
    /// <summary>
    /// Interface for Orders Service.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Method to get the last generated order.
        /// </summary>
        /// <returns>Last generated order.</returns>
        Task<ResultModel> GetLastGeneratedOrder();
    }
}
