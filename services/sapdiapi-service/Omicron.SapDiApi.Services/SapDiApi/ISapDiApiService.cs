// <summary>
// <copyright file="ISapDiApiService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using Omicron.SapDiApi.Entities.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// the interface.
    /// </summary>
    public interface ISapDiApiService
    {
        /// <summary>
        /// insert the fab orders.
        /// </summary>
        /// <param name="orderWithDetail">the list of data.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> InsertOrdenFab(List<OrderWithDetailModel> orderWithDetail);

        /// <summary>
        /// Updates the fabrication orders.
        /// </summary>
        /// <param name="orderModels">the models to update.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateFabOrders(List<UpdateFabOrderModel> orderModels);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the formula.</param>
        /// <returns></returns>
        Task<ResultModel> UpdateFormula(UpdateFormulaModel updateFormula);

        /// <summary>
        /// Cancel a prodution order
        /// </summary>
        /// <param name="productionOrder">Production order to update</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CancelProductionOrder(CancelOrderModel productionOrder);

        /// <summary>
        /// The method to update batches.
        /// </summary>
        /// <param name="updateBatches">the update batches.</param>
        /// <returns>the batches updated.</returns>
        Task<ResultModel> UpdateBatches(List<AssignBatchModel> updateBatches);

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        ResultModel FinishOrder(List<CloseProductionOrderModel> productionOrders);

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderModel isolatedFabOrder);

        /// <summary>
        /// Connects to SAP.
        /// </summary>
        /// <returns>the connection.</returns>
        Task<ResultModel> Connect();
    }
}
