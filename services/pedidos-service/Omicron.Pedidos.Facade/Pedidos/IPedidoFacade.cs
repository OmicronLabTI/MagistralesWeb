// <summary>
// <copyright file="IPedidoFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Dtos.Models;

    /// <summary>
    /// interfaces for the pedidos.
    /// </summary>
    public interface IPedidoFacade
    {
        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="orderDto">the pedidos list.</param>
        /// <returns>the result.</returns>
        Task<ResultDto> ProcessOrders(ProcessOrderDto orderDto);

        /// <summary>
        /// returns the list of userOrder by sales order.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrderBySalesOrder(List<int> listIds);

        /// <summary>
        /// Gets the orders of a specific QFB (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the list to returns.</returns>
        Task<ResultDto> GetFabOrderByUserID(string userId);

        /// <summary>
        /// Gets the user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrdersByUserId(List<string> listIds);

        /// <summary>
        /// Assigns the order.
        /// </summary>
        /// <param name="manualAssign">the dto to assign.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> AssignHeader(ManualAssignDto manualAssign);

        /// <summary>
        /// updates the formulas for the order.
        /// </summary>
        /// <param name="updateFormula">the update object.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateComponents(UpdateFormulaDto updateFormula);

        /// <summary>
        /// updates the status of the orders.
        /// </summary>
        /// <param name="updateStatus">the status object.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateStatusOrder(List<UpdateStatusOrderDto> updateStatus);

        /// <summary>
        /// gets the connection to DI api.
        /// </summary>
        /// <returns>the connectin.</returns>
        Task<ResultDto> ConnectDiApi();

        /// <summary>
        /// Process by order.
        /// </summary>
        /// <param name="processByOrder">process by order dto.</param>
        /// <returns>the order.</returns>
        Task<ResultDto> ProcessByOrder(ProcessByOrderDto processByOrder);

        /// <summary>
        /// Change order status to cancel.
        /// </summary>
        /// <param name="cancelOrders">Update orders info.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultDto> CancelOrder(List<CancelOrderDto> cancelOrders);
    }
}
