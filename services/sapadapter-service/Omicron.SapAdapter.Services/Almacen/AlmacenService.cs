// <summary>
// <copyright file="AlmacenService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Almacen
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// the almacen service.
    /// </summary>
    public class AlmacenService : IAlmacenService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlmacenService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public AlmacenService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetAlmacenOrders(string route)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode >= 300)
                {
                    throw new CustomServiceException(jsonString);
                }

                result = JsonConvert.DeserializeObject<ResultDto>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> PostAlmacenOrders(string route, object dataToSend)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");

            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = JsonConvert.DeserializeObject<ResultDto>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
