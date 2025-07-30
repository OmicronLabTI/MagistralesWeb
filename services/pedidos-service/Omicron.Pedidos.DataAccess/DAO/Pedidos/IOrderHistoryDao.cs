// <summary>
// <copyright file="IOrderHistoryDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    using Omicron.Pedidos.Entities.Model.Db;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IOrderHistoryDao
    {
        /// <summary>
        /// Insert order details
        /// </summary>
        /// <param name="detaildOrder">order details</param>
        /// <returns>True was successfully inserted</returns>
        Task<bool> InsertDetailOrder(List<ProductionOrderSeparationDetailModel> detaildOrder);

        /// <summary>
        /// Insert order parent
        /// </summary>
        /// <param name="order">Parent order model</param>
        /// <returns>True was successfully inserted</returns>
        Task<bool> InsertOrder(ProductionOrderSeparationModel order);

        /// <summary>
        /// Updates an existing parent order
        /// </summary>
        /// <param name="order">Parent order model</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateOrder(ProductionOrderSeparationModel order);

        /// <summary>
        /// Gets a parent order by its number
        /// </summary>
        /// <param name="orderNumber"> parent order number</param>
        /// <returns>order model or null if it does not exist</returns>
        Task<ProductionOrderSeparationModel> GetParentOrderByOrderNumber(string orderNumber);

        /// <summary>
        /// Gets the maximum split number for a parent order
        /// </summary>
        /// <param name="orderNumber">Parent order number</param>
        /// <returns>Maximum division number</returns>
        Task<int> GetMaxDivisionNumber(int orderNumber);

        /// <summary>
        /// Gets the order details by parent order
        /// </summary>
        /// <param name="detailOrderNumbers">List of parent order numbers</param>
        /// <returns>list of order details</returns>
        Task<IEnumerable<ProductionOrderSeparationDetailModel>> GetDetailOrderByParentOrder(List<string> detailOrderNumbers);

        /// <summary>
        /// Gets the history of parent orders by numbers
        /// </summary>
        /// <param name="orderNumbers">List of parent order numbers</param>
        /// <returns>List of parent orders</returns>
        Task<IEnumerable<ProductionOrderSeparationModel>> GetParentOrderByOrderNumbers(List<string> orderNumbers);
    }
}
