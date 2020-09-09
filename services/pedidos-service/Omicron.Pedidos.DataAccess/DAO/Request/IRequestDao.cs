// <summary>
// <copyright file="IRequestDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Request
{
    using Omicron.Pedidos.Entities.Model.Db;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
        /// Get request for production order id.
        /// </summary>
        /// <param name="productionOrderId">Production order id to find.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<RawMaterialRequestModel> GetRawMaterialRequestByProductionOrderId(int productionOrderId);
    }
}

