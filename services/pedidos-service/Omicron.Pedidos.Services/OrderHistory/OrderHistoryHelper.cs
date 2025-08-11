// <summary>
// <copyright file="OrderHistoryHelper.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.OrderHistory
{
    using System;
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.MediatR.Commands;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// Class for pedidos utils.
    /// </summary>
    public class OrderHistoryHelper : IOrderHistoryHelper
    {
        private readonly IPedidosDao pedidosDao;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHistoryHelper"/> class.
        /// </summary>
        /// <param name="pedidosDao">Order History DAO.</param>
        /// <param name="logger">Logger.</param>
        public OrderHistoryHelper(IPedidosDao pedidosDao, ILogger logger)
        {
            this.pedidosDao = pedidosDao ?? throw new ArgumentNullException(nameof(pedidosDao));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <summary>
        /// Combined method to register both the child order and update the parent order.
        /// </summary>
        /// <param name="detailOrderId">Child order number.</param>
        /// <param name="request">request.</param>
        /// <returns>True if the registration was successful.</returns>
        public async Task SaveHistoryOrdersFab(int detailOrderId, SeparateProductionOrderCommand request)
        {
            var logBase = string.Format(LogsConstants.SaveHistoryOrdersFabLogBase, detailOrderId, request.ProductionOrderId);
            this.logger.Information(LogsConstants.SaveHistoryOrdersFabStart, logBase, request.Pieces, request.UserId, request.SapOrder);
            try
            {
                var existingOrderDetail = await this.pedidosDao.GetDetailOrderById(detailOrderId);
                if (existingOrderDetail != null)
                {
                    this.logger.Warning(LogsConstants.SaveHistoryOrdersFabChildExists, logBase);
                    return;
                }

                var childResult = await this.RegisterSeparatedOrdersDetail(detailOrderId, request);

                var parentResult = await this.UpsertOrderSeparation(request.ProductionOrderId, request.TotalPieces, request.Pieces);
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
        /// <param name="request"> request.</param>
        /// <returns>True register successfully.</returns>
        public async Task<bool> RegisterSeparatedOrdersDetail(int detailOrderId, SeparateProductionOrderCommand request)
        {
            var consecutiveIndex = await this.pedidosDao.GetMaxDivision(request.ProductionOrderId);

            var insertDetailOrderId = new ProductionOrderSeparationDetailModel
            {
                DetailOrderId = detailOrderId,
                OrderId = request.ProductionOrderId,
                UserId = request.UserId,
                CreatedAt = DateTime.Now,
                DxpOrder = string.IsNullOrEmpty(request.DxpOrder) ? null : request.DxpOrder,
                SapOrder = request.SapOrder,
                AssignedPieces = request.Pieces,
                ConsecutiveIndex = consecutiveIndex,
            };

            return await this.pedidosDao.InsertDetailOrder(insertDetailOrderId);
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
            var existingParent = await this.pedidosDao.GetParentOrderId(orderId);

            if (existingParent == null)
            {
                var availablePieces = totalPieces - assignedPieces;
                var isCompleteDivided = availablePieces == 0;

                var newParent = new ProductionOrderSeparationModel
                {
                    OrderId = orderId,
                    ProductionDetailCount = ServiceConstants.ProductionDetailCount,
                    TotalPieces = totalPieces,
                    AvailablePieces = availablePieces,
                    Status = isCompleteDivided ? ServiceConstants.CompletelyDivided : ServiceConstants.PartiallyDivided,
                    CompletedAt = isCompleteDivided ? DateTime.Now : null,
                };

                return await this.pedidosDao.InsertOrder(newParent);
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

                return await this.pedidosDao.UpdateOrder(existingParent);
            }
        }
    }
}
