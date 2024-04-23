// <summary>
// <copyright file="ISapServiceLayerAdapterService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.SapServiceLayerAdapter
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// class for the saporderadapter service.
    /// </summary>
    public interface ISapServiceLayerAdapterService
    {
        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">route to send.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> PostAsync(object dataToSend, string route);
    }
}
