// <summary>
// <copyright file="SapAdapter.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.SapAdapter
{
    /// <summary>
    /// the sap adapter.
    /// </summary>
    public class SapAdapter : ISapAdapter
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAdapter" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public SapAdapter(HttpClient httpClient, Serilog.ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">the route.</param>
        /// <returns>the return.</returns>
        public async Task<ResultDto> PostSapAdapter(object dataToSend, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Error peticion sapadapter");
            }

            return result;
        }
    }
}
