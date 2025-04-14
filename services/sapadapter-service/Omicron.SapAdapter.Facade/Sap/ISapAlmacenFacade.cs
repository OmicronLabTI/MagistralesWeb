// <summary>
// <copyright file="ISapAlmacenFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Facade.Sap
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// Interface for sapAlmacen.
    /// </summary>
    public interface ISapAlmacenFacade
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the order detail.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersDetails(int orderId);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedDataMagistral(string code);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="code">the code scanned.</param>
        /// <param name="orderId">the order Id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedDataLinea(string code, int orderId);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedDataRemision(string code);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedDataRemisionMg(string code);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedDataRemisionLn(string code);

        /// <summary>
        /// Gets the data for the scanned qr or bar code.
        /// </summary>
        /// <param name="code">the code scanned.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetScannedDataFactura(string code);

        /// <summary>
        /// Gets the data for order detail.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDetailOrder(int orderId);

        /// <summary>
        /// Gets the products with codebars.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetProductsWithCodeBars();

        /// <summary>
        /// Gets all the details.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetCompleteDetail(int orderId);

        /// <summary>
        /// Gets all the orders.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersByIds(List<int> ordersId);

        /// <summary>
        /// Gets the orders from delivery.
        /// </summary>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDeliveryBySaleOrderId(List<int> ordersId);

        /// <summary>
        /// Gets the delivery orders.
        /// </summary>
        /// <param name="parameters">the parameters to look.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDelivery(Dictionary<string, string> parameters);

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="deliveryId">the delivery ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersDeliveryDetail(int deliveryId);

        /// <summary>
        /// Gets the products of a specific delivery.
        /// </summary>
        /// <param name="saleId">the ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetProductsDelivery(string saleId);

        /// <summary>
        /// Gets the invoices.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetInvoice(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the GetInvoiceByFilters.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetInvoiceByFilters(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the invoices.
        /// </summary>
        /// <param name="invoice">the invoice to look for.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetInvoiceDetail(int invoice);

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="invoiceId">The invoice Id.</param>
        /// <param name="type">The type.</param>
        /// <param name="deliveriesIds">The deliveriesIds.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetInvoiceProducts(int invoiceId, string type, List<int> deliveriesIds);

        /// <summary>
        /// Gets the headers from SAP.
        /// </summary>
        /// <param name="dataToLook">the data to look.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetInvoiceHeader(InvoicePackageSapLookDto dataToLook);

        /// <summary>
        /// Gets the ids for delivry and invoice by sales id.
        /// </summary>
        /// <param name="salesid">the sales id separated by commas.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetSapIds(List<int> salesid);

        /// <summary>
        /// Retrieves the ids for the dates.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> AlmacenGraphCount(Dictionary<string, string> parameters);

        /// <summary>
        /// Get the delivery parties.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDeliveryParties();

        /// <summary>
        /// looks the delivery.
        /// </summary>
        /// <param name="deliveryIds">the ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetDeliveries(List<int> deliveryIds);

        /// <summary>
        /// Gets the cancelled iinvoices.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetCancelledInvoices(int days);

        /// <summary>
        /// Get the advance lookup.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> AdvanceLookUp(Dictionary<string, string> parameters);

        /// <summary>
        /// Get the Almacen Orders By Dxp Order.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> SearchAlmacenOrdersByDxpId(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the detail for an dxp order.
        /// </summary>
        /// <param name="details">the details.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> SearchAlmacenOrdersDetailsByDxpId(DoctorOrdersSearchDeatilDto details);

        /// <summary>
        /// Get the Almacen Orders By Doctor.
        /// </summary>
        /// <param name="saleorderid">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrderdetail(int saleorderid);

        /// <summary>
        /// Gets all the invoices.
        /// </summary>
        /// <param name="invoicesIds">the invoices id.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetInvoicesByIds(List<int> invoicesIds);
    }
}
