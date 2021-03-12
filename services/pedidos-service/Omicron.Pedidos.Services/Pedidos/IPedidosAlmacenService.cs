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
        /// Gets the pdf for the invoice.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="invoiceIds">the invoiced ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreatePdf(string type, List<int> invoiceIds);
    }
}
