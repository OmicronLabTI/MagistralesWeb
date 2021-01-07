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
    using Omicron.Pedidos.Resources.Enums;

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
        /// Get the user order by fabrication order id.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrderByFabOrder(List<int> listIds);

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
        /// updates order comments.
        /// </summary>
        /// <param name="updateComments">Fabrication order comments.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateFabOrderComments(List<UpdateOrderCommentsDto> updateComments);

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
        Task<ResultDto> CancelOrder(List<OrderIdDto> cancelOrders);

        /// <summary>
        /// Change order status to finish.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultDto> CloseSalesOrders(List<OrderIdDto> finishOrders);

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</para
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultDto> CancelFabOrder(List<OrderIdDto> cancelOrders);

        /// <summary>
        /// Finish fabrication orders.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</para
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultDto> CloseFabOrders(List<CloseProductionOrderDto> finishOrders);

        /// <summary>
        /// the automatic assign.
        /// </summary>
        /// <param name="automaticAssing">the assign object.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> AutomaticAssign(AutomaticAssingDto automaticAssing);

        /// <summary>
        /// Updates the batches.
        /// </summary>
        /// <param name="assignBatch">the objecto to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateBatches(List<AssignBatchDto> assignBatch);

        /// <summary>
        /// Save signatures.
        /// </summary>
        /// <param name="signatureType">The signature type.</param>
        /// <param name="signatureModel">The signature info.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> UpdateOrderSignature(SignatureType signatureType, UpdateOrderSignatureDto signatureModel);

        /// <summary>
        /// Get production order signatures.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> GetOrderSignatures(int productionOrderId);

        /// <summary>
        /// finish the order by the qfb.
        /// </summary>
        /// <param name="updateOrderSignature">the signature dto.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> FinishOrder(FinishOrderDto updateOrderSignature);

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        Task<ResultDto> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder);

        /// <summary>
        /// Look for the orders.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetFabOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Reassigns the orde to a user.
        /// </summary>
        /// <param name="manualAssign">the object to reassign.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> ReassignOrder(ManualAssignDto manualAssign);

        /// <summary>
        /// Gets the productivity indicators.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetProductivityData(Dictionary<string, string> parameters);

        /// <summary>
        /// Create custom component list.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="customList">The custom list.</param>
        /// <returns>New custom list.</returns>
        Task<ResultDto> CreateCustomComponentList(string userId, CustomComponentListDto customList);

        /// <summary>
        /// Get custom components list by product id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns>Custom lists.</returns>
        Task<ResultDto> GetCustomComponentListByProductId(string productId);

        /// <summary>
        /// Gets the workload.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetWorkLoad(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets if the order has batches.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>if the batches are completed.</returns>
        Task<ResultDto> CompletedBatches(int orderId);

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="ordersId">the pedidos list.</param>
        /// <returns>the result.</returns>
        Task<ResultDto> PrintOrders(List<int> ordersId);

        /// <summary>
        /// updates the sale orders.
        /// </summary>
        /// <param name="updateSaleOrder">the update orders.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateSaleOrders(UpdateOrderCommentsDto updateSaleOrder);

        /// <summary>
        /// Updates de designer label value and signature.
        /// </summary>
        /// <param name="updateDesignerLabel">the objects.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateDesignerLabel(UpdateDesignerLabelDto updateDesignerLabel);

        /// <summary>
        /// Create the pdf for the sale order.
        /// </summary>
        /// <param name="orderIds">the orders id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreateSaleOrderPdf(List<int> orderIds);

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> DeleteFiles();

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForAlmacen();

        /// <summary>
        /// Updates the user orders.
        /// </summary>
        /// <param name="userOrders">The orders.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateUserOrders(List<UserOrderDto> userOrders);

        /// <summary>
        /// Get the orders for delivery.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForDelivery();

        /// <summary>
        /// Gets the deliveries for the invoices.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForInvoice();

        /// <summary>
        /// Gets the order for the packages by type.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForPackages(string type);
    }
}
