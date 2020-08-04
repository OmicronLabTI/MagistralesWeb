// <summary>
// <copyright file="ISapAdapter.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.SapAdapter
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// the list of pedidos.
    /// </summary>
    public interface ISapAdapter
    {
        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="listPedidos">the orders.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> GetSapOrders(List<int> listPedidos);
    }
}
