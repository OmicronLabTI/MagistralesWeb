// <summary>
// <copyright file="SapAlmacenController.cs" company="Axity">
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
    using Serilog;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SapAlmacenController : ControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        private readonly ISapAlmacenFacade sapFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenController"/> class.
        /// </summary>
        /// <param name="sapFacade">the sap facade.</param>
        /// <param name="logger">the logger factory.</param>
        public SapAlmacenController(ISapAlmacenFacade sapFacade, ILogger logger)
        {
            this.sapFacade = sapFacade ?? throw new ArgumentNullException(nameof(sapFacade));
            this.logger = logger;
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/orders")]
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapFacade.GetOrders(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Makes the ping.
        /// </summary>
        /// <returns>return the pong.</returns>
        [Route("/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return this.Ok("Pong");
        }
    }
}
