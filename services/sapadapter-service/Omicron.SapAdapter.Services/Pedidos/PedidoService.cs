// <summary>
// <copyright file="PedidoService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Pedidos
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// clase de pedido Service.
    /// </summary>
    public class PedidoService : IPedidosService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidoService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public PedidoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets the QFB with pedidos.
        /// </summary>
        /// <param name="listPedidos">Pedidos id.</param>
        /// <param name="route">the route.</param>
        /// <returns>Result object.</returns>
        public async Task<ResultDto> GetUserPedidos(List<int> listPedidos, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(listPedidos), UnicodeEncoding.UTF8, "application/json");

            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = JsonConvert.DeserializeObject<ResultDto>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }

        /// <summary>
        /// Makes a get to pedidos service.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetPedidosService(string route)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode >= 300)
                {
                    throw new System.Exception();
                }

                result = JsonConvert.DeserializeObject<ResultDto>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
