// <summary>
// <copyright file="IPedidosDxpService.cs" company="Axity">
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
    public interface IPedidosDxpService
    {
        /// <summary>
        /// Method to get orders active.
        /// </summary>
        /// <param name="ordersid">The parameters.</param>
        /// <returns>List of orders.</returns>
        Task<ResultModel> GetOrdersActive(List<int> ordersid);
    }
}
