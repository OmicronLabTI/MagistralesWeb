// <summary>
// <copyright file="UsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Class User Service.
    /// </summary>
    public class UsersService : BaseClientService, IUsersService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public UsersService(HttpClient httpClient)
            : base(httpClient)
        {
            httpClient.BaseAddress = new Uri("http://localhost:5101/");
        }

        /// <summary>
        /// Method for get users by id.
        /// </summary>
        /// <param name="userIds">User ids's.</param>
        /// <returns>User list.</returns>
        public async Task<List<UserModel>> GetUsersById(params string[] userIds)
        {
            var resultModel = await this.PostAsync(userIds, "getUsersById");
            return JsonConvert.DeserializeObject<List<UserModel>>(resultModel.Response.ToString());
        }
    }
}
