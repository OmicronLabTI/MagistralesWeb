// <summary>
// <copyright file="PedidosDpxController.cs" company="Axity">
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

    /// <summary>
    /// the class for pedidos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosDpxController : ControllerBase
    {
        private readonly IPedidosDxpFacade pedidosDxpFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosDpxController"/> class.
        /// </summary>
        /// <param name="pedidosDxpFacade">the pedido facade.</param>
        public PedidosDpxController(IPedidosDxpFacade pedidosDxpFacade)
        {
            this.pedidosDxpFacade = pedidosDxpFacade ?? throw new ArgumentNullException(nameof(pedidosDxpFacade));
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="orderDto">the id of the orders.</param>
        /// <returns>the result.</returns>
        [Route("/dxp/sent/orders")]
        [HttpPost]
        public async Task<IActionResult> GetDeliveredPayments(List<int> orderDto)
        {
            var response = await this.pedidosDxpFacade.GetDeliveredPayments(orderDto);
            return this.Ok(response);
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="orders">the orders to search.</param>
        /// <returns>the result.</returns>
        [Route("/dxp/orders/status")]
        [HttpPost]
        public async Task<IActionResult> GetOrdersHeaderStatus(List<string> orders)
        {
            var response = await this.pedidosDxpFacade.GetOrdersHeaderStatus(orders);
            return this.Ok(response);
        }
    }
}
