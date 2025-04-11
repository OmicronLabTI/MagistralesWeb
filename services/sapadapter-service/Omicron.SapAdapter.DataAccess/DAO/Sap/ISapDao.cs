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
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<List<OrderModel>> GetOrdersByIdJoinDoctor(List<int> pedidoID);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetCountDxpOrdersByIds(List<string> dxpIds);

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="pedidoId">PedidoID</param>
        /// <returns>the details.</returns>
        Task<IEnumerable<CompleteDetailOrderModel>> GetAllDetails(List<int?> pedidoId);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int init, int end);

        /// <summary>
        /// REturns the order by docnum dxp.
        /// </summary>
        /// <param name="docNumDxp">the docnum.</param>
        /// <returns>the data.</returns>
        Task<List<CompleteOrderModel>> GetAllOrdersByDocNumDxp(string docNumDxp);

        /// <summary>
        /// REturns the order by docnum dxp.
        /// </summary>
        /// <param name="docNumDxp">the docnum.</param>
        /// <returns>the data.</returns>
        Task<List<OrderModel>> GetOrdersByDocNumDxp(List<string> docNumDxp);

        /// <summary>
        /// gets the orders by product and item.
        /// </summary>
        /// <param name="pedidoId">the product id.</param>
        /// <param name="productId">the product id.</param>
        /// <param name="productId">the datasources.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetProdOrderByOrderProduct(int pedidoId, string productId, List<string> datasources);

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
        /// <param name="warehouse">the warehouse.</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(string value, string warehouse);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <param name="warehouse">The warehouse</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(List<string> value, string warehouse);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <param name="warehouse">The warehouse.</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsDescription(string value, string warehouse);

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="pedidoId">the pedido id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DetallePedidoModel>> GetPedidoByIdJoinProduct(int pedidoId);

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
        /// <param name="datasources">the datasources.</param>
        /// <returns>the data.</returns>
        Task<int> GetlLastIsolatedProductionOrderId(string productId, string uniqueId, List<string> datasources);

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
        /// GetAllOrdersForAlmacen.
        /// </summary>
        /// <param name="startDate">startDate.</param>
        /// <param name="endDate">endDate.</param>
        /// <returns>the attachaments.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacen(DateTime startDate, DateTime endDate);

        /// <summary>
        /// GetAllOrdersForAlmacenDxp.
        /// </summary>
        /// <param name="startDate">startDate.</param>
        /// <param name="endDate">endDate.</param>
        /// <returns>the attachaments.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenDxp(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenByListIds(List<int> saleordersIds);

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
        /// <param name="orderToLook">The orders to look.</param>
        /// <param name="needOnlyDxp">needOnlyDxp.</param>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteAlmacenOrderModel>> GetAllOrdersForAlmacenByTypeOrder(string typeOrder, List<int> orderToLook, bool needOnlyDxp);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailBySaleOrder(List<int> ordersId);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetCompleteDeliveryWithDetailBySaleOrder(List<int> ordersId);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailBySaleOrderJoinProduct(List<int> ordersId);

        /// <summary>
        /// Get the delivery orders headers.
        /// </summary>
        /// <param name="docuNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliverModel>> GetDeliveryModelByDocNum(List<int> docuNums);

        /// <summary>
        /// Get the delivery orders headers join with doctor.
        /// </summary>
        /// <param name="docuNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<List<DeliverModel>> GetDeliveryModelByDocNumJoinDoctor(List<int> docuNums);

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
        /// Gets the invoiceHeader by doc num.
        /// </summary>
        /// <param name="docNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderByInvoiceIdJoinDoctor(List<int> docNums);

        /// <summary>
        /// Gets the invoice detail by docEntry.
        /// </summary>
        /// <param name="docEntry">the doc entries.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByDocEntry(List<int> docEntry);

        /// <summary>
        /// Gets the invoiceHeader by doc num.
        /// </summary>
        /// <param name="docNums">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteInvoiceDetailModel>> GetInvoiceHeaderDetailByInvoiceIdJoinDoctor(List<int> docNums);

        /// <summary>
        /// Gets the invoice detail by docEntry.
        /// </summary>
        /// <param name="docEntry">the doc entries.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByDocEntryJoinProduct(List<int> docEntry);

        /// <summary>
        /// Gets the invoice header by docnum.
        /// </summary>
        /// <param name="docNum">the docnum</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocNum(List<int> docNum);

        /// <summary>
        /// Gets the invoice header by docnum.
        /// </summary>
        /// <param name="docNum">the docnum</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocNumJoinDoctor(List<int> docNum);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailByDocEntry(List<int> ordersId);

        /// <summary>
        /// Gets the deliveries by the sale order.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailByDocEntryJoinProduct(List<int> ordersId);

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
        Task<IEnumerable<DeliveyJoinOrderModel>> GetDeliveryByInvoiceId(List<int?> invoices);

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
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceByDocDate(DateTime date);

        /// <summary>
        /// Gets the deliveries by date.
        /// </summary>
        /// <param name="initDate">the init date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliverModel>> GetDeliveryByDocDateJoinDoctor(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Gets the deliveries by date.
        /// </summary>
        /// <param name="initDate">the init date.</param>
        /// <param name="endDate">the end date.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeadersByDocDateJoinDoctor(DateTime initDate, DateTime endDate);

        /// <summary>
        /// gets the invoice details by delivery id.
        /// </summary>
        /// <param name="baseEntry">the base entry.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceDetailModel>> GetInvoiceDetailByBaseEntryJoinProduct(List<int> baseEntry);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByIds(List<int> ids);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByIdsJoinProduct(List<int> ids);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersWIthDetailByDocNumDxpJoinProduct(string DocNumDxp);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetCountOrdersWIthDetailByDocNumDxpJoinProduct(string DocNumDxp);

        /// <summary>
        /// Gets the order by init date.
        /// </summary>
        Task<IEnumerable<OrderModel>> GetOrderModelByDocDateJoinDoctor(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Gets the detail for the order id.
        /// </summary>
        /// <param name="orderIds">the data.</param>
        /// <returns>the detail.</returns>
        Task<List<CompleteRecepcionPedidoDetailModel>> GetSapOrderDetailForAlmacenRecepcionById(List<int> orderIds);

        /// <summary>
        /// Gets the deatils for the delivry.
        /// </summary>
        /// <param name="delveryId">the ids to look.</param>
        /// <returns>the data.</returns>
        Task<List<CompleteDeliveryDetailModel>> GetDeliveryDetailForDeliveryById(List<int> delveryId);

        /// <summary>
        /// Get the Get Complete Raw Material Request By Filters.
        /// </summary>
        /// <param name="initDate">Init date.</param>
        /// <param name="endDate">End date.</param>
        /// <param name="userId">User id.</param>
        /// <returns>WRaw material request information.</returns>
        Task<List<CompleteRawMaterialRequestModel>> GetCompleteRawMaterialRequestByFilters(DateTime initDate, DateTime endDate, string userId);

        /// <summary>
        /// Gets the doctor by id transaction.
        /// </summary>
        /// <param name="idtransaction"> id transaction.</param>
        /// <returns> information.</returns>
        Task<OrderModel> GetOrderInformationByTransaction(string idtransaction);
        /// <summary>
        /// Gets the doctor by cardcode.
        /// </summary>
        /// <param name="cardCode"> parameter cardcode. </param>
        /// <returns> information.</returns>
        Task<List<ClientCatalogModel>> GetClientCatalogCardCode(List<string> cardCode);

        /// <summary>
        /// GetInvoiceHeaderJoinDoctorByDocNumsForSearchs
        /// </summary>
        /// <param name="docNums">docNums.</param>
        /// <returns></returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderJoinDoctorByDocNumsForSearchs(List<int> docNums);

        /// <summary>
        /// GetInvoiceHeaderJoinDoctorByInvoiceIdOrDatesRange
        /// </summary>
        /// <param name="startDate">startDate.</param>
        /// <param name="endDate">endDate</param>
        /// <returns></returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetInvoiceHeaderJoinDoctorByDatesRangesForSearchs(DateTime startDate, DateTime endDate);

        /// <summary>
        /// GetDeliveryDetailJoinProductByInvoicesIds.
        /// </summary>
        /// <param name="invoicesIds">the orders id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DeliveryDetailModel>> GetDeliveryDetailJoinProductByInvoicesIds(List<int> invoicesIds);

        /// <summary>
        /// GetClosedInvoicesByDocNum.
        /// </summary>
        /// <param name="docNums">docNums.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<InvoiceHeaderModel>> GetClosedInvoicesByDocNum(List<int> docNums);
    }
}
