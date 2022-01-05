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
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

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
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidoService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public PedidoService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the QFB with pedidos.
        /// </summary>
        /// <param name="listPedidos">Pedidos id.</param>
        /// <param name="route">the route.</param>
        /// <returns>Result object.</returns>
        public async Task<ResultDto> PostPedidos(object listPedidos, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(listPedidos), UnicodeEncoding.UTF8, "application/json");

            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ServiceUtils.GetResponse(response, this.logger, "Error peticion pedidos");
            }

            return result;
        }

        /// <summary>
        /// Makes a get to sapAdapter.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetUserPedidos(string route)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await ServiceUtils.GetResponse(response, this.logger, "Error peticion pedidos");
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
                result = await ServiceUtils.GetResponse(response, this.logger, "Error peticion pedidos");
            }

            return result;
        }
    }
}
