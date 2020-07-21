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
    }
}