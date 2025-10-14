// <summary>
// <copyright file="IPedidosDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPedidosDao
    {
        /// <summary>
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="userorder">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertUserOrder(List<UserOrderModel> userorder);

        /// <summary>
        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderBySaleOrder(List<string> listIDs);

        /// <summary>
        /// Returns the user orders by InvoiceId (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByInvoiceId(List<int> listIDs);

        /// <summary>
        /// Gets only the sale orders by id.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetOnlySaleOrderBySaleId(List<string> listIds);

        /// <summary>
        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByProducionOrder(List<string> listIDs);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByUserId(List<string> listIds);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <param name="status">the list of status.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByUserIdAndStatusAndTecnic(List<string> listIds, List<string> status);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByUserIdAndStatus(List<string> listIds, List<string> status);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByUserIdAndNotInStatus(List<string> listIds, List<string> status);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of sale order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderBySalesOrderIdAndNotInStatus(List<string> listIDs, List<string> status);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listStatus">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByStatus(List<string> listStatus);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listStatus">the list of users.</param>
        /// <param name="almacenStatusToIgnore">almacenStatusToIgnore.</param>
        /// <param name="startDate">StartDate.</param>
        /// <param name="endDate">EndDate.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderForDelivery(List<string> listStatus, string almacenStatusToIgnore, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByFechaFin(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByFechaClose(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Updates the entries.
        /// </summary>
        /// <param name="userOrderModels">the user model.</param>
        /// <returns>the data.</returns>
        Task<bool> UpdateUserOrders(List<UserOrderModel> userOrderModels);

        /// <summary>
        /// Method for add order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to add.</param>
        /// <returns>Operation result</returns>
        Task<bool> InsertOrderSignatures(UserOrderSignatureModel orderSignature);

        /// <summary>
        /// Method for add order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to add.</param>
        /// <returns>Operation result</returns>
        Task<bool> InsertOrderSignatures(List<UserOrderSignatureModel> orderSignature);

        /// <summary>
        /// Method for save order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to save.</param>
        /// <returns>Operation result</returns>
        Task<bool> SaveOrderSignatures(UserOrderSignatureModel orderSignature);

        /// <summary>
        /// Method for save order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to save.</param>
        /// <returns>Operation result</returns>
        Task<bool> SaveOrderSignatures(List<UserOrderSignatureModel> orderSignature);

        /// <summary>
        /// Get order signature by user order id.
        /// </summary>
        /// <param name="userOrderId">User order to find.</param>
        /// <returns>Operation result</returns>
        Task<UserOrderSignatureModel> GetSignaturesByUserOrderId(int userOrderId);

        /// <summary>
        /// Get order signature by user order id.
        /// </summary>
        /// <param name="userOrderId">User order to find.</param>
        /// <returns>Operation result</returns>
        Task<IEnumerable<UserOrderSignatureModel>> GetSignaturesByUserOrderId(List<int> userOrderId);

        /// <summary>
        /// Insert new custom component list.
        /// </summary>
        /// <param name="customComponentList">Custom list to insert.</param>
        /// <returns>Operation result</returns>
        Task<bool> InsertCustomComponentList(CustomComponentListModel customComponentList);

        /// <summary>
        /// Insert new components of custom list.
        /// </summary>
        /// <param name="components">Components of custom list to insert.</param>
        /// <returns>Operation result.</returns>
        Task<bool> InsertComponentsOfCustomList(List<ComponentCustomComponentListModel> components);

        /// <summary>
        /// Get all custom component lists for product id.
        /// </summary>
        /// <param name="productId">Te product id.</param>
        /// <returns>Related lists.</returns>
        Task<List<CustomComponentListModel>> GetCustomComponentListByProduct(string productId);

        /// <summary>
        /// Get all component for custom list id.
        /// </summary>
        /// <param name="customListIds">Te custom list ids.</param>
        /// <returns>Related components.</returns>
        Task<List<ComponentCustomComponentListModel>> GetComponentsByCustomListId(List<int> customListIds);

        /// <summary>
        /// Gets the data by field.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>the data.</returns>
        Task<List<ParametersModel>> GetParamsByFieldContains(string fieldName);

        /// <summary>
        /// Gets the data by field.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>the data.</returns>
        Task<List<ParametersModel>> GetParamsByFieldContainsQueryOnly(string fieldName);

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="userOrderId">the orders ids.</param>
        /// <returns>the data.</returns>
        Task<List<ProductionOrderQr>> GetQrRoute(List<int> userOrderId);

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="saleOrder">the orders ids.</param>
        /// <returns>the data.</returns>
        Task<List<ProductionRemisionQrModel>> GetQrRemisionRouteBySaleOrder(List<int> saleOrder);

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="delivery">the orders ids.</param>
        /// <returns>the data.</returns>
        Task<List<ProductionRemisionQrModel>> GetQrRemisionRouteByDelivery(List<int> delivery);

        /// <summary>
        /// Gets the production qr invoice by invoiceid.
        /// </summary>
        /// <param name="invoiceId">the list of invoices.</param>
        /// <returns>the data.</returns>
        Task<List<ProductionFacturaQrModel>> GetQrFacturaRouteByInvoice(List<int> invoiceId);

        /// <summary>
        /// Inserts the data to the data base.
        /// </summary>
        /// <param name="modelsToSave">the models to save.</param>
        /// <returns>the data.</returns>
        Task<bool> InsertQrRouteFactura(List<ProductionFacturaQrModel> modelsToSave);

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="modelsToSave">the orders ids.</param>
        /// <returns>the data.</returns>
        Task<bool> InsertQrRouteRemision(List<ProductionRemisionQrModel> modelsToSave);

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="modelsToSave">the orders ids.</param>
        /// <returns>the data.</returns>
        Task<bool> InsertQrRoute(List<ProductionOrderQr> modelsToSave);

        /// <summary>
        /// Gets the orders for almance.
        /// </summary>
        /// <param name="status">The status tu,</param>
        /// <param name="dateToLook">the min date to look.</param>
        /// <param name="statusPending">The status for pending.</param>
        /// <param name="secondStatus">The second status.</param>
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>> GetSaleOrderForAlmacen(string status, DateTime dateToLook, List<string> statusPending, string secondStatus);

        /// <summary>
        /// Gets the orders for almance.
        /// </summary>
        /// <param name="status">The status tu,</param>
        /// <param name="dateToLook">the min date to look.</param>
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>> GetOrderForAlmacenToIgnore(string status, DateTime dateToLook);

        /// <summary>
        /// GEts the orders by id.
        /// </summary>
        /// <param name="ordersId">th eorderd id.</param>
        /// <returns>the orders.</returns>
        Task<List<UserOrderModel>> GetUserOrdersById(List<int> ordersId);

        /// <summary>
        /// GEts the orders by id.
        /// </summary>
        /// <param name="ordersId">th eorderd id.</param>
        /// <returns>the orders.</returns>
        /// <param name="statusForSale">the status for the sale.</param>
        /// <param name="statusForOrder">the status for the order.</param>
        Task<List<UserOrderModel>> GetUserOrdersForInvoice(string statusForSale, string statusForOrder);

        /// <summary>
        /// Gets the production qr invoice by invoiceid.
        /// </summary>
        /// <param name="invoiceId">the list of invoices.</param>
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>> GetUserOrdersByInvoiceId(List<int> invoiceId);

        /// <summary>
        /// Gets the production qr invoice by invoiceid.
        /// </summary>
        /// <param name="invoiceId">the invoice.</param>
        /// <param name="linenumbers">the list of linenumbers.</param>
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>> GetUserOrdersByInvoiceIdAndLineNumber(List<int> invoiceId, List<int> linenumbers);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listStatus">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByStatusInvoice(List<string> listStatus);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="types">the list of users.</param>
        /// <param name="startDate">startDate.</param>
        /// <param name="endDate">startDate.</param>
        /// <param name="statusToSearch">statusToSearch.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByInvoiceType(List<string> types, DateTime startDate, DateTime endDate, List<string> statusToSearch);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="types">the list of users.</param>
        /// <param name="invoiceId">invoiceId.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByInvoiceTypeAndId(List<string> types, List<int> invoiceId);

        /// <summary>
        /// Get the data by finalized date.
        /// </summary>
        /// <param name="init">the date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByFinalizeDate(DateTime init, DateTime endDate);

        /// <summary>
        /// Looks the orders by delivery id.
        /// </summary>
        /// <param name="deliveryIds">the deliveryies.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByDeliveryId(List<int> deliveryIds);

        /// Get all custom component lists for product id and name.
        /// </summary>
        /// <param name="productId">Te product id.</param>
        /// <param name="name">Te name.</param>
        /// <returns>Related lists.</returns>
        Task<List<CustomComponentListModel>> GetCustomComponentListByProductAndName(string productId, string name);

        /// <summary>
        /// Delete custom component list.
        /// </summary>
        /// <param name="customComponentList">Custom list to insert.</param>
        /// <returns>Operation result</returns>
        Task<bool> DeleteCustomComponentList(CustomComponentListModel customComponentList);

        /// <summary>
        /// Delete components of custom list.
        /// </summary>
        /// <param name="components">Components of custom list to insert.</param>
        /// <returns>Operation result.</returns>
        Task<bool> DeleteComponentsOfCustomList(List<ComponentCustomComponentListModel> components);

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByPlanningDate(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Returns the user order by tecnic id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByTecnicId(List<string> listIds);

        /// <summary>
        /// GetSaleOrderForAlmacenByRangeDates.
        /// </summary>
        /// <param name="startDate">Start Date.</param>
        /// <param name="endDate">End Date.</param>
        /// <param name="statusPending">The status for pending.</param>
        /// <param name="status">The status.</param>
        /// <param name="secondStatus">The second status.</param>
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>> GetSaleOrderForAlmacenByRangeDates(DateTime startDate, DateTime endDate, List<string> statusPending, string status, string secondStatus);

        /// <summary>
        /// GetSaleOrderForAlmacenByOrderIds.
        /// </summary>
        /// <param name="orderIds">OrderIds.</param>
        /// <param name="statusPending">The status for pending.</param>
        /// <param name="status">The status.</param>
        /// <param name="secondStatus">The second status.</param>
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>> GetSaleOrderForAlmacenByOrderIds(List<string> orderIds, List<string> statusPending, string status, string secondStatus);

        /// <summary>
        /// GetUserOrdersForInvoiceByRangeDates.
        /// <returns>the orders.</returns>
        /// <param name="startDate">Start Date.</param>
        /// <param name="endDate">End Date.</param>
        /// <param name="statusForSale">the status for the sale.</param>
        /// <param name="statusForOrder">the status for the order.</param>
        /// </summary>
        Task<IEnumerable<UserOrdersForInvoicesModel>> GetUserOrdersForInvoiceByRangeDates(DateTime startDate, DateTime endDate, string statusForSale, string statusForOrder);

        /// <summary>
        /// GetUserOrdersForInvoiceByDeliveryIds.
        /// </summary>
        /// <param name="deliveryIds">deliveryIds.</param>
        /// <param name="statusForSale">the status for the sale.</param>
        /// <param name="statusForOrder">the status for the order.</param>
        /// </summary>
        Task<IEnumerable<UserOrdersForInvoicesModel>> GetUserOrdersForInvoiceByDeliveryIds(List<int> deliveryIds, string statusForSale, string statusForOrder);

        /// <summary>
        /// Updates the data to the data base.
        /// </summary>
        /// <param name="modelsToSave"> the models to save. </param>
        /// <returns> the data. </returns>
        Task<bool> UpdatesQrRouteFactura(List<ProductionFacturaQrModel> modelsToSave);

        /// <summary>
        /// InsertProductionOrderProcessingStatus.
        /// </summary>
        /// <param name="productionOrderProcessingStatus">productionOrderProcessingStatus.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertProductionOrderProcessingStatus(List<ProductionOrderProcessingStatusModel> productionOrderProcessingStatus);

        /// <summary>
        /// UpdatesProductionOrderProcessingStatus.
        /// </summary>
        /// <param name="productionOrderProcessingStatus">productionOrderProcessingStatus.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> UpdatesProductionOrderProcessingStatus(List<ProductionOrderProcessingStatusModel> productionOrderProcessingStatus);

        /// <summary>
        /// GetProductionOrderProcessingStatusByProductionOrderIds.
        /// </summary>
        /// <param name="productionOrderIds">productionOrderIds.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<ProductionOrderProcessingStatusModel>> GetProductionOrderProcessingStatusByProductionOrderIds(IEnumerable<int> productionOrderIds);

        /// <summary>
        /// GetFirstProductionOrderProcessingStatusByProductionOrderId.
        /// </summary>
        /// <param name="productionOrderId">productionOrderId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ProductionOrderProcessingStatusModel> GetFirstProductionOrderProcessingStatusByProductionOrderId(int productionOrderId);

        /// <summary>
        /// GetFirstProductionOrderProcessingStatusById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ProductionOrderProcessingStatusModel> GetFirstProductionOrderProcessingStatusById(string id);

        /// <summary>
        /// GetAllProductionOrderProcessingStatusByStatus.
        /// </summary>
        /// <param name="status">status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<ProductionOrderProcessingStatusModel>> GetAllProductionOrderProcessingStatusByStatus(List<string> status);

        /// <summary>
        /// GetProductionOrderSeparationByOrderId.
        /// </summary>
        /// <param name="ordersIds">production order ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ProductionOrderSeparationModel>> GetProductionOrderSeparationByOrderId(List<int> ordersIds);

        /// <summary>
        /// GetProductionOrderSeparationByOrderId.
        /// </summary>
        /// <param name="ordersIds">production order ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ProductionOrderSeparationDetailModel>> GetProductionOrderSeparationDetailByDetailOrderId(List<int> ordersIds);

        /// <summary>
        /// GetProductionOrderSeparationDetailBySapOrderId.
        /// </summary>
        /// <param name="ordersIds">production order ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ProductionOrderSeparationDetailModel>> GetProductionOrderSeparationDetailBySapOrderId(List<int> ordersIds);

        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="separationId">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetOrdersBySeparationId(string separationId);

        /// <summary>
        /// Insert order details
        /// </summary>
        /// <param name="detaildOrderId">order details</param>
        /// <returns>True was successfully inserted</returns>
        Task<bool> InsertDetailOrder(ProductionOrderSeparationDetailModel detaildOrderId);

        /// <summary>
        /// Insert order parent
        /// </summary>
        /// <param name="orderId">Parent order model</param>
        /// <returns>True was successfully inserted</returns>
        Task<bool> InsertOrder(ProductionOrderSeparationModel orderId);

        /// <summary>
        /// Updates an existing parent order
        /// </summary>
        /// <param name="orderId">Parent order model</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateOrder(ProductionOrderSeparationModel orderId);

        /// <summary>
        /// Gets a parent order by its number
        /// </summary>
        /// <param name="orderId"> parent order number</param>
        /// <returns>order model or null if it does not exist</returns>
        Task<ProductionOrderSeparationModel> GetParentOrderId(int orderId);

        /// <summary>
        /// Gets the maximum split number for a parent order
        /// </summary>
        /// <param name="orderId">Parent order number</param>
        /// <returns>Maximum division number</returns>
        Task<int> GetMaxDivision(int orderId);

        /// <summary>
        /// Gets the detailOrderId
        /// </summary>
        /// <param name="detailOrderId">detailOrderId</param>
        /// <returns>detailOrderId</returns>
        Task<ProductionOrderSeparationDetailModel> GetDetailOrderById(int detailOrderId);

        /// <summary>
        /// Gets fabOrderId with assigned pieces.
        /// </summary>
        /// <param name="fabOrderId">fabOrderId</param>
        /// <returns>fabOrderId order info with assigned pieces</returns>
        Task<OrderFabModel> GetChildOrderWithPieces(int fabOrderId);

        /// <summary>
        /// GetParentOrderById available pieces for a parent order
        /// </summary>
        /// <param name="parentOrderId">Parent order ID</param>
        /// <returns>Success indicator</returns>
        Task<ProductionOrderSeparationModel> GetParentOrderById(int parentOrderId);

        /// <summary>
        /// Updates available pieces for a parent order
        /// </summary>
        /// <returns>Success indicator</returns>
        Task UpdateParentOrder();

        /// <summary>
        /// GetProductionOrderSeparationDetailLogById 
        /// </summary>
        /// <param name="parentOrderId">Separation order ID</param>
        /// <returns>the model</returns>
        Task<ProductionOrderSeparationDetailLogsModel> GetProductionOrderSeparationDetailLogById(string separationId);

        /// <summary>
        /// InsertProductionOrderSeparationDetailLogById 
        /// </summary>
        /// <param name="modelToSave">Model to save</param>
        /// <returns>Success indicator</returns>
        Task<bool> InsertProductionOrderSeparationDetailLogById(ProductionOrderSeparationDetailLogsModel modelToSave);

        /// <summary>
        /// GetProductionOrderSeparationDetailLogById 
        /// </summary>
        /// <param name="modelToSave">Model to save</param>
        /// <returns>Success indicator</returns>
        Task<bool> UpdateProductionOrderSeparationDetailLog(ProductionOrderSeparationDetailLogsModel modelToSave);

        /// <summary>
        /// GetProductionOrderSeparationDetailLogById 
        /// </summary>
        /// <param name="parentId">Parent id</param>
        /// <returns>the model</returns>
        Task<ProductionOrderSeparationDetailLogsModel> GetProductionOrderSeparationDetailLogByParentOrderId(int parentId);

        /// <summary>
        /// GetAllFailedDivisionOrders.
        /// </summary>
        /// <param>status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<ProductionOrderSeparationDetailLogsModel>> GetAllFailedDivisionOrders();

        /// <summary>
        /// GetParentOrderDetailByOrderId.
        /// </summary>
        /// <param name="parentId">Parent id</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ParentOrderDetailModel>> GetParentOrderDetailByOrderId(int parentId);

        /// <summary>
        /// GetAllOpenParentOrdersByQfb.
        /// </summary>
        /// <param>status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<OpenOrderProductionModel>> GetAllOpenParentOrdersByQfb(string qfbId, string partiallyDivided);

        /// <summary>
        /// GetChildrenMapByParents.
        /// </summary>
        /// <param>status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<DetailOrderProductionModel>> GetChildrenByParentIds(IEnumerable<int> parentIds, bool excludeCanceled = true);

        /// <summary>
        /// FindExistingParentIds.
        /// </summary>
        /// <param>status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HashSet<int>> FindExistingParentIds(List<int> ids);

        /// <summary>
        /// FindParentsByChildIds.
        /// </summary>
        /// <param>status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<Dictionary<int, int>> FindParentsByChildIds(List<int> childIds);


        /// <summary>
        /// GetParentsAssignedToQfbByIds.
        /// </summary>
        /// <param>status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<OpenOrderProductionModel>> GetParentsAssignedToQfbByIds(List<int> parentIds, string qfbId, string partiallyDivided );

        /// <summary>
        /// GetProductionOrderSeparationByOrderId.
        /// </summary>
        /// <param name="ordersIds">production order ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<ProductionOrderSeparationModel>> GetProductionOrderSeparationByOrderIdWithPendingPieces(List<int> ordersIds);

        /// <summary>
        /// GetProductionOrderSeparationByOrderId.
        /// </summary>
        /// <param name="parentOrderId">parentOrderId ids.</param>
        /// <param name="workStatus">newWorkStatus.</param>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> UpdateStatusWorkParent(int parentOrderId, string workStatus);

        /// <summary>
        /// IsParentOrder.
        /// </summary>
        /// <param name="productionOrderId">production order ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> IsParentOrder(int productionOrderId);
    }
}
