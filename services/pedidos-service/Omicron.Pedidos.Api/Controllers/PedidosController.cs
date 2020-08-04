// <summary>
// <copyright file="PedidosController.cs" company="Axity">
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
    using Omicron.Pedidos.Facade.Pedidos;

    /// <summary>
    /// the class for pedidos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoFacade pedidoFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosController"/> class.
        /// </summary>
        /// <param name="pedidoFacade">the pedido facade.</param>
        public PedidosController(IPedidoFacade pedidoFacade)
        {
            this.pedidoFacade = pedidoFacade ?? throw new ArgumentNullException(nameof(pedidoFacade));
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="pedidosIds">the id of the orders.</param>
        /// <returns>the result.</returns>
        [Route("/processOrders")]
        [HttpPost]
        public async Task<IActionResult> ProcessOrders(List<int> pedidosIds)
        {
            var response = await this.pedidoFacade.ProcessOrders(pedidosIds);
            return this.Ok(response);
        }
    }
}
