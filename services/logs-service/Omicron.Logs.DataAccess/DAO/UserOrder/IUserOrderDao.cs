// <summary>
// <copyright file="IUserDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.DataAccess.DAO.UserOrder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Logs.Entities.Model;

    /// <summary>
    /// Interface IUserOrderDao
    /// </summary>
    public interface  IUserOrderDao
    {
        /// <summary>
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="userorder">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertUserOrder(UserOrderModel userorder);
    }
}
