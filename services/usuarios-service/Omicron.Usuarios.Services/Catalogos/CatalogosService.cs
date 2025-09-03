// <summary>
// <copyright file="CatalogosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Catalogos
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Omicron.Usuarios.Dtos.Models;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Resources.Exceptions;
    using Omicron.Usuarios.Services.Utils;

    /// <summary>
    /// the catalogs service.
    /// </summary>
    public class CatalogosService : ICatalogosService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogosService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public CatalogosService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetCatalogos(string route)
        {
            ResultModel result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode >= 300)
                {
                    throw new CustomServiceException(jsonString, System.Net.HttpStatusCode.BadRequest);
                }

                result = JsonConvert.DeserializeObject<ResultModel>(jsonString);
            }

            return result;
        }
    }
}
