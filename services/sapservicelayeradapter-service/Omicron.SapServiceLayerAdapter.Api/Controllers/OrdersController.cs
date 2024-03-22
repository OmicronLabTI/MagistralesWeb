// <summary>
// <copyright file="OrdersController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
{
    /// <summary>
    /// OrdersController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersFacade ordersFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="ordersFacade">Orders Facade.</param>
        public OrdersController(IOrdersFacade ordersFacade)
            => this.ordersFacade = ordersFacade ?? throw new ArgumentNullException(nameof(ordersFacade));

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
