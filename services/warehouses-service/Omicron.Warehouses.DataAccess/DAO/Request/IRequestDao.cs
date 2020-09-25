// <summary>
// <copyright file="IRequestDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.DataAccess.DAO.Request
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Warehouses.Entities.Model;

    public interface IRequestDao
    {
        /// <summary>
        /// Method for add new raw material request
        /// </summary>
        /// <param name="request">Request to add.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertRawMaterialRequest(RawMaterialRequestModel request);

        /// <summary>
        /// Method for add detail of raw material request.
        /// </summary>
        /// <param name="detail">Raw material request detail to add.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertDetailsOfRawMaterialRequest(List<RawMaterialRequestDetailModel> detail);

        /// <summary>
        /// Method for add production orders related to  raw material request.
        /// </summary>
        /// <param name="detail">Production order ids to add.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertOrdersOfRawMaterialRequest(List<RawMaterialRequestOrderModel> orders);

        /// <summary>
        /// Get request for production order id.
        /// </summary>
        /// <param name="productionOrderIds">Production order ids to find.</param>
        /// <returns>Raw material request related to production orders.</returns>
        Task<List<RawMaterialRequestModel>> GetRawMaterialRequestByProductionOrderIds(params int[] productionOrderIds);

        /// <summary>
        /// Get request-orders for production order id.
        /// </summary>
        /// <param name="productionOrderIds">Production order ids to find.</param>
        /// <returns>Raw material request ids related to production order ids.</returns>
        Task<List<RawMaterialRequestOrderModel>> GetRawMaterialRequestOrdersByProductionOrderIds(params int[] productionOrderIds);
    }
}

