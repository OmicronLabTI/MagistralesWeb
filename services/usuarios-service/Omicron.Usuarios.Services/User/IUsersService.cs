// <summary>
// <copyright file="IUsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;

    /// <summary>
    /// Interface User Service.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Method for get all users from db.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Method for get user by id from db.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserDto> GetUserAsync(int userId);

        /// <summary>
        /// Method for add user to DB.
        /// </summary>
        /// <param name="user">User Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertUser(UserDto user);

        /// <summary>
        /// Method for validating the login.
        /// </summary>
        /// <param name="login">the login object.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> ValidateCredentials(LoginModel login);

        /// <summary>
        /// Method to create a user.
        /// </summary>
        /// <param name="userModel">the user model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> CreateUser(UserModel userModel);

        /// <summary>
        /// the method to return data.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetUsers(Dictionary<string, string> parameters);
    }
}
