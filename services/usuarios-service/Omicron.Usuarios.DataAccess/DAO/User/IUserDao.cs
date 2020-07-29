// <summary>
// <copyright file="IUserDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.DataAccess.DAO.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Usuarios.Entities.Model;

    /// <summary>
    /// Interface IUserDao
    /// </summary>
    public interface  IUserDao
    {
        /// <summary>
        /// Method for get all users from db.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<UserModel>> GetAllUsersAsync();

        /// <summary>
        /// Method for get user by id from db.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserModel> GetUserAsync(int userId);

        /// <summary>
        /// Method for add user to DB.
        /// </summary>
        /// <param name="user">User Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertUser(UserModel user);

        /// <summary>
        /// Method for looking the user by user name.
        /// </summary>
        /// <param name="userName">the user name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserModel> GetUserByUserName(string userName);

        /// <summary>
        /// Updates the uses.
        /// </summary>
        /// <param name="listUsers">the list of users.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdateUsers(List<UserModel> listUsers);

        /// <summary>
        /// Method for get user by id from db.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserModel> GetUserById(string userId);

        /// <summary>
        /// Updates a single user.
        /// </summary>
        /// <param name="user">the user to update.</param>
        /// <returns>the user.</returns>
        Task<bool> UpdateUser(UserModel user);
    }
}
