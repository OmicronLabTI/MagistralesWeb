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
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> FinishOrder(List<CloseProductionOrderDto> productionOrders);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the formula.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> UpdateFormula(UpdateFormulaDto updateFormula);
    }
}