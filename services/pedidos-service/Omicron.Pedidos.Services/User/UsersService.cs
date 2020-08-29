// <summary>
// <copyright file="UsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.User
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// Class User Service.
    /// </summary>
    public class UsersService : IUsersService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="httpClient">Object to mapper.</param>
        public UsersService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> SimpleGetUsers(string route)
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

        /// <summary>
        /// gets the data.
        /// </summary>
        /// <param name="data">the data.</param>
        /// <param name="route">the route.</param>
        /// <returns>the returns.</returns>
        public async Task<ResultModel> PostSimpleUsers(object data, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(data), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
