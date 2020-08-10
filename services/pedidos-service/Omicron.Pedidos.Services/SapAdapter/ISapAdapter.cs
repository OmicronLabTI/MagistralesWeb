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
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">route to send.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> PostSapAdapter(object dataToSend, string route);

        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetSapAdapter(string route);
    }
}
