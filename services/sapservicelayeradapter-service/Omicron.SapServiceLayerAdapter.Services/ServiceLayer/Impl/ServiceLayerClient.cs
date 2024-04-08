// <summary>
// <copyright file="ServiceLayerClient.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.ServiceLayer.Impl
{
    /// <summary>
    /// Class representing a generic service layer client.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class ServiceLayerClient : IServiceLayerClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLayerClient"/> class with the specified HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for sending requests.</param>
        /// <param name="connection">The IServiceLayerAuth instance to use for get sessionId.</param>
        /// <param name="logger">The ILogger instance to logg information.</param>
        public ServiceLayerClient(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetAsync(string url)
        {
            ResultModel result;
            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PutAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PUT"), url);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.SendAsync(request))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger, requestBody);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PatchAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.SendAsync(request))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger, requestBody);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PostAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.PostAsync(url, content))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger, requestBody);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> DeleteAsync(string url)
        {
            ResultModel result;
            using (var response = await this.httpClient.DeleteAsync(url))
            {
                result = await ServiceUtils.GetServiceLayerResponse(response, this.logger);
            }

            return result;
        }
    }
}