// <summary>
// <copyright file="UserDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.DataAccess.DAO.User
{
    using Omicron.Usuarios.Entities.Context;
    using Omicron.Usuarios.Entities.Model;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Class User Dao
    /// </summary>
    public class UserDao : IUserDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDao"/> class.
        /// </summary>
        /// <param name="databaseContext">DataBase Context</param>
        public UserDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await this.databaseContext.Usuarios.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<UserModel> GetUserAsync(int userId)
        {
            return await this.databaseContext.Usuarios.FirstOrDefaultAsync(p => p.Id.Equals(userId));
        }

        /// <summary>
        /// Looks for the user by the user name.
        /// </summary>
        /// <param name="userName">the user name.</param>
        /// <returns>the user.</returns>
        public async Task<UserModel> GetUserByUserName(string userName)
        {
            return await this.databaseContext.Usuarios.FirstOrDefaultAsync(p => p.UserName.Equals(userName));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUser(UserModel user)
        {
            var response = await this.databaseContext.Usuarios.AddAsync(user);
            bool result = response.State.Equals(EntityState.Added);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return result;
        }

        /// <summary>
        /// Updates the uses.
        /// </summary>
        /// <param name="listUsers">the list of users.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> UpdateUsers(List<UserModel> listUsers)
        {
            this.databaseContext.Usuarios.UpdateRange(listUsers);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for get user by id from db.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<UserModel> GetUserById(string userId)
        {
            return await this.databaseContext.Usuarios.FirstOrDefaultAsync(p => p.Id.Equals(userId));
        }
    }
}
