// <summary>
// <copyright file="PedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Pedidos
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Usuarios.Entities.Model;

    /// <summary>
    /// The class for pedido service.
    /// </summary>
    public class PedidosService : IPedidosService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public PedidosService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Makes a post to pedidos.
        /// </summary>
        /// <param name="dataToSend">the data to send.</param>
        /// <param name="route">the route.</param>
        /// <returns>the data retunred.</returns>
        public async Task<ResultModel> PostPedidos(object dataToSend, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(dataToSend), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
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
