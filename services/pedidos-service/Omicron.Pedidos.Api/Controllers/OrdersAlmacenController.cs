// <summary>
// <copyright file="OrdersAlmacenController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Facade.Pedidos;
    using Omicron.Pedidos.Resources.Enums;

    /// <summary>
    /// the class for pedidos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersAlmacenController : ControllerBase
    {
        private readonly IPedidoFacade pedidoFacade;

        private readonly IQrFacade qrsFacade;

        private readonly IPedidosAlmacenFacade pedidosAlmacenFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersAlmacenController"/> class.
        /// </summary>
        /// <param name="pedidoFacade">the pedido facade.</param>
        /// <param name="qrsFacade">The qr Facade.</param>
        /// <param name="pedidosAlmacen">The pedidos almacen facade.</param>
        public OrdersAlmacenController(IPedidoFacade pedidoFacade, IQrFacade qrsFacade, IPedidosAlmacenFacade pedidosAlmacen)
        {
            this.pedidoFacade = pedidoFacade ?? throw new ArgumentNullException(nameof(pedidoFacade));
            this.qrsFacade = qrsFacade ?? throw new ArgumentException(nameof(qrsFacade));
            this.pedidosAlmacenFacade = pedidosAlmacen ?? throw new ArgumentNullException(nameof(pedidosAlmacen));
        }

        /// <summary>
        /// Creates the pdf for the sale order.
        /// </summary>
        /// <param name="orderIds">The orders ids.</param>
        /// <returns>the data.</returns>
        [Route("/qr/magistral")]
        [HttpPost]
        public async Task<IActionResult> CreateQrMagistral(List<int> orderIds)
        {
            var response = await this.qrsFacade.CreateMagistralQr(orderIds);
            return this.Ok(response);
        }

        /// <summary>
        /// Creates the pdf for the sale order.
        /// </summary>
        /// <param name="orderIds">The orders ids.</param>
        /// <returns>the data.</returns>
        [Route("/qr/remision")]
        [HttpPost]
        public async Task<IActionResult> CreateQrRemision(List<int> orderIds)
        {
            var response = await this.qrsFacade.CreateRemisionQr(orderIds);
            return this.Ok(response);
        }

        /// <summary>
        /// Creates the pdf for the sale order.
        /// </summary>
        /// <param name="orderIds">The orders ids.</param>
        /// <returns>the data.</returns>
        [Route("/qr/factura")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoiceQr(List<int> orderIds)
        {
            var response = await this.qrsFacade.CreateInvoiceQr(orderIds);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Route("/userorders/almacen")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersForAlmacen()
        {
            var response = await this.pedidosAlmacenFacade.GetOrdersForAlmacen();
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="listUser">The list of users.</param>
        /// <returns>the data.</returns>
        [Route("/userorders")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserOrders(List<UserOrderDto> listUser)
        {
            var response = await this.pedidosAlmacenFacade.UpdateUserOrders(listUser);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Route("/userorders/delivery")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersForDelivery()
        {
            var response = await this.pedidosAlmacenFacade.GetOrdersForDelivery();
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Route("/userorders/invoice")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersForInvoice()
        {
            var response = await this.pedidosAlmacenFacade.GetOrdersForInvoice();
            return this.Ok(response);
        }

        /// <summary>
        /// Get the user order by Pedido id.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <returns>the data.</returns>
        [Route("/getUserOrder/deliveryOrders")]
        [HttpPost]
        public async Task<IActionResult> GetUserOrderByDeliveryOrder(List<int> listIds)
        {
            var response = await this.pedidosAlmacenFacade.GetUserOrderByDeliveryOrder(listIds);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        [Route("/userorders/packages")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersForPackages([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.pedidosAlmacenFacade.GetOrdersForPackages(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="listUser">The type of packages.</param>
        /// <returns>the data.</returns>
        [Route("/sent/orders")]
        [HttpPut]
        public async Task<IActionResult> UpdateSentOrders(List<UserOrderDto> listUser)
        {
            var response = await this.pedidosAlmacenFacade.UpdateSentOrders(listUser);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/graph")]
        [HttpGet]
        public async Task<IActionResult> GetAlmacenGraphData([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.pedidosAlmacenFacade.GetAlmacenGraphData(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Creates the pdf for the sale order.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="ordersId">the orders.</param>
        /// <returns>the data.</returns>
        [Route("/{type}/pdf")]
        [HttpPost]
        public async Task<IActionResult> CreateinvoicePdf(string type, List<int> ordersId)
        {
            var response = await this.pedidosAlmacenFacade.CreatePdf(type, ordersId);
            return this.Ok(response);
        }

        /// <summary>
        /// Creates the pdf for the sale order.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="deliveryId">the orders.</param>
        /// <returns>the data.</returns>
        [Route("/cancel/{type}/delivery")]
        [HttpPost]
        public async Task<IActionResult> CancelDelivery(string type, List<int> deliveryId)
        {
            var response = await this.pedidosAlmacenFacade.CancelDelivery(deliveryId);
            return this.Ok(response);
        }
    }
}
