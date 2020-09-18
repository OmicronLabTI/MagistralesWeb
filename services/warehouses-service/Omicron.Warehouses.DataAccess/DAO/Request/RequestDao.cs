// <summary>
// <copyright file="PedidosDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.DataAccess.DAO.Request
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Omicron.Warehouses.Entities.Context;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// dao for pedidos
    /// </summary>
    public class RequestDao : IRequestDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDao"/> class.
        /// </summary>
        /// <param name="databaseContext">DataBase Context</param>
        public RequestDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Method for add new raw material request
        /// </summary>
        /// <param name="request">Request to add.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> InsertRawMaterialRequest(RawMaterialRequestModel request)
        {
            await this.databaseContext.RawMaterialRequests.AddAsync(request);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for add detail of raw material request.
        /// </summary>
        /// <param name="detail">Raw material request detail to add.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> InsertDetailsOfRawMaterialRequest(List<RawMaterialRequestDetailModel> detail)
        {
            await this.databaseContext.RawMaterialRequestDetails.AddRangeAsync(detail);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for add production orders related to  raw material request.
        /// </summary>
        /// <param name="detail">Production order ids to add.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> InsertOrdersOfRawMaterialRequest(List<RawMaterialRequestOrderModel> orders)
        {
            await this.databaseContext.RawMaterialRequestOrders.AddRangeAsync(orders);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get request for production order id.
        /// </summary>
        /// <param name="productionOrderIds">Production order ids to find.</param>
        /// <returns>Raw material request related to production orders.</returns>
        public async Task<List<RawMaterialRequestModel>> GetRawMaterialRequestByProductionOrderIds(params int[] productionOrderIds)
        {
            var ids = (await this.GetRawMaterialRequestOrdersByProductionOrderIds(productionOrderIds)).Select(x => x.RequestId).Distinct().ToList();

            var allRequest = await this.databaseContext.RawMaterialRequests.Where(x => ids.Contains(x.Id)).ToListAsync();
            var allRequestOrders = await this.databaseContext.RawMaterialRequestOrders.Where(x => ids.Contains(x.RequestId)).ToListAsync();
            var allRequestDetails = await this.databaseContext.RawMaterialRequestDetails.Where(x => ids.Contains(x.RequestId)).ToListAsync();

            foreach (var request in allRequest)
            {
                request.ProductionOrderIds = allRequestOrders.Where(x => x.RequestId.Equals(request.Id)).Select(x => x.ProductionOrderId).ToList();
                request.OrderedProducts = allRequestDetails.Where(x => x.RequestId.Equals(request.Id)).ToList();
            }

            return allRequest;
        }

        /// <summary>
        /// Get request for production order id.
        /// </summary>
        /// <param name="productionOrderIds">Production order ids to find.</param>
        /// <returns>Raw material request related to production orders.</returns>
        public async Task<List<RawMaterialRequestOrderModel>> GetRawMaterialRequestOrdersByProductionOrderIds(params int[] productionOrderIds)
        {
            var ids = productionOrderIds.Distinct().ToList();
            return  await this.databaseContext.RawMaterialRequestOrders.Where(x => ids.Contains(x.ProductionOrderId)).ToListAsync();
        }
    }
}

