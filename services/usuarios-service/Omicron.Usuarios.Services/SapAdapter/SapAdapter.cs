// <summary>
// <copyright file="SapAdapter.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.SapAdapter
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Usuarios.Entities.Model;

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
        /// Initializes a new instance of the <see cref="SapAdapter" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public SapAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="listPedidos">the orders.</param>
        /// <param name="route">the route.</param>
        /// <returns>the return.</returns>
        public async Task<ResultModel> PostSapAdapter(object listPedidos, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(listPedidos), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode >= 300)
                {
                    throw new CustomServiceException(jsonString, System.Net.HttpStatusCode.BadRequest);
                }

                result = JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
