// <summary>
// <copyright file="ResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapDiApi.Dtos.Models;

    public interface ISapFacade
    {
        /// <summary>
        /// creates order.
        /// </summary>
        /// <returns>the result.</returns>
        Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetailDto);

        /// <summary>
        /// updates the fabriction orders.
        /// </summary>
        /// <param name="updateFabOrderDtos">the orders to update.</param>
        /// <returns>the reult.</returns>
        Task<ResultDto> UpdateFabOrder(List<UpdateFabOrderDto> updateFabOrderDtos);

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the object to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula);

        /// <summary>
        /// Cancel a Production order
        /// </summary>
        /// <param name="productionOrder">Production order to update</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CancelProductionOrder(CancelOrderDto productionOrder);

        /// <summary>
        /// Upfate the batches.
        /// </summary>
        /// <param name="assginBatches">Assign batches.</param>
        /// <returns>the batches.</returns>
        Task<ResultDto> UpdateBatches(List<AssginBatchDto> assginBatches);

        /// <summary>
        /// Finish production orders.
        /// </summary>
        /// <param name="productionOrders">Production orders to finish.</param>
        /// <returns>Operation result.</returns>
        ResultDto FinishOrder(List<CloseProductionOrderDto> productionOrders);

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder);

        /// <summary>
        /// Creates the delivery.
        /// </summary>
        /// <param name="createDelivery">the deliveries.</param>
        /// <returns>the status.</returns>
        Task<ResultDto> CreateDelivery(List<CreateDeliveryDto> createDelivery);

        /// <summary>
        /// connecto to sap.
        /// </summary>
        /// <returns>connects.</returns>
        Task<ResultDto> Connect();
    }
}
