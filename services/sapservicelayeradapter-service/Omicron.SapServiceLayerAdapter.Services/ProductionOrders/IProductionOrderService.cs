// <summary>
// <copyright file="IProductionOrderService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.ProductionOrders
{
    /// <summary>
    /// the interface.
    /// </summary>
    public interface IProductionOrderService
    {
        /// <summary>
        /// insert the fab orders.
        /// </summary>
        /// <param name="orderWithDetail">the list of data.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateFabOrder(List<OrderWithDetailDto> orderWithDetail);

        /// <summary>
        /// Update the fabrication orders.
        /// </summary>
        /// <param name="orderModels">the models to update.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateFabOrders(List<UpdateFabOrderDto> orderModels);

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> FinishOrder(List<CloseProductionOrderDto> productionOrders);

        /// <summary>
        /// Update Production Orders Batches.
        /// </summary>
        /// <param name="batchesToAssign">Batches To Assign.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> UpdateProductionOrdersBatches(List<AssignBatchDto> batchesToAssign);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the formula.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> UpdateFormula(UpdateFormulaDto updateFormula);
    }
}