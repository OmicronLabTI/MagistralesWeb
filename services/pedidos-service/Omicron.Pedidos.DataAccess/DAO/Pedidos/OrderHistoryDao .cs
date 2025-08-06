// <summary>
// <copyright file="OrderHistoryDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Microsoft.EntityFrameworkCore;
using Omicron.Pedidos.Entities.Context;
using Omicron.Pedidos.Entities.Model;
using Omicron.Pedidos.Entities.Model.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    /// <summary>
    /// the OrderHistory service.
    /// </summary>
    public class OrderHistoryDao : IOrderHistoryDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHistoryDao"/> class.
        /// </summary>
        /// <param name="databaseContext">DataBase Context</param>
        public OrderHistoryDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertDetailOrder(ProductionOrderSeparationDetailModel detaildOrder)
        {
            this.databaseContext.ProductionOrderSeparationDetailModel.Add(detaildOrder);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> InsertOrder(ProductionOrderSeparationModel orderId)
        {
            this.databaseContext.ProductionOrderSeparationModel.Add(orderId);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateOrder(ProductionOrderSeparationModel orderId)
        {
            this.databaseContext.ProductionOrderSeparationModel.Update(orderId);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<int> GetMaxDivision(int orderId)
        {
            return await this.databaseContext.ProductionOrderSeparationDetailModel
               .Where(c => c.OrderId == orderId)
               .MaxAsync(c => (int?)c.ConsecutiveIndex) ?? 0;
        }

        /// <inheritdoc/>
        public async Task<ProductionOrderSeparationModel> GetParentOrderId(int orderId)
        {
            return await this.databaseContext.ProductionOrderSeparationModel
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        /// <inheritdoc/>
        public async Task<ProductionOrderSeparationDetailModel> GetDetailOrderById(int detailOrderId)
        {
            return await this.databaseContext.ProductionOrderSeparationDetailModel
                .FirstOrDefaultAsync(x => x.DetailOrderId == detailOrderId);
        }

        /// <inheritdoc/>
        public async Task<OrderFabModel> GetChildOrderWithPieces(int childOrderId)
        {
            var childOrderInfo = await (from parent in this.databaseContext.ProductionOrderSeparationModel
                                        join detail in this.databaseContext.ProductionOrderSeparationDetailModel
                                        on parent.OrderId equals detail.OrderId
                                        where detail.DetailOrderId == childOrderId
                                        select new OrderFabModel
                                        {
                                            ParentId = parent.Id,
                                            AvailablePieces = parent.AvailablePieces,
                                            AssignedPieces = detail.AssignedPieces
                                        }).FirstOrDefaultAsync();

            return childOrderInfo;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAvailablePieces(int parentOrderId, int piecesToAdd)
        {
            var parentOrder = await this.databaseContext.ProductionOrderSeparationModel
                .FirstOrDefaultAsync(x => x.OrderId == parentOrderId);

            if (parentOrder != null)
            {
                parentOrder.AvailablePieces += piecesToAdd;
                await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
                return true;
            }

            return false;
        }

    }
}
