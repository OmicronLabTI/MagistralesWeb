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
    }
}
