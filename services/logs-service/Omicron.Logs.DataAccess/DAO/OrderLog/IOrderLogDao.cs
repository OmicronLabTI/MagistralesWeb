// <summary>
// <copyright file="IUserDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.DataAccess.DAO.OrderLog
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Logs.Entities.Model;

    /// <summary>
    /// Interface IOrderLogDao
    /// </summary>
    public interface  IOrderLogDao
    {
        /// <summary>
        /// Method for add user to DB.
        /// </summary>
        /// <param name="orderlog">OrderLog Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertOrderLog(OrderLogModel orderlog);
    }
}
