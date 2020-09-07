// <summary>
// <copyright file="PedidosDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Request
{
    using Omicron.Pedidos.Entities.Context;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model.Db;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

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
        /// Get request for production order id.
        /// </summary>
        /// <param name="productionOrderId">Production order id to find.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<RawMaterialRequestModel> GetRawMaterialRequestByProductionOrderId(int productionOrderId)
        {
            var request = await this.databaseContext.RawMaterialRequests.FirstOrDefaultAsync(x => x.ProductionOrderId.Equals(productionOrderId));
            
            if (request != null)
            {
                request.OrderedProducts = await this.databaseContext.RawMaterialRequestDetails.Where(x => x.RequestId.Equals(request.Id)).ToListAsync();
            }

            return request;
        }
    }
}

