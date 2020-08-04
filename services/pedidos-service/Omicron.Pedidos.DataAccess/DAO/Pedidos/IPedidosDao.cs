// <summary>
// <copyright file="IPedidosDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    using Omicron.Pedidos.Entities.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPedidosDao
    {
        /// <summary>
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="userorder">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertUserOrder(List<UserOrderModel> userorder);

        /// <summary>
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="orderLog">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertOrderLog(List<OrderLogModel> orderLog);
    }
}
