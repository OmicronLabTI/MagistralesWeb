// <summary>
// <copyright file="SapController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Omicron.SapAdapter.Facade.Sap;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SapController : ControllerBase
    {
        private readonly ISapFacade sapFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapController"/> class.
        /// </summary>
        /// <param name="sapFacade">the sap facade.</param>
        public SapController(ISapFacade sapFacade)
        {
            this.sapFacade = sapFacade ?? throw new ArgumentNullException(nameof(sapFacade));
        }

        /// <summary>
        /// Method to get all orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>List of orders.</returns>
        [Route("/orders")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapFacade.GetOrders(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the detail of the order.
        /// </summary>
        /// <param name="docEntry">the detail id.</param>
        /// <returns>the details.</returns>
        [Route("/detail/{docEntry}")]
        [HttpGet]
        public async Task<IActionResult> GetDetails(string docEntry)
        {
            var result = await this.sapFacade.GetDetallePedidos(docEntry);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene las formulas de la orden de fabricacion.
        /// </summary>
        /// <param name="ordenId">the order id.</param>
        /// <returns>the object.</returns>
        [Route("/formula/{ordenId}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderFormula(int ordenId)
        {
            var result = await this.sapFacade.GetOrderFormula(ordenId);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene las ordenes de fabricacion con su pedido.
        /// </summary>
        /// <param name="pedidosId">the ids.</param>
        /// <returns>the result.</returns>
        [Route("/getDetails")]
        [HttpPost]
        public async Task<IActionResult> GetOrderWithDetails(List<int> pedidosId)
        {
            var result = await this.sapFacade.GetPedidoWithDetail(pedidosId);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene la orden de fabricacion en base al orderitem-productid para el insert de ordenes de fabricacion.
        /// </summary>
        /// <param name="pedidosId">the ids.</param>
        /// <returns>the result.</returns>
        [Route("/getProductionOrderItem")]
        [HttpPost]
        public async Task<IActionResult> GetProdOrderByOrderItem(List<string> pedidosId)
        {
            var result = await this.sapFacade.GetProdOrderByOrderItem(pedidosId);
            return this.Ok(result);
        }
    }
}
