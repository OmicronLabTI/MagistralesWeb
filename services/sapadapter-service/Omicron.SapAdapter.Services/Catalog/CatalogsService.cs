// <summary>
// <copyright file="CatalogsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Catalog
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

    /// <summary>
    /// class for the catalog service.
    /// </summary>
    public class CatalogsService : ICatalogsService
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
        /// Initializes a new instance of the <see cref="CatalogsService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public CatalogsService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetParams(string route)
        {
            ResultDto result;
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.GetAsync(url))
            {
                result = await ServiceUtils.GetResponse(response, this.logger, "Error peticion catalogs");
            }

            return result;
        }
    }
}
