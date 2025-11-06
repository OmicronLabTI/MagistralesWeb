// <summary>
// <copyright file="CatalogsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Catalog
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Almacen.Services.Utils;
    using Serilog;

    /// <summary>
    /// Class for catalogs.
    /// </summary>
    public class CatalogsService : ICatalogsService
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
        /// Initializes a new instance of the <see cref="CatalogsService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public CatalogsService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetParams(string route)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Error peticion catalogos");
            }

            return result;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">the route.</param>
        /// <returns>the return.</returns>
        public async Task<ResultDto> PostCatalogs(object dataToSend, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Error peticion catalogos");
            }

            return result;
        }
    }
}
