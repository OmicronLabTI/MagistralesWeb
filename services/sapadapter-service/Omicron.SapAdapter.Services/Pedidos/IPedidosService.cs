// <summary>
// <copyright file="IPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// The pedido service.
    /// </summary>
    public interface IPedidosService
    {
        /// <summary>
        /// Gets the pedidos.
        /// </summary>
        /// <param name="listPedidos">the pedidos.</param>
        /// <param name="route">The route.</param>
        /// <returns>the qfb.</returns>
        Task<ResultDto> GetUserPedidos(List<int> listPedidos, string route);

        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserPedidos(string route);

        /// <summary>
        /// Makes a get to pedidos.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetPedidosService(string route);
    }
}
