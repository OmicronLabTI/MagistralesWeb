// <summary>
// <copyright file="IUsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Interface User Service.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Method for get users by id.
        /// </summary>
        /// <param name="userIds">User ids's.</param>
        /// <returns>User list.</returns>
        Task<List<UserModel>> GetUsersById(params string[] userIds);
    }
}
