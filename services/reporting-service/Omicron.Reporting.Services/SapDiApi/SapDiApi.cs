// <summary>
// <copyright file="SapDiApi.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.SapDiApi
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using DocumentFormat.OpenXml.Office2010.CustomUI;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Utils;
    using Serilog;

    /// <summary>
    /// the sap adapter.
    /// </summary>
    public class SapDiApi : ISapDiApi
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
        /// Initializes a new instance of the <see cref="SapDiApi" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public SapDiApi(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.httpClient.Timeout = new System.TimeSpan(0, 30, 0);
            this.logger = logger;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the orders.</param>
        /// <param name="route">the route to send.</param>
        /// <returns>the return.</returns>
        public async Task<ResultModel> PostToSapDiApi(object dataToSend, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await CommonCall.GetResponse(response, this.logger, "Error peticion sapdiapi service");
            }

            return result;
        }
    }
}
