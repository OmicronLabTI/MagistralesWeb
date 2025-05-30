// <summary>
// <copyright file="IPedidosAlmacenService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// interface for the almacen pedidos.
    /// </summary>
    public interface IPedidosAlmacenService
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForAlmacen();

        /// <summary>
        /// GetOrdersForAlmacenByOrdersId.
        /// </summary>
        /// <param name="ordersId">Orders Id.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForAlmacenByOrdersId(List<int> ordersId);

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="idsToLook">ids to look.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForAlmacen(List<int> idsToLook);

        /// <summary>
        /// Updates user orders.
        /// </summary>
        /// <param name="listOrders">the list of orders.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateUserOrders(List<UserOrderModel> listOrders);

        /// <summary>
        /// Gets the orders for deliveru.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForDelivery(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the orders for deliveru.
        /// </summary>
        /// <param name="deliveryIds">The delivery ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForDelivery(List<int> deliveryIds);

        /// <summary>
        /// Gets the orders for the invoice.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForInvoice();

        /// <summary>
        /// Return the orders for packages.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForPackages(Dictionary<string, string> parameters);

        /// <summary>
        /// The list of users to update.
        /// </summary>
        /// <param name="userToUpdate">the list to update.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateSentOrders(List<UserOrderModel> userToUpdate);

        /// <summary>
        /// Gets the data for the graph.
        /// </summary>
        /// <param name="parameters">the dict.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetAlmacenGraphData(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the data by delivery id.
        /// </summary>
        /// <param name="deliveryIds">the deliveries.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrderByDeliveryOrder(List<int> deliveryIds);

        /// <summary>
        /// Gets the orders by invoices id.
        /// </summary>
        /// <param name="invoices">the invoices ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrderByInvoiceId(List<int> invoices);

        /// <summary>
        /// Gets the pdf for the invoice.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="invoiceIds">the invoiced ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreatePdf(string type, List<int> invoiceIds);

        /// <summary>
        /// Looks by docnums.
        /// </summary>
        /// <param name="docNum">the doc nums.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> AdvanceLook(List<int> docNum);

        /// <summary>
        /// Get the user orders by invoices id.
        /// </summary>
        /// <param name="invoicesIds">the ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrdersByInvoicesIds(List<int> invoicesIds);

        /// <summary>
        /// Get Orders For Almacen By Range Dates.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForAlmacenByRangeDates(Dictionary<string, string> parameters);

        /// <summary>
        /// GetOrdersForInvoiceByRangeDates.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrdersForInvoiceByRangeDates(Dictionary<string, string> parameters);

        /// <summary>
        /// GetUserOrdersForInvoiceByDeliveryIds.
        /// </summary>
        /// <param name="deliveryIds">deliveryIds.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetUserOrdersForInvoiceByDeliveryIds(List<int> deliveryIds);

        /// <summary>
        /// unassign the batch from a product.
        /// </summary>
        /// <param name="dto">the order id.</param>
        /// <returns>the result.</returns>
        Task<ResultModel> GetInfoPiecesByOrderId(InvoiceProductsDto dto);
    }
}
