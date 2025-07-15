// <summary>
// <copyright file="IProductionOrdersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.ProductionOrders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Interface for ProductionOrdersService.
    /// </summary>
    public interface IProductionOrdersService
    {
        /// <summary>
        /// Finalize Production Orders Async.
        /// </summary>
        /// <param name="productionOrdersToFinalize">Production Orders To Finalize.</param>
        /// <returns>Process Result.</returns>
        Task<ResultModel> FinalizeProductionOrdersAsync(List<FinalizeProductionOrderModel> productionOrdersToFinalize);

        /// <summary>
        /// Finalize Production Orders On Sap Async.
        /// </summary>
        /// <param name="productionOrderProcessingPayload">Payload With info.</param>
        /// <returns>Process Result.</returns>
        Task<ResultModel> FinalizeProductionOrdersOnSapAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload);

        /// <summary>
        /// Finalize Production Orders On Postgresql Async.
        /// </summary>
        /// <param name="productionOrderProcessingPayload">Payload With info.</param>
        /// <returns>Process Result.</returns>
        Task<ResultModel> FinalizeProductionOrdersOnPostgresqlAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload);

        /// <summary>
        /// Production Order Pdf Generation Async.
        /// </summary>
        /// <param name="productionOrderProcessingPayload">Payload With info.</param>
        /// <returns>Process Result.</returns>
        Task<ResultModel> ProductionOrderPdfGenerationAsync(ProductionOrderProcessingStatusModel productionOrderProcessingPayload);

        /// <summary>
        /// Get Failed Production Orders.
        /// </summary>
        /// <returns>Failed Production Orders.</returns>
        Task<ResultModel> GetFailedProductionOrders();

        /// <summary>
        /// RetryFailedProductionOrderFinalization.
        /// </summary>
        /// <param name="payloadRetry">RetryFailedProductionOrderFinalizationDto.</param>
        /// <returns>Process Result.</returns>
        Task<ResultModel> RetryFailedProductionOrderFinalization(RetryFailedProductionOrderFinalizationDto payloadRetry);
    }
}
