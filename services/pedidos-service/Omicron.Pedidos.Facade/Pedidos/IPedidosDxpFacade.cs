// <summary>
// <copyright file="IPedidosDxpFacade.cs" company="Axity">
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
    /// Gets the DXP facade.
    /// </summary>
    public interface IPedidosDxpFacade
    {
        /// <summary>
        /// returns the sale order that are delivered.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDeliveredPayments(List<int> ordersId);
    }
}
