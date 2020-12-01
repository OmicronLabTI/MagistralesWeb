// <summary>
// <copyright file="IQrFacade.cs" company="Axity">
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
    /// Generates the QR.
    /// </summary>
    public interface IQrFacade
    {
        /// <summary>
        /// Geths the QR url for the orders.
        /// </summary>
        /// <param name="ordersId">The orders id.</param>
        /// <returns>The urls for the QR.</returns>
        Task<ResultDto> CreateMagistralQr(List<int> ordersId);
    }
}
