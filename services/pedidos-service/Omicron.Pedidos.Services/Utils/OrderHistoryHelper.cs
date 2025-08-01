// <summary>
// <copyright file="OrderHistoryHelper.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System;
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Serilog;

    /// <summary>
    /// Class for pedidos utils.
    /// </summary>
    public class OrderHistoryHelper
    {
        private readonly IOrderHistoryDao orderHistoryDao;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHistoryHelper"/> class.
        /// </summary>
        /// <param name="orderHistoryDao">Order History DAO.</param>
        /// <param name="logger">Logger.</param>
        public OrderHistoryHelper(IOrderHistoryDao orderHistoryDao, ILogger logger)
        {
            this.orderHistoryDao = orderHistoryDao ?? throw new ArgumentNullException(nameof(orderHistoryDao));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <summary>
        /// Combined method to register both the child order and update the parent order.
        /// </summary>
        /// <param name="detailOrderId">Child order number.</param>
        /// <param name="orderId">Parent order number.</param>
        /// <param name="userId">User who performed the division.</param>
        /// <param name="dxpOrder">DXP order number (can be null).</param>
        /// <param name="sapOrder">SAP order number (can be null).</param>
        /// <param name="assignedPieces">Pieces assigned in this division.</param>
        /// <param name="totalPieces">Total pieces of the parent order.</param>
        /// <returns>True if the registration was successful.</returns>
        public async Task SaveHistoryOrdersFab(
            int detailOrderId,
            int orderId,
            string userId,
            string dxpOrder,
            int? sapOrder,
            int assignedPieces,
            int totalPieces)
        {
            var logBase = string.Format(LogsConstants.SaveHistoryOrdersFabLogBase, detailOrderId, orderId);
            this.logger.Information(LogsConstants.SaveHistoryOrdersFabStart, logBase, assignedPieces, userId, sapOrder);
            try
            {
                var existingOrderDetail = await this.orderHistoryDao.GetDetailOrderById(detailOrderId);
                if (existingOrderDetail != null)
                {
                    this.logger.Warning(LogsConstants.SaveHistoryOrdersFabChildExists, logBase);
                    return;
                }

                var childResult = await this.RegisterSeparatedOrdersDetail(
                detailOrderId,
                orderId,
                userId,
                dxpOrder,
                sapOrder,
                assignedPieces);

                var parentResult = await this.UpsertOrderSeparation(orderId, totalPieces, assignedPieces);
            }
            catch (Exception ex)
            {
                var error = string.Format(LogsConstants.SaveHistoryOrdersFabEndWithError, logBase);
                this.logger.Error(ex, error);
                throw;
            }
        }

        /// <summary>
        /// Register the details of an order.
        /// </summary>
        /// <param name="detailOrderId"> detailOrderId.</param>
        /// <param name="orderId"> orderId.</param>
        /// <param name="userId"> userId.</param>
        /// <param name="dxpOrder">dxpOrder.</param>
        /// <param name="sapOrder">sapOrder.</param>
        /// <param name="assignedPieces">assignedPieces.</param>
        /// <returns>True register successfully.</returns>
        public async Task<bool> RegisterSeparatedOrdersDetail(
            int detailOrderId,
            int orderId,
            string userId,
            string dxpOrder,
            int? sapOrder,
            int assignedPieces)
        {
            var consecutiveIndex = await this.GetNextDivision(orderId);

            var insertDetailOrderId = new ProductionOrderSeparationDetailModel
            {
                DetailOrderId = detailOrderId,
                OrderId = orderId,
                UserId = userId,
                CreatedAt = DateTime.Now,
                DxpOrder = string.IsNullOrEmpty(dxpOrder) ? null : dxpOrder,
                SapOrder = sapOrder,
                AssignedPieces = assignedPieces,
                ConsecutiveIndex = consecutiveIndex,
            };

            return await this.orderHistoryDao.InsertDetailOrder(insertDetailOrderId);
        }

        /// <summary>
        /// Updates or inserts the history of a parent order.
        /// </summary>
        /// <param name="orderId">Parent orderId number.</param>
        /// <param name="totalPieces">Total pieces of the parent order.</param>
        /// <param name="assignedPieces">Pieces assigned in this division.</param>
        /// <returns>True if the update was successful.</returns>
        public async Task<bool> UpsertOrderSeparation(
            int orderId,
            int totalPieces,
            int assignedPieces)
        {
            var existingParent = await this.orderHistoryDao.GetParentOrderId(orderId);

            if (existingParent == null)
            {
                var availablePieces = totalPieces - assignedPieces;
                var isCompleteDivided = availablePieces == 0;

                var newParent = new ProductionOrderSeparationModel
                {
                    OrderId = orderId,
                    ProductionDetailCount = 1,
                    TotalPieces = totalPieces,
                    AvailablePieces = availablePieces,
                    Status = isCompleteDivided ? ServiceConstants.CompletelyDivided : ServiceConstants.PartiallyDivided,
                    CompletedAt = isCompleteDivided ? DateTime.Now : null,
                };

                return await this.orderHistoryDao.InsertOrder(newParent);
            }
            else
            {
                existingParent.ProductionDetailCount++;
                existingParent.AvailablePieces -= assignedPieces;

                var isCompletelyDivided = existingParent.AvailablePieces == 0;
                existingParent.Status = isCompletelyDivided ? ServiceConstants.CompletelyDivided : ServiceConstants.PartiallyDivided;

                if (isCompletelyDivided && existingParent.CompletedAt == null)
                {
                    existingParent.CompletedAt = DateTime.Now;
                }

                return await this.orderHistoryDao.UpdateOrder(existingParent);
            }
        }

        /// <summary>
        /// Gets the next division number for a parent order.
        /// </summary>
        /// <param name="orderId">Parent order number.</param>
        /// <returns>Next division number.</returns>
        private async Task<int> GetNextDivision(int orderId)
        {
            var maxDivisionNumber = await this.orderHistoryDao.GetMaxDivision(orderId);
            return maxDivisionNumber + 1;
        }
    }
}
