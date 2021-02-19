// <summary>
// <copyright file="ISapService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;

    /// <summary>
    /// The interface for sap.
    /// </summary>
    public interface ISapService
    {
        /// <summary>
        /// get the orders.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="docId">the doc id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetOrderDetails(int docId);

        /// <summary>
        /// Gets the orders with their detail.
        /// </summary>
        /// <param name="pedidosIds">the detail.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetPedidoWithDetail(List<int> pedidosIds);

        /// <summary>
        /// Gets the production orders bu produc and id.
        /// </summary>
        /// <param name="pedidosIds">list ids each elemente is orderId-producId.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetProdOrderByOrderItem(List<string> pedidosIds);

        /// <summary>
        /// Gets the formula of the orden de fabricaion.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <param name="returnFirst">if it returns only the first.</param>
        /// <param name="returnDetails">Return the details.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrderFormula(List<int> listIds, bool returnFirst, bool returnDetails);

        /// <summary>
        /// Get fabrication orders by criterial.
        /// </summary>
        /// <param name="salesOrderIds">Sales order ids.</param>
        /// <param name="fabricationOrderIds">Production order ids.</param>
        /// <param name="components">Flag for get components.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabricationOrdersByCriterial(List<int> salesOrderIds, List<int> fabricationOrderIds, bool components);

        /// <summary>
        /// gets the items from the dict.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetComponents(Dictionary<string, string> parameters);

        /// <summary>
        /// Get products management by batches with criterials.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetProductsManagmentByBatch(Dictionary<string, string> parameters);

        /// <summary>
        /// Get the components managed by batches.
        /// </summary>
        /// <param name="ordenId">the ordenid.</param>
        /// <returns>the data to return.</returns>
        Task<ResultModel> GetBatchesComponents(int ordenId);

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetlLastIsolatedProductionOrderId(string productId, string uniqueId);

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetNextBatchCode(string productCode);

        /// <summary>
        /// Validate if exists batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <param name="batchCode">the batch code.</param>
        /// <returns>the validation result.</returns>
        Task<ResultModel> ValidateIfExistsBatchCodeByItemCode(string productCode, string batchCode);

        /// <summary>
        /// Gets the ordersby the filter.
        /// </summary>
        /// <param name="orderFabModel">the params.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabOrders(GetOrderFabModel orderFabModel);

        /// <summary>
        /// Gets the orderd by id.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabOrdersById(List<int> ordersId);

        /// <summary>
        /// Gets the recipes.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetRecipe(int orderId);

        /// <summary>
        /// Gets the recipes.
        /// </summary>
        /// <param name="ordersId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOriginalRouteRecipes(List<int> ordersId);

        /// <summary>
        /// Validates if the order is ready to be finished.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> ValidateOrder(int orderId);

        /// <summary>
        /// Gets the consecutive or previous.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <param name="kind">the kind to look.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetDetails(Dictionary<string, string> parameters, string kind);
    }
}
