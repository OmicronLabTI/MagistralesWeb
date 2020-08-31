// <summary>
// <copyright file="ISapFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// interface for sapFacade.
    /// </summary>
    public interface ISapFacade
    {
        /// <summary>
        /// Method to return orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="docEntry">the order ir.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetDetallePedidos(string docEntry);

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="pedidosId">the order ir.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetPedidoWithDetail(List<int> pedidosId);

        /// <summary>
        /// Gets the production orders bu produc and id.
        /// </summary>
        /// <param name="pedidosId">list ids each elemente is orderId-producId.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetProdOrderByOrderItem(List<string> pedidosId);

        /// <summary>
        /// gets the formula.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrderFormula(int orderId);

        /// <summary>
        /// Gets the componenets based in the dic.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetComponents(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the componentes managed by batches.
        /// </summary>
        /// <param name="ordenId">the order id.</param>
        /// <returns>the components.</returns>
        Task<ResultDto> GetBatchesComponents(int ordenId);

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetlLastIsolatedProductionOrderId(string productId, string uniqueId);

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetNextBatchCode(string productCode);
    }
}
