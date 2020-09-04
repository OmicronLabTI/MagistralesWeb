// <summary>
// <copyright file="IUsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// Interface User Service.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Method for get all users from db.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// /// <param name="route">the route.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetUsersById(List<string> listIds, string route);
    }
}
