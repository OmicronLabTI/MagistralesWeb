// <summary>
// <copyright file="IUsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.User
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// Interface User Service.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Method for get all users from db.
        /// </summary>
        /// <param name="route">the list ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> SimpleGetUsers(string route);

        /// <summary>
        /// gets the data.
        /// </summary>
        /// <param name="data">the data.</param>
        /// <param name="route">the route.</param>
        /// <returns>the returns.</returns>
        Task<ResultModel> PostSimpleUsers(object data, string route);
    }
}
