// <summary>
// <copyright file="SapServiceLayerAdapterService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.ServiceLayerAdapter
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Resources.Exceptions;
    using Omicron.Reporting.Services.Utils;
    using Serilog;

    /// <summary>
    /// Class representing a generic service layer adapter service.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class SapServiceLayerAdapterService : ISapServiceLayerAdapterService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapServiceLayerAdapterService"/> class with the specified HttpClient instance.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to use for sending requests.</param>
        /// <param name="connection">The IServiceLayerAuth instance to use for get sessionId.</param>
        /// <param name="logger">The ILogger instance to logg information.</param>
        public SapServiceLayerAdapterService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> PostAsync(string url, string requestBody)
        {
            ResultModel result;
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            using (var response = await this.httpClient.PostAsync(url, content))
            {
                result = await CommonCall.GetResponse(response, this.logger, "Reporting - PostAsync - Error peticion Sap Service Layer.");
            }

            if (result.Code >= 300)
            {
                this.logger.Error($"Reporting - Post Async - Error petición Service Layer Adapter {JsonConvert.SerializeObject(result)}");
                throw new CustomServiceException(result.UserError.ToString(), HttpStatusCode.NotFound);
            }

            return result;
        }
    }
}
