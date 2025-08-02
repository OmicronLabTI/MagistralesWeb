// <summary>
// <copyright file="IOrderHistoryHelper.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.OrderHistory
{
    using System.Threading.Tasks;

    /// <summary>
    /// IOrderHistoryHelper.
    /// </summary>
    public interface IOrderHistoryHelper
    {
        /// <summary>
        /// SaveHistoryOrdersFab.
        /// </summary>
        /// <param name="detailOrderId">Child order number.</param>
        /// <param name="orderId">Parent order number.</param>
        /// <param name="userId">User who performed the division.</param>
        /// <param name="dxpOrder">DXP order number (can be null).</param>
        /// <param name="sapOrder">SAP order number (can be null).</param>
        /// <param name="assignedPieces">Pieces assigned in this division.</param>
        /// <param name="totalPieces">Total pieces of the parent order.</param>
        /// <returns>the return.</returns>
        Task SaveHistoryOrdersFab(
            int detailOrderId,
            int orderId,
            string userId,
            string dxpOrder,
            int? sapOrder,
            int assignedPieces,
            int totalPieces);
    }
}
