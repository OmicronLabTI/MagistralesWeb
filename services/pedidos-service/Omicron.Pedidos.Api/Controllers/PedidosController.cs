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
    using Omicron.Pedidos.Resources.Enums;

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
        /// Get the user order by fabrication order id.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <returns>the data.</returns>
        [Route("/getUserOrder/fabOrder")]
        [HttpPost]
        public async Task<IActionResult> GetUserOrderByFabOrder(List<int> listIds)
        {
            var response = await this.pedidoFacade.GetUserOrderByFabOrder(listIds);
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
        /// Change order status to cancel.
        /// </summary>
        /// <param name="cancelOrders">Update orders info.</param>
        /// <returns>Order with updated info.</returns>
        [Route("/salesOrder/cancel")]
        [HttpPut]
        public async Task<IActionResult> CancelOrder(List<OrderIdDto> cancelOrders)
        {
            var response = await this.pedidoFacade.CancelOrder(cancelOrders);
            return this.Ok(response);
        }

        /// <summary>
        /// Change order status to finish.
        /// </summary>
        /// <param name="finishOrders">Orders to finish.</param>
        /// <returns>Order with updated info.</returns>
        [Route("/salesOrder/finish")]
        [HttpPut]
        public async Task<IActionResult> CloseSalesOrders(List<OrderIdDto> finishOrders)
        {
            var response = await this.pedidoFacade.CloseSalesOrders(finishOrders);
            return this.Ok(response);
        }

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</param>
        /// <returns>Order with updated info.</returns>
        [Route("/fabOrder/cancel")]
        [HttpPut]
        public async Task<IActionResult> CancelFabOrder(List<OrderIdDto> cancelOrders)
        {
            var response = await this.pedidoFacade.CancelFabOrder(cancelOrders);
            return this.Ok(response);
        }

        /// <summary>
        /// Finish fabrication orders.
        /// </summary>
        /// <param name="finishOrders">Orders to cancel.</param>
        /// <returns>Order with updated info.</returns>
        [Route("/fabOrder/finish")]
        [HttpPut]
        public async Task<IActionResult> CloseFabOrders(List<OrderIdDto> finishOrders)
        {
            var response = await this.pedidoFacade.CloseFabOrders(finishOrders);
            return this.Ok(response);
        }

        /// <summary>
        /// Change fabrication order comments.
        /// </summary>
        /// <param name="updateComments">the order info.</param>
        /// <returns>the order.</returns>
        [Route("/fabOrder/comments")]
        [HttpPut]
        public async Task<IActionResult> UpdateFabOrderComments(List<UpdateOrderCommentsDto> updateComments)
        {
            var response = await this.pedidoFacade.UpdateFabOrderComments(updateComments);
            return this.Ok(response);
        }

        /// <summary>
        /// Update the order signatures.
        /// </summary>
        /// <param name="signatureType">Signature to update.</param>
        /// <param name="orderSignature">Orders signature data.</param>
        /// <returns>Operation result.</returns>
        [Route("/fabOrder/{signatureType:regex(^(logistic|technical)$)}/signature")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrderSignature(string signatureType, UpdateOrderSignatureDto orderSignature)
        {
            ResultDto result = null;
            if (signatureType.Equals("logistic"))
            {
                result = await this.pedidoFacade.UpdateOrderSignature(SignatureTypeEnum.LOGISTICS, orderSignature);
            }
            else if (signatureType.Equals("technical"))
            {
                result = await this.pedidoFacade.UpdateOrderSignature(SignatureTypeEnum.TECHNICAL, orderSignature);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Get order signatures.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Operation result.</returns>
        [Route("/fabOrder/signatures")]
        [HttpPost]
        public async Task<IActionResult> GetOrderSignatures(int productionOrderId)
        {
            ResultDto result = await this.pedidoFacade.GetOrderSignatures(productionOrderId);
            return this.Ok(result);
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

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="assignBatches">Orders to cancel.</param>
        /// <returns>Order with updated info.</returns>
        [Route("/assignBatches")]
        [HttpPut]
        public async Task<IActionResult> UpdateBatches(List<AssignBatchDto> assignBatches)
        {
            var response = await this.pedidoFacade.UpdateBatches(assignBatches);
            return this.Ok(response);
        }

        /// <summary>
        /// Finish the order by the qfb.
        /// </summary>
        /// <param name="orderSignature">Orders to cancel.</param>
        /// <returns>Order with updated info.</returns>
        [Route("/finishOrder")]
        [HttpPost]
        public async Task<IActionResult> FinishOrder(UpdateOrderSignatureDto orderSignature)
        {
            var response = await this.pedidoFacade.FinishOrder(orderSignature);
            return this.Ok(response);
        }

        /// <summary>
        /// Makes the ping.
        /// </summary>
        /// <returns>return the pong.</returns>
        [Route("/ping")]
        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            return this.Ok("Pong");
        }
    }
}
