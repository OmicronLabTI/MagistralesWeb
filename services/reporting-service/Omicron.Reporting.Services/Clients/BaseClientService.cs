// <summary>
// <copyright file="BaseClientService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Clients
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Base class of client services.
    /// </summary>
    public abstract class BaseClientService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClientService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        protected BaseClientService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Invoke to POST endpoint.
        /// </summary>
        /// <param name="data">data to submit.</param>
        /// <param name="route">the route to send.</param>
        /// <returns>result model.</returns>
        public async Task<ResultModel> PostAsync(object data, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(data), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode > 200)
                {
                    throw new CustomServiceException(jsonString);
                }

                result = JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }

        /// <summary>
        /// Invoke to GET endpoint.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>result model.</returns>
        public async Task<ResultModel> GetAsync(string route)
        {
            ResultModel result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode >= 300)
                {
                    throw new CustomServiceException(jsonString);
                }

                result = JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
