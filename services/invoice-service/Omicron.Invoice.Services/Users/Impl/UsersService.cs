// <summary>
// <copyright file="UsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Users.Impl
{
    using System.Text;
    using Newtonsoft.Json;
    using Omicron.Invoice.Services.Utils;
    using Serilog;

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
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        /// <param name="logger">the logger.</param>
        public UsersService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Method for get all users from db.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <param name="route">the route.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> GetUsersById(List<string> listIds, string route)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(listIds), UnicodeEncoding.UTF8, "application/json");

            var url = this.httpClient.BaseAddress + route;
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ResponseUtils.GetResponse(response, this.logger, "Error peticion usuarios");
            }

            return result;
        }
    }
}