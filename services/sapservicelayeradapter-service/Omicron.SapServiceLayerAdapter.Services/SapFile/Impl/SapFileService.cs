// <summary>
// <copyright file="SapFileService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.SapFile.Impl
{
    /// <summary>
    /// Class Sap File Service.
    /// </summary>
    public class SapFileService : ISapFileService
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
        /// Initializes a new instance of the <see cref="SapFileService"/> class.
        /// </summary>
        /// <param name="httpClient">Http Client.</param>
        /// <param name="logger">Logger.</param>
        public SapFileService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PostAsync(object data, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(data), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ServiceUtils.GetResponse(response, this.logger, "Sap File Service", "Error en la petición hacia Sap Files Service.");
            }

            return result;
        }
    }
}
