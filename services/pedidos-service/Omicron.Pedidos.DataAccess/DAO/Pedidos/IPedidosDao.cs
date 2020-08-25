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

        /// <summary>
        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderBySaleOrder(List<string> listIDs);

        /// <summary>
        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByProducionOrder(List<string> listIDs);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByUserId(List<string> listIds);

        /// <summary>
        /// Updates the entries.
        /// </summary>
        /// <param name="userOrderModels">the user model.</param>
        /// <returns>the data.</returns>
        Task<bool> UpdateUserOrders(List<UserOrderModel> userOrderModels);

        /// <summary>
        /// Method for add order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to add.</param>
        /// <returns>Operation result</returns>
        Task<bool> InsertOrderSignatures(UserOrderSignatureModel orderSignature);

        /// <summary>
        /// Method for save order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to save.</param>
        /// <returns>Operation result</returns>
        Task<bool> SaveOrderSignatures(UserOrderSignatureModel orderSignature);
         
        /// <summary>
        /// Get order signature by user order id.
        /// </summary>
        /// <param name="userOrderId">User order to find.</param>
        /// <returns>Operation result</returns>
        Task<UserOrderSignatureModel> GetSignaturesByUserOrderId(int userOrderId);
    }
}
