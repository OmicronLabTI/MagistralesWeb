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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Class for pedidos utils.
    /// </summary>
    public class OrderHistoryHelper
    {
        private readonly IOrderHistoryDao orderHistoryDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHistoryHelper"/> class.
        /// </summary>
        /// <param name="orderHistoryDao">Order History DAO.</param>
        public OrderHistoryHelper(IOrderHistoryDao orderHistoryDao)
        {
            this.orderHistoryDao = orderHistoryDao ?? throw new ArgumentNullException(nameof(orderHistoryDao));
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
        public async Task<bool> SaveChildOrderHistoryAsync(
            int detailOrderId,
            int orderId,
            string userId,
            string dxpOrder,
            int sapOrder,
            int assignedPieces)
        {
            // Obtener el siguiente número de división
            var consecutiveIndex = await this.GetNextDivisionNumberAsync(orderId);

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

            try
            {
                return await this.orderHistoryDao.InsertDetailOrder(
                    new List<ProductionOrderSeparationDetailModel> { insertDetailOrderId });
            }
            catch (Exception ex)
            {
                // Muestra el mensaje de la excepción interna
                var inner = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                throw new Exception($"Error al insertar detalle: {ex.Message} - Inner: {inner}", ex);
            }
        }

        /// <summary>
        /// Obtiene el siguiente número de división para una orden padre.
        /// </summary>
        /// <param name="orderId">Número de orden padre.</param>
        /// <returns>Siguiente número de división.</returns>
        private async Task<int> GetNextDivisionNumberAsync(int orderId)
        {
            var maxDivisionNumber = await this.orderHistoryDao.GetMaxDivisionNumber(orderId);
            return maxDivisionNumber + 1;
        }
    }
}
