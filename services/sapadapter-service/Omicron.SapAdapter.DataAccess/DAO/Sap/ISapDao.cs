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
    using Omicron.SapAdapter.Entities.Model.BusinessModels;

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
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByIds(List<int> ids);

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
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int init, int end);

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
        /// gets the asesors by salesOrderId.
        /// </summary>
        /// <param name="docsEntry">the list of salesOrderId.</param>        
        /// <returns>the data.</returns>
        Task<IEnumerable<SalesAsesorModel>> GetAsesorWithEmailByIds(List<int> docsEntry);

        /// <summary>
        /// gets the asesors by salesOrderId.
        /// </summary>
        /// <param name="docsEntry">the list of salesOrderId.</param>        
        /// <returns>the data.</returns>
        Task<IEnumerable<SalesPersonModel>> GetAsesorWithEmailByIdsFromTheAsesor(List<int> salesPrsonId);

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
        /// gets the realtion between WOR1, OITM ans OITW.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetDetalleFormula(List<int> orderId);

        /// <summary>
        /// Gets the formula by orders.
        /// </summary>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DetalleFormulaModel>> GetDetalleFormulaByProdOrdId(List<int> ordersId);

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
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(List<string> value);

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
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="orderId">the pedido id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetComponentByBatches(List<int> orderId);

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<ProductoModel>> GetProductById(string itemCode);

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<ProductoModel>> GetProductByCodeBar(string codeBar);

        /// <summary>
        /// gets the valid batches by item.
        /// </summary>
        /// <param name="components">the components.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteBatchesJoinModel>> GetValidBatches(List<CompleteDetalleFormulaModel> components);

        /// <summary>
        /// Gest the batch transaction by order and item code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(string itemCode, int orderId);

        /// <summary>
        /// Gest the batch transaction by order and item code.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<BatchTransacitions>> GetBatchesTransactionByOrderItem(List<int> orderId);

        /// <summary>
        /// Gets the record from ITL1 by log entry.
        /// </summary>
        /// <param name="logEntry">the log entry.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<BatchesTransactionQtyModel>> GetBatchTransationsQtyByLogEntry(List<int> logEntry);

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
        /// Gets the batches by a list of product ids and the dist number.
        /// </summary>
        /// <param name="productCode">the products.</param>
        /// <param name="batchCode">the batch codes.</param>
        /// <returns>the data.</returns>
        Task<List<Batches>> GetBatchByProductDistNumber(List<string> productCode, List<string> batchCode);

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

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <param name="typeOrder">The type order.</param>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenByTypeOrder(string typeOrder);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryBySaleOrder(List<int> ordersId);

        /// <summary>
        /// Get the delivery orders headers.
        /// </summary>
        /// <param name="docuNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliverModel>> GetDeliveryModelByDocNum(List<int> docuNums);

        /// <summary>
        /// Get the delivery orders headers.
        /// </summary>
        /// <param name="docuNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DetallePedidoModel>> GetDetailByDocNum(List<int> docuNums);

        /// <summary>
        /// Gets the invoiceHeader by doc num.
        /// </summary>
        /// <param name="docNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderByInvoiceId(List<int> docNums);

        /// <summary>
        /// Gets the invoice detail by docEntry.
        /// </summary>
        /// <param name="docEntry">the doc entries.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByDocEntry(List<int> docEntry);

        /// <summary>
        /// Gets the invoice header by docnum.
        /// </summary>
        /// <param name="docNums">the docnum</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocNum(List<int> docNums);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryByDocEntry(List<int> ordersId);

        /// <summary>
        /// Gets te clients by the id.
        /// </summary>
        /// <param name="clientId">the client id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<ClientCatalogModel>> GetClientsById(List<string> clientId);

        /// <summary>
        /// Gets the ids.
        /// </summary>
        /// <param name="ids">the ids.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<Repartidores>> GetDeliveryCompanyById(List<short> ids);

        /// <summary>
        /// Gets the deliveries by invoice.
        /// </summary>
        /// <param name="invoices">the invoices.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryByInvoiceId(List<int?> invoices);

        /// <summary>
        /// Gets all line products.
        /// </summary>
        /// <returns>the data.</returns>
        Task<IEnumerable<ProductoModel>> GetAllLineProducts();

        /// <summary>
        /// gets the details by date.
        /// </summary>
        /// <param name="initDate">the init date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DetallePedidoModel>> GetDetailsbyDocDate(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<ProductoModel>> GetProductByIds(List<string> itemCode);

        /// <summary>
        /// Gets the delivery parties.
        /// </summary>
        /// <returns>the data.</returns>
        Task<IEnumerable<Repartidores>> GetDeliveryCompanies();

        /// <summary>
        /// Get Batches by products.
        /// </summary>
        /// <param name="productsIds">the products.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<Batches>> GetBatchesByProdcuts(List<string> productsIds);

        /// <summary>
        /// Get the invoices by update date.
        /// </summary>
        /// <param name="date">the date to look.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceByUpdateDate(DateTime date);

        /// <summary>
        /// Gets the deliveries by date.
        /// </summary>
        /// <param name="initDate">the init date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliverModel>> GetDeliveryByDocDate(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Gets the deliveries by date.
        /// </summary>
        /// <param name="initDate">the init date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocDate(DateTime initDate, DateTime endDate);
    }
}
