// <summary>
// <copyright file="IBusquedaPedidoService.cs" company="Axity">
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
    /// interface to look for pedidos.
    /// </summary>
    public interface IBusquedaPedidoService
    {
        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">the critetira.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrders(Dictionary<string, string> parameters);
    }
}
