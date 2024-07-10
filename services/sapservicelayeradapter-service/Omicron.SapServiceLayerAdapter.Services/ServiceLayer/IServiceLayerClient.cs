// <summary>
// <copyright file="IServiceLayerClient.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapServiceLayerAdapter.Services.ServiceLayer
{
    /// <summary>
    /// Interface representing a generic service layer client.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public interface IServiceLayerClient
    {
        /// <summary>
        /// Sends an HTTP GET request to the specified URL and returns the response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response data of type <typeparamref name="T"/>.</returns>
        Task<ResultModel> GetAsync(string url);

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the provided request body and returns the response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="requestBody">The request body to send with the POST request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response data of type <typeparamref name="T"/>.</returns>
        Task<ResultModel> PostAsync(string url, string requestBody);

        /// <summary>
        /// Sends an HTTP PATCH request to the specified URL with the provided request body and returns the response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="requestBody">The request body to send with the PATCH request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response data of type <typeparamref name="T"/>.</returns>
        Task<ResultModel> PatchAsync(string url, string requestBody);

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URL and returns the response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response data of type <typeparamref name="T"/>.</returns>
        Task<ResultModel> DeleteAsync(string url);

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the provided request body and returns the response as an asynchronous operation.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="requestBody">The request body to send with the PATCH request.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response data of type <typeparamref name="T"/>.</returns>
        Task<ResultModel> PutAsync(string url, string requestBody);
    }
}