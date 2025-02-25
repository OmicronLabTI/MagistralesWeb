// <summary>
// <copyright file="IPedidosAlmacenFacade.cs" company="Axity">
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
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// interfaces for the pedidos.
    /// </summary>
    public interface IPedidosAlmacenFacade
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForAlmacen();

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="idsToLook">The ids to llok.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForAlmacen(List<int> idsToLook);

        /// <summary>
        /// Updates the user orders.
        /// </summary>
        /// <param name="userOrders">The orders.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateUserOrders(List<UserOrderDto> userOrders);

        /// <summary>
        /// Get the orders for delivery.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForDelivery(Dictionary<string, string> parameters);

        /// <summary>
        /// Get the orders for delivery.
        /// </summary>
        /// <param name="deliveryIds">The deliveryIds.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForDelivery(List<int> deliveryIds);

        /// <summary>
        /// Gets the deliveries for the invoices.
        /// </summary>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForInvoice();

        /// <summary>
        /// Gets the order for the packages by type.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForPackages(Dictionary<string, string> parameters);

        /// <summary>
        /// Updates whe the package is sent.
        /// </summary>
        /// <param name="usersToUpdate">the data to update.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> UpdateSentOrders(List<UserOrderDto> usersToUpdate);

        /// <summary>
        /// Gets the totals for graphs.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetAlmacenGraphData(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the orders by delivery id.
        /// </summary>
        /// <param name="deliveryIds">the delivery ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrderByDeliveryOrder(List<int> deliveryIds);

        /// <summary>
        /// Gets the orders by invoices id.
        /// </summary>
        /// <param name="invoices">the invoices ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrderByInvoiceId(List<int> invoices);

        /// <summary>
        /// Gets the invoices pdf.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="invoiceIds">the invoices.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreatePdf(string type, List<int> invoiceIds);

        /// <summary>
        /// The cancels the delivery.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="deliveryIds">the ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CancelDelivery(string type, CancelDeliveryPedidoCompleteDto deliveryIds);

        /// <summary>
        /// Cleans the invoices.
        /// </summary>
        /// <param name="invoiceIds">the invoice ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> CleanInvoices(List<int> invoiceIds);

        /// <summary>
        /// Looks for the data in invoices, delivery and sale.
        /// </summary>
        /// <param name="docNums">the docunms.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> AdvanceLook(List<int> docNums);

        /// <summary>
        /// Get the user orders by invoices id.
        /// </summary>
        /// <param name="invoicesIds">the ids.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetUserOrdersByInvoicesIds(List<int> invoicesIds);

        /// <summary>
        /// Get Orders For Almacen By Range Dates.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForAlmacenByRangeDates(Dictionary<string, string> parameters);

        /// <summary>
        /// GetOrdersForInvoiceByRangeDates.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrdersForInvoiceByRangeDates(Dictionary<string, string> parameters);
    }
}
