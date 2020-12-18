// <summary>
// <copyright file="IQrService.cs" company="Axity">
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
    /// The service to create the Qr.
    /// </summary>
    public interface IQrService
    {
        /// <summary>
        /// Creates the QR and returns a Url by qr.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateMagistralQr(List<int> ordersId);

        /// <summary>
        /// Gets the orders qr.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateRemisionQr(List<int> ordersId);
    }
}
