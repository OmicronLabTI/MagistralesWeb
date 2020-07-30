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
        /// <returns>List of orders.</returns>
        [Route("/orders")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await this.sapFacade.GetOrders();
            return this.Ok(response);
        }
    }
}
