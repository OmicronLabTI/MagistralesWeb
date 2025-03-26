// <summary>
// <copyright file="CatalogsDxpService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.CatalogsDxp
{
    /// <summary>
    /// Class catalogs dxp.
    /// </summary>
    public class CatalogsDxpService : ICatalogsDxpService
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
        /// Initializes a new instance of the <see cref="CatalogsDxpService" /> class.
        /// </summary>
        /// <param name="httpClient"> The HttpClient for SAP communication. </param>
        /// <param name="logger"> The logger for logging operations. </param>
        public CatalogsDxpService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> Post(object data, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(data), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ServiceUtils.GetResponse(response, this.logger, "Error peticion catalogs dxp service");
            }

            return result;
        }
    }
}
