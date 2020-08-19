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
    using Omicron.Pedidos.Dtos.Models;
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
        /// <param name="orderDto">the id of the orders.</param>
        /// <returns>the result.</returns>
        [Route("/processOrders")]
        [HttpPost]
        public async Task<IActionResult> ProcessOrders(ProcessOrderDto orderDto)
        {
            var response = await this.pedidoFacade.ProcessOrders(orderDto);
            return this.Ok(response);
        }

        /// <summary>
        /// planificar by order.
        /// </summary>
        /// <param name="processByOrder">process by order.</param>
        /// <returns>the data to return.</returns>
        [Route("/processByOrder")]
        [HttpPost]
        public async Task<IActionResult> ProcessByOrder(ProcessByOrderDto processByOrder)
        {
            var response = await this.pedidoFacade.ProcessByOrder(processByOrder);
            return this.Ok(response);
        }

        /// <summary>
        /// Get the user order by Pedido id.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <returns>the data.</returns>
        [Route("/getUserOrder/salesOrder")]
        [HttpPost]
        public async Task<IActionResult> GetUserOrderBySalesOrder(List<int> listIds)
        {
            var response = await this.pedidoFacade.GetUserOrderBySalesOrder(listIds);
            return this.Ok(response);
        }

        /// <summary>
        /// gets the user order for Ipad cards.
        /// </summary>
        /// <param name="userId">the ids.</param>
        /// <returns>the data.</returns>
        [Route("/qfbOrders/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetQfbOrders(string userId)
        {
            var response = await this.pedidoFacade.GetFabOrderByUserID(userId);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets all the user orders by user ids.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        [Route("/qfbOrders")]
        [HttpPost]
        public async Task<IActionResult> GetAllQfbOrders(List<string> listIds)
        {
            var response = await this.pedidoFacade.GetUserOrdersByUserId(listIds);
            return this.Ok(response);
        }

        /// <summary>
        /// Asignacion manual.
        /// </summary>
        /// <param name="manualAssign">the assign model.</param>
        /// <returns>la asignacion manual.</returns>
        [Route("/asignar/manual")]
        [HttpPost]
        public async Task<IActionResult> AsignarManual(ManualAssignDto manualAssign)
        {
            var response = await this.pedidoFacade.AssignHeader(manualAssign);
            return this.Ok(response);
        }

        /// <summary>
        /// Assignacion automatica.
        /// </summary>
        /// <param name="automaticAssing">the object to assign.</param>
        /// <returns>the data.</returns>
        [Route("/asignar/automatico")]
        [HttpPost]
        public async Task<IActionResult> AssignarAutomatico(AutomaticAssingDto automaticAssing)
        {
            var response = await this.pedidoFacade.AutomaticAssign(automaticAssing);
            return this.Ok(response);
        }

        /// <summary>
        /// Asignacion manual.
        /// </summary>
        /// <param name="updateFormula">the assign model.</param>
        /// <returns>la asignacion manual.</returns>
        [Route("/formula")]
        [HttpPut]
        public async Task<IActionResult> UpdateFormula(UpdateFormulaDto updateFormula)
        {
            var response = await this.pedidoFacade.UpdateComponents(updateFormula);
            return this.Ok(response);
        }

        /// <summary>
        /// the update status.
        /// </summary>
        /// <param name="updateStatus">the status object.</param>
        /// <returns>the order.</returns>
        [Route("/status/fabOrder")]
        [HttpPut]
        public async Task<IActionResult> UpdateStatusOrder(List<UpdateStatusOrderDto> updateStatus)
        {
            var response = await this.pedidoFacade.UpdateStatusOrder(updateStatus);
            return this.Ok(response);
        }

        /// <summary>
        /// connects the DI api.
        /// </summary>
        /// <returns>the connection.</returns>
        [Route("/connectDiApi")]
        [HttpGet]
        public async Task<IActionResult> ConnectDiApi()
        {
            var response = await this.pedidoFacade.ConnectDiApi();
            return this.Ok(response);
        }
    }
}
