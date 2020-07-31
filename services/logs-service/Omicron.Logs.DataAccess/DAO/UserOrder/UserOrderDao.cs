// <summary>
// <copyright file="UserDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.DataAccess.DAO.UserOrder
{
    using Omicron.Logs.Entities.Context;
    using Omicron.Logs.Entities.Model;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Class UserOrder Dao
    /// </summary>
    public class UserOrderDao : IUserOrderDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserOrderDao"/> class.
        /// </summary>
        /// <param name="databaseContext">DataBase Context</param>
        public UserOrderDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUserOrder(UserOrderModel userorder)
        {
            var response = await this.databaseContext.UserOrder.AddAsync(userorder);
            bool result = response.State.Equals(EntityState.Added);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return result;
        }
    }
}
