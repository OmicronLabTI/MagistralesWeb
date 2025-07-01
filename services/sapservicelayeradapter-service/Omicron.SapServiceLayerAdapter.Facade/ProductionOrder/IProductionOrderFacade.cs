// <summary>
// <copyright file="IProductionOrderFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.ProductionOrder
{
    /// <summary>
    /// Interface IProductionOrderFacade.
    /// </summary>
    public interface IProductionOrderFacade
    {
        /// <summary>
        /// insert the fab orders.
        /// </summary>
        /// <param name="orderWithDetail">the list of data.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetail);

        /// <summary>
        /// Update the fabrication orders.
        /// </summary>
        /// <param name="orderModels">the models to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateFabOrders(List<UpdateFabOrderDto> orderModels);

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> FinishOrder(List<CloseProductionOrderDto> productionOrders);

        /// <summary>
        /// Primary Validation For Production Order Finalization In Sap.
        /// </summary>
        /// <param name="productionOrderInfoToValidate">Production Order Info To Validate.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> PrimaryValidationForProductionOrderFinalizationInSap(List<CloseProductionOrderDto> productionOrderInfoToValidate);

        /// <summary>
        /// Update Production Orders Batches.
        /// </summary>
        /// <param name="batchesToAssign">Batches To Assign.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> UpdateProductionOrdersBatches(List<AssignBatchDto> batchesToAssign);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the object to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula);

        /// <summary>
        /// Cancel a prodution order.
        /// </summary>
        /// <param name="productionOrder">Production order to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CancelProductionOrder(CancelOrderDto productionOrder);

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder);
    }
}