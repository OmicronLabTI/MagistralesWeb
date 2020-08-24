// <summary>
// <copyright file="IPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;

    /// <summary>
    /// The pedidos service interface.
    /// </summary>
    public interface IPedidosService
    {
        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosId">the ids of the orders.</param>
        /// <returns>the result.</returns>
        Task<ResultModel> ProcessOrders(ProcessOrderModel pedidosId);

        /// <summary>
        /// returns the orders ids.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds);

        /// <summary>
        /// Gets the QFB orders (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabOrderByUserID(string userId);

        /// <summary>
        /// Gets the list of user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrdersByUserId(List<string> listIds);

        /// <summary>
        /// Assign the orders.
        /// </summary>
        /// <param name="manualAssign">the manual assign.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> AssignOrder(ManualAssignModel manualAssign);

        /// <summary>
        /// Updates the formula for an order.
        /// </summary>
        /// <param name="updateFormula">upddates the formula.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateComponents(UpdateFormulaModel updateFormula);

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="updateStatusOrder">the status model.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateStatusOrder(List<UpdateStatusOrderModel> updateStatusOrder);

        /// <summary>
        /// Gets the connection to sap di api.
        /// </summary>
        /// <returns>the conection.</returns>
        Task<ResultModel> ConnectDiApi();

        /// <summary>
        /// Process by order.
        /// </summary>
        /// <param name="processByOrder">the orders.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> ProcessByOrder(ProcessByOrderModel processByOrder);

        /// <summary>
        /// Change order status to cancel.
        /// </summary>
        /// <param name="cancelOrders">Update orders info.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CancelOrder(List<CancelOrderModel> cancelOrders);

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</para
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CancelFabOrder(List<CancelOrderModel> cancelOrders);

        /// <summary>
        /// Makes the automatic assign.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> AutomaticAssign(AutomaticAssingModel assignModel);

        /// <summary>
        /// Makes the call to assign batches.
        /// </summary>
        /// <param name="assignBatches">the batches.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateBatches(List<AssignBatchModel> assignBatches);

        /// Save signatures.
        /// </summary>
        /// <param name="signatureType">The signature type.</param>
        /// <param name="signatureModel">The signature info.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> UpdateOrderSignature(SignatureTypeEnum signatureType, UpdateOrderSignatureModel signatureModel);

        /// <summary>
        /// Get production order signatures.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> GetOrderSignatures(int productionOrderId);
    }
}
