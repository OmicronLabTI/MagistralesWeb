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
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="orderLog">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> InsertOrderLog(List<OrderLogModel> orderLog);

        /// <summary>
        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderBySaleOrder(List<string> listIDs);

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
        /// <param name="listStatus">the list of users.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<UserOrderModel>> GetUserOrderByStatus(List<string> listStatus);

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
        /// <returns>the data.</returns>
        Task<List<UserOrderModel>>GetSaleOrderForAlmacen(string status, DateTime dateToLook);

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
    }
}
