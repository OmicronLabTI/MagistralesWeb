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
        /// returns the orders ids.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrderBySalesOrder(List<int> listIds);

        /// <summary>
        /// Get the user order by fabrication order id.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrderByFabOrder(List<int> listIds);

        /// <summary>
        /// Gets the QFB orders (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabOrderByUserId(string userId);

        /// <summary>
        /// Gets the list of user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrdersByUserId(List<string> listIds);

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
        /// updates order comments.
        /// </summary>
        /// <param name="updateComments">Fabrication order comments.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateFabOrderComments(List<UpdateOrderCommentsModel> updateComments);

        /// <summary>
        /// Gets the connection to sap di api.
        /// </summary>
        /// <returns>the conection.</returns>
        Task<ResultModel> ConnectDiApi();

        /// <summary>
        /// Change order status to finish.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CloseSalesOrders(List<OrderIdModel> finishOrders);

        /// <summary>
        /// Finish fabrication orders.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</param>
        /// <returns>Orders with updated info.</returns>urns>
        Task<ResultModel> CloseFabOrders(List<CloseProductionOrderModel> finishOrders);

        /// <summary>
        /// Makes the call to assign batches.
        /// </summary>
        /// <param name="assignBatches">the batches.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateBatches(List<AssignBatchModel> assignBatches);

        /// <summary>
        /// Save signatures.
        /// </summary>
        /// <param name="signatureType">The signature type.</param>
        /// <param name="signatureModel">The signature info.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> UpdateOrderSignature(SignatureType signatureType, UpdateOrderSignatureModel signatureModel);

        /// <summary>
        /// Get production order signatures.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> GetOrderSignatures(int productionOrderId);

        /// <summary>
        /// Finish the order by the QFB.
        /// </summary>
        /// <param name="updateOrderSignature">the model.</param>
        /// <returns>the result.</returns>
        Task<ResultModel> FinishOrder(FinishOrderModel updateOrderSignature);

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> CreateIsolatedProductionOrder(CreateIsolatedFabOrderModel isolatedFabOrder);

        /// <summary>
        /// Gets the ordersby the filter.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetFabOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the completed batch.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CompletedBatches(int orderId);

        /// <summary>
        /// gets the data to send to print.
        /// </summary>
        /// <param name="ordersId">the sales orders..</param>
        /// <returns>the data.</returns>
        Task<ResultModel> PrintOrders(List<int> ordersId);

        /// <summary>
        /// Updates the saleorder comments.
        /// </summary>
        /// <param name="updateOrder">the order to update.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateSaleOrders(UpdateOrderCommentsModel updateOrder);

        /// <summary>
        /// Updates the orders designer label.
        /// </summary>
        /// <param name="updateDesignerLabels">the data to save.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateDesignerLabel(UpdateDesignerLabelModel updateDesignerLabels);

        /// <summary>
        /// Creates the pdf for the sale orders.
        /// </summary>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateSaleOrderPdf(List<int> ordersId);

        /// <summary>
        /// Send the petition to delete.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultModel> DeleteFiles();

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForAlmacen();

        /// <summary>
        /// Updates user orders.
        /// </summary>
        /// <param name="listOrders">the list of orders.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateUserOrders(List<UserOrderModel> listOrders);

        /// <summary>
        /// Gets the orders for deliveru.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForDelivery();
    }
}
