// <summary>
// <copyright file="CatalogController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Omicron.Catalogos.Facade.Catalogs;

    /// <summary>
    /// The controller for the catalog.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogFacade catalogFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogController"/> class.
        /// </summary>
        /// <param name="catalogFacade">the controller.</param>
        public CatalogController(ICatalogFacade catalogFacade)
        {
            this.catalogFacade = catalogFacade ?? throw new ArgumentNullException(nameof(catalogFacade));
        }

        /// <summary>
        /// Get for all the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        [Route("/getroles")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await this.catalogFacade.GetRoles();
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the value from params where contains field.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        [Route("/params/contains/field")]
        [HttpGet]
        public async Task<IActionResult> GetParamsContainsByField([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.catalogFacade.GetParamsContains(parameters);
            return this.Ok(response);
        }
    }
}
