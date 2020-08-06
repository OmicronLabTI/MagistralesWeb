// <summary>
// <copyright file="SapDiApi.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Newtonsoft.Json;
using Omicron.Pedidos.Entities.Model;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Omicron.Pedidos.Services.SapDiApi
{
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
        /// Initializes a new instance of the <see cref="SapDiApi" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public SapDiApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="pedidos">the orders.</param>
        /// <returns>the return.</returns>
        public async Task<ResultModel> CreateFabOrder(object pedidos)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(pedidos), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + "createFabOrder";
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = JsonConvert.DeserializeObject<ResultModel>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
