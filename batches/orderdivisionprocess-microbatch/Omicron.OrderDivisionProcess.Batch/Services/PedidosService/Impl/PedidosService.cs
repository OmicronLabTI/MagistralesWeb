// <summary>
// <copyright file="PedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Services.PedidosService.Impl
{
    /// <summary>
    /// PedidosService.
    /// </summary>
    public class PedidosService : IPedidosService
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
        /// Initializes a new instance of the <see cref="PedidosService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">The logger.</param>
        public PedidosService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetAsync(string route, string logBase)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await CommonFunctions.GetResponse(
                    response,
                    this.logger,
                    string.Format(BatchConstants.ErrorCallingGetPedidosService, logBase));
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> PostAsync(string route, object dataToSend, string logBase)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, BatchConstants.StringContentMediaType);
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await CommonFunctions.GetResponse(
                    response,
                    this.logger,
                    string.Format(BatchConstants.ErrorCallingPostPedidosService, logBase));
            }

            return result;
        }
    }
}
