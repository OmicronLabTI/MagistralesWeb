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
        /// <param name="logError">Log Error.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> PostAsync(object dataToSend, string route, string logError = null);

        /// <summary>
        /// Sends an HTTP PATCH request to the specified URL with the provided request body and returns the response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="requestBody">The request body to send with the PATCH request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response data of type <typeparamref name="T"/>.</returns>
        Task<ResultModel> PatchAsync(string url, string requestBody);
    }
}
