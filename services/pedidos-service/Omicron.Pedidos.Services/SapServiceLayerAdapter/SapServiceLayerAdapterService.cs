// <summary>
// <copyright file="SapServiceLayerAdapterService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.SapServiceLayerAdapter
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// the sap adapter.
    /// </summary>
    public class SapServiceLayerAdapterService : ISapServiceLayerAdapterService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapServiceLayerAdapterService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public SapServiceLayerAdapterService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PatchAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpRequestMessage request = new
                (new HttpMethod("PATCH"), url)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
            };

            using (var response = await this.httpClient.SendAsync(request))
            {
                result = await ServiceShared.GetResponse(response, this.logger, "Pedidos Service - PatchAsync - Error peticion Sap Service Layer.");
            }

            return result;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">the route.</param>
        /// <param name="logError">Log Error.</param>
        /// <returns>the return.</returns>
        public async Task<ResultModel> PostAsync(object dataToSend, string route, string logError = null)
        {
            logError = logError ?? "Pedidos Service - POST - Error peticion service layer";
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ServiceShared.GetResponse(response, this.logger, logError);
            }

            return result;
        }
    }
}
