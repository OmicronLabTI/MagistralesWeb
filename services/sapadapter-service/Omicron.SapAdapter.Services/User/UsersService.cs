// <summary>
// <copyright file="UsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.User
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.Models;

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
        /// Initializes a new instance of the <see cref="UsersService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public UsersService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Method for get all users from db.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> GetUsersById(List<string> listIds)
        {
            ResultDto result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(listIds), UnicodeEncoding.UTF8, "application/json");

            var url = this.httpClient.BaseAddress + "getUsersById";
            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = JsonConvert.DeserializeObject<ResultDto>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }
    }
}
