// <summary>
// <copyright file="ISapDiApi.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.SapDiApi
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// interface for Di API.
    /// </summary>
    public interface ISapDiApi
    {
        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="pedidos">the orders.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> CreateFabOrder(object pedidos);
    }
}
