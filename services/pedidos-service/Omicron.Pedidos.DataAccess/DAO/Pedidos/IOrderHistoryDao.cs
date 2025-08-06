// <summary>
// <copyright file="IOrderHistoryDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using System.Threading.Tasks;
    public interface IOrderHistoryDao
    {
        /// <summary>
        /// Insert order details
        /// </summary>
        /// <param name="detaildOrderId">order details</param>
        /// <returns>True was successfully inserted</returns>
        Task<bool> InsertDetailOrder(ProductionOrderSeparationDetailModel detaildOrderId);

        /// <summary>
        /// Insert order parent
        /// </summary>
        /// <param name="orderId">Parent order model</param>
        /// <returns>True was successfully inserted</returns>
        Task<bool> InsertOrder(ProductionOrderSeparationModel orderId);

        /// <summary>
        /// Updates an existing parent order
        /// </summary>
        /// <param name="orderId">Parent order model</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateOrder(ProductionOrderSeparationModel orderId);

        /// <summary>
        /// Gets a parent order by its number
        /// </summary>
        /// <param name="orderId"> parent order number</param>
        /// <returns>order model or null if it does not exist</returns>
        Task<ProductionOrderSeparationModel> GetParentOrderId(int orderId);

        /// <summary>
        /// Gets the maximum split number for a parent order
        /// </summary>
        /// <param name="orderId">Parent order number</param>
        /// <returns>Maximum division number</returns>
        Task<int> GetMaxDivision(int orderId);

        /// <summary>
        /// Gets the detailOrderId
        /// </summary>
        /// <param name="detailOrderId">detailOrderId</param>
        /// <returns>detailOrderId</returns>
        Task<ProductionOrderSeparationDetailModel> GetDetailOrderById(int detailOrderId);

        /// <summary>
        /// Gets fabOrderId with assigned pieces.
        /// </summary>
        /// <param name="fabOrderId">fabOrderId</param>
        /// <returns>fabOrderId order info with assigned pieces</returns>
        Task<OrderFabModel> GetChildOrderWithPieces(int fabOrderId);

        /// <summary>
        /// Updates available pieces for a parent order
        /// </summary>
        /// <param name="parentOrderId">Parent order ID</param>
        /// <param name="piecesToAdd">Pieces to add back to available</param>
        /// <returns>Success indicator</returns>
        Task<bool> UpdateAvailablePieces(int parentOrderId, int piecesToAdd);
    }
}
