// <summary>
// <copyright file="ISapDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DataAccess.DAO.Sap
{    
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// the IsapDao.
    /// </summary>
    public interface ISapDao
    {
        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaIni(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaFin(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<List<OrderModel>> GetOrdersById(int pedidoID);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<List<OrderModel>> GetOrdersById(List<int> pedidoID);

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="pedidoId">PedidoID</param>
        /// <returns>the details.</returns>
        Task<IEnumerable<CompleteDetailOrderModel>> GetAllDetails(int pedidoId);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int id);

        /// <summary>
        /// gets the orders by product and item.
        /// </summary>
        /// <param name="pedidoId">the product id.</param>
        /// <param name="productId">the product id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetProdOrderByOrderProduct(int pedidoId, string productId);

        /// <summary>
        /// gets the orders by orderid.
        /// </summary>
        /// <param name="pedidoId">the product id.</param>        
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderById(List<int> pedidoId);

        /// <summary>
        /// gets the fabrication orders by sales order id.
        /// </summary>
        /// <param name="salesOrderIds">the sales order ids.</param>        
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderBySalesOrderId(List<int> salesOrderIds);
        
        /// <summary>
        /// gets the orders by orderid.
        /// </summary>
        /// <param name="fechaInit">initial date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderByCreateDate(DateTime fechaInit, DateTime endDate);

        /// <summary>
        /// Gets the prod by itemcode.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderByItemCode(string itemCode);

        /// <summary>
        /// gets the realtion between WOR1, OITM ans OITW.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetDetalleFormula(int orderId);

        /// <summary>
        /// gets the sap user.
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        Task<Users> GetSapUserById(int userId);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(string value);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsDescription(string value);

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="pedidoId">the pedido id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DetallePedidoModel>> GetPedidoById(int pedidoId);

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="orderId">the pedido id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetComponentByBatches(int orderId);

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<ProductoModel>> GetProductById(string itemCode);

        /// <summary>
        /// gets the valid batches by item.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="warehouse">the warehouse.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteBatchesJoinModel>> GetValidBatches(string itemCode, string warehouse);

        /// <summary>
        /// Gest the batch transaction by order and item code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(string itemCode, int orderId);

        /// <summary>
        /// Gets the record from ITL1 by log entry.
        /// </summary>
        /// <param name="logEntry">the log entry.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<BatchesTransactionQtyModel>> GetBatchTransationsQtyByLogEntry(int logEntry);

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        Task<int> GetlLastIsolatedProductionOrderId(string productId, string uniqueId);

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <param name="batchCodePattern">Batch code pattern.</param>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        Task<string> GetMaxBatchCode(string batchCodePattern, string productCode);

        /// <summary>
        /// Get batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <param name="batchCode">the product code.</param>
        /// <returns>the data.</returns>
        Task<string> GetBatchCode(string productCode, string batchCode);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="criterials">the values to look.</param>
        /// <returns>the value.</returns>
        Task<List<ProductoModel>> GetProductsManagmentByBatch(List<string> criterials);

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <param name="ids">gets the attachments by id</param>
        /// <returns>the attachaments.</returns>
        Task<List<AttachmentModel>> GetAttachmentsById(List<int> ids);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacen(DateTime initDate);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <param name="saleOrderId">The order id.</param>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenById(int saleOrderId);
    }
}
