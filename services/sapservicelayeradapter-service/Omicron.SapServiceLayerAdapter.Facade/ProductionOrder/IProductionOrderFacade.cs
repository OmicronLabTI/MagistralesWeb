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
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> FinishOrder(List<CloseProductionOrderDto> productionOrders);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the object to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula);
    }
}