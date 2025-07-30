// <summary>
// <copyright file="OrderHistoryDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Microsoft.EntityFrameworkCore;
using Omicron.Pedidos.Entities.Context;
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

        public async Task<bool> InsertDetailOrder(List<ProductionOrderSeparationDetailModel> detaildOrder)
        {
            this.databaseContext.ProductionOrderSeparationDetailModel.AddRange(detaildOrder);
            await((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        public Task<bool> InsertOrder(ProductionOrderSeparationModel order)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateOrder(ProductionOrderSeparationModel order)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ProductionOrderSeparationDetailModel>> GetDetailOrderByParentOrder(List<string> detailOrderNumbers)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> GetMaxDivisionNumber(int orderNumber)
        {
            return await this.databaseContext.ProductionOrderSeparationDetailModel
               .Where(c => c.OrderId == orderNumber)
               .MaxAsync(c => (int?)c.ConsecutiveIndex) ?? 0;
        }

        public Task<ProductionOrderSeparationModel> GetParentOrderByOrderNumber(string orderNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ProductionOrderSeparationModel>> GetParentOrderByOrderNumbers(List<string> orderNumbers)
        {
            throw new System.NotImplementedException();
        }       
    }
}
