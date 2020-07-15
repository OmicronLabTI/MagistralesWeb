// <summary>
// <copyright file="UserDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.DataAccess.DAO.User
{
    using Omicron.Catalogos.Entities.Context;
    using Omicron.Catalogos.Entities.Model;
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
            return await this.databaseContext.CatUser.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<UserModel> GetUserAsync(int userId)
        {
            return await this.databaseContext.CatUser.FirstOrDefaultAsync(p => p.Id.Equals(userId));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUser(UserModel user)
        {
            var response = await this.databaseContext.CatUser.AddAsync(user);
            bool result = response.State.Equals(EntityState.Added);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return result;
        }
    }
}
