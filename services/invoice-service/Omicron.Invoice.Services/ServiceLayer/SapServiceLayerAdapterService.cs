// <summary>
// <copyright file="SapServiceLayerAdapterService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.ServiceLayer
{
    /// <summary>
    /// Class representing a generic service layer client.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class SapServiceLayerAdapterService : ISapServiceLayerAdapterService
    {
        private readonly HttpClient httpClient;
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapServiceLayerAdapterService"/> class with the specified HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for sending requests.</param>
        /// <param name="logger">The ILogger instance to logg information.</param>
        public SapServiceLayerAdapterService(HttpClient httpClient, Serilog.ILogger logger)
        {
            this.httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> PostAsync(string url, string requestBody)
        {
            ResultDto result;
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.PostAsync(url, content))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Almacen - PostAsync - Error peticion Sap Service Layer.");
            }

            return result;
        }
    }
}