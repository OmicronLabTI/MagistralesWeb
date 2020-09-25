// <summary>
// <copyright file="IProcessOrdersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// interfaces for process orders.
    /// </summary>
    public interface IProcessOrdersService
    {
        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosId">the ids of the orders.</param>
        /// <returns>the result.</returns>
        Task<ResultModel> ProcessOrders(ProcessOrderModel pedidosId);

        /// <summary>
        /// Process by order.
        /// </summary>
        /// <param name="processByOrder">the orders.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> ProcessByOrder(ProcessByOrderModel processByOrder);
    }
}
