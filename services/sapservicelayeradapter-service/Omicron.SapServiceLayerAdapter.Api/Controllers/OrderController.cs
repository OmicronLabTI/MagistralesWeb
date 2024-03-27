// <summary>
// <copyright file="OrderController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices;
using Omicron.SapServiceLayerAdapter.Common.DTOs.Orders;

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
{
    /// <summary>
    /// OrdersController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderFacade ordersFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="ordersFacade">Orders Facade.</param>
        public OrderController(IOrderFacade ordersFacade)
            => this.ordersFacade = ordersFacade ?? throw new ArgumentNullException(nameof(ordersFacade));

        /// <summary>
        /// Close sample orders.
        /// </summary>
        /// <param name="sampleOrders">Sample orders.</param>
        /// <returns>Result.</returns>
        [HttpPost("/close/sampleorders")]
        public async Task<IActionResult> CloseSampleOrders([FromBody] List<CloseSampleOrderDto> sampleOrders)
            => this.Ok(await this.ordersFacade.CloseSampleOrders(sampleOrders));

        /// <summary>
        /// Method to get the last generated order.
        /// </summary>
        /// <returns>Last Order.</returns>
        [Route("/lastgeneratedorder")]
        [HttpGet]
        public async Task<IActionResult> GetSaleProductByUser()
            => this.Ok(await this.ordersFacade.GetLastGeneratedOrder());

        /// <summary>
        /// Method Ping.
        /// </summary>
        /// <returns>Pong.</returns>
        [Route("ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return this.Ok("Pong");
        }
    }
}
