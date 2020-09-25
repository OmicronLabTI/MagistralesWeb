// <summary>
// <copyright file="WarehousesController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Omicron.Warehouses.Dtos.Model;
    using Omicron.Warehouses.Facade.Request;
    using Omicron.Warehouses.Resources.Extensions;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IRequestFacade requestFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehousesController"/> class.
        /// </summary>
        /// <param name="requestFacade">Request Facade.</param>
        public WarehousesController(IRequestFacade requestFacade)
        {
            this.requestFacade = requestFacade ?? throw new ArgumentNullException(nameof(requestFacade));
        }

        /// <summary>
        /// Create a raw material request.
        /// </summary>
        /// <param name="request">New request to add.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        [HttpPost]
        [Route("/request/rawmaterial")]
        public async Task<IActionResult> CreateRawMaterialRequest(UserActionDto<RawMaterialRequestDto> request)
        {
            var response = await this.requestFacade.CreateRawMaterialRequest(request.UserId, request.Data);
            return this.Ok(response);
        }

        /// <summary>
        /// Update a raw material request.
        /// </summary>
        /// <param name="request">New request to add.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        [HttpPut]
        [Route("/request/rawmaterial")]
        public async Task<IActionResult> UpdateRawMaterialRequest(UserActionDto<RawMaterialRequestDto> request)
        {
            var response = await this.requestFacade.UpdateRawMaterialRequest(request.UserId, request.Data);
            return this.Ok(response);
        }

        /// <summary>
        /// Get a raw material request.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>The material request.</returns>
        [HttpGet]
        [Route("/request/rawmaterial")]
        public async Task<IActionResult> GetRawMaterialRequest([FromQuery] int productionOrderId)
        {
            var response = await this.requestFacade.GetRawMaterialRequest(productionOrderId);
            return this.Ok(response);
        }

        /// <summary>
        /// Get a raw material pre-request.
        /// </summary>
        /// <param name="salesOrders">the sales order ids.</param>
        /// <param name="productionOrders">the production order ids.</param>
        /// <returns>The material request.</returns>
        [HttpGet]
        [Route("/prerequest/rawmaterial")]
        public async Task<IActionResult> GetRawMaterialPreRequest(
            [FromQuery] string salesOrders,
            [FromQuery] string productionOrders)
        {
            var salesOrdersIds = (salesOrders ?? string.Empty).ToIntList();
            var productionOrdersIds = (productionOrders ?? string.Empty).ToIntList();

            var response = await this.requestFacade.GetRawMaterialPreRequest(salesOrdersIds, productionOrdersIds);
            return this.Ok(response);
        }

        /// <summary>
        /// Method Ping.
        /// </summary>
        /// <returns>Pong.</returns>
        [Route("/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return this.Ok("Pong");
        }
    }
}