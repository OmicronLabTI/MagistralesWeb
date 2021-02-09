// <summary>
// <copyright file="IAlmacenService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.AlmacenService
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// Interface to call almacen service.
    /// </summary>
    public interface IAlmacenService
    {
        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetAlmacenData(string route);

        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <param name="dataToSend">The data to send.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> PostAlmacenData(string route, object dataToSend);
    }
}
