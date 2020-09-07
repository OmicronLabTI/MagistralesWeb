// <summary>
// <copyright file="IUserFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Facade.Catalogs.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Usuarios.Dtos.Models;
    using Omicron.Usuarios.Dtos.User;

    /// <summary>
    /// Interface User Facade.
    /// </summary>
    public interface IUserFacade
    {
        /// <summary>
        /// Method for get all list of Users.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<UserDto>> GetListUsersActive();

        /// <summary>
        /// Method for get User by id.
        /// </summary>
        /// <param name="id">Id User.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserDto> GetListUserActive(int id);

        /// <summary>
        /// Method to add user to DB.
        /// </summary>
        /// <param name="user">User Model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertUser(UserDto user);

        /// <summary>
        /// The create user method.
        /// </summary>
        /// <param name="userDto">The user Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> CreateUser(UserDto userDto);

        /// <summary>
        /// Gets all the user with offset and limit.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetUsers(Dictionary<string, string> parameters);

        /// <summary>
        /// deletes the user.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> DeleteUser(List<string> listIds);

        /// <summary>
        /// updates the user.
        /// </summary>
        /// <param name="user">the user to update.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> UpdateUser(UserDto user);

        /// <summary>
        /// Get the user.
        /// </summary>
        /// <param name="userName">the username.</param>
        /// <returns>the user.</returns>
        Task<ResultDto> GetUser(string userName);

        /// <summary>
        /// gets the qfb.
        /// </summary>
        /// <param name="roleId">the roleid.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetUsersByRole(string roleId);

        /// <summary>
        /// the list ids.
        /// </summary>
        /// <param name="listIds">the users.</param>
        /// <returns>the users.</returns>
        Task<ResultDto> GetUsersById(List<string> listIds);

        /// <summary>
        /// Gets the QFB with the total count of orders.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetQfbWithOrderCount();
    }
}