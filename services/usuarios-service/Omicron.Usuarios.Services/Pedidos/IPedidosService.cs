// <summary>
// <copyright file="IPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Pedidos
{
    using System.Threading.Tasks;
    using Omicron.Usuarios.Entities.Model;

    /// <summary>
    /// Makes calls for pedido service.
    /// </summary>
    public interface IPedidosService
    {
        /// <summary>
        /// Makes a post to pedidos.
        /// </summary>
        /// <param name="dataToSend">the data to send.</param>
        /// <param name="route">the route.</param>
        /// <returns>the data retunred.</returns>
        Task<ResultModel> PostPedidos(object dataToSend, string route);
    }
}
