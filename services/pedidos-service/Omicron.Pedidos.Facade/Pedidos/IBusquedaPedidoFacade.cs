// <summary>
// <copyright file="IBusquedaPedidoFacade.cs" company="Axity">
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
    /// Interface for look up pedidos.
    /// </summary>
    public interface IBusquedaPedidoFacade
    {
        /// <summary>
        /// Gets the order by parameters.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrders(Dictionary<string, string> parameters);
    }
}
