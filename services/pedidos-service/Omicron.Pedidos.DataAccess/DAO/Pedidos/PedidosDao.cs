// <summary>
// <copyright file="PedidosDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// dao for pedidos
    /// </summary>
    public class PedidosDao : IPedidosDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosDao"/> class.
        /// </summary>
        /// <param name="databaseContext">DataBase Context</param>
        public PedidosDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUserOrder(List<UserOrderModel> userorder)
        {
            this.databaseContext.UserOrderModel.AddRange(userorder);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="orderLog">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> InsertOrderLog(List<OrderLogModel> orderLog)
        {
            this.databaseContext.OrderLogModel.AddRange(orderLog);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// the list ids.
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderBySaleOrder(List<string> listIDs)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listIDs.Contains(x.Salesorderid)).ToListAsync();
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByUserId(List<string> listIds)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listIds.Contains(x.Userid)).ToListAsync();
        }
    }
}
