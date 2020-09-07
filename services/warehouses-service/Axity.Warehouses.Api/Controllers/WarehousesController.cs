// <summary>
// <copyright file="WarehousesController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Axity.Warehouses.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Axity.Warehouses.Dtos.Model;
    using Axity.Warehouses.Facade.Request;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IRequestFacade requestFacade;

        private readonly IDatabase database;

        private readonly IConnectionMultiplexer redis;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehousesController"/> class.
        /// </summary>
        /// <param name="requestFacade">Request Facade.</param>
        /// <param name="redis">Redis Cache.</param>
        public WarehousesController(IRequestFacade requestFacade, IConnectionMultiplexer redis)
        {
            this.requestFacade = requestFacade ?? throw new ArgumentNullException(nameof(requestFacade));
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.database = redis.GetDatabase();
        }

        /// <summary>
        /// Create a raw material request.
        /// </summary>
        /// <param name="requests">New request to add.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        [HttpPost]
        [Route("/request/rawmaterial")]
        public async Task<IActionResult> CreateRawMaterialRequest(UserActionDto<List<RawMaterialRequestDto>> requests)
        {
            var response = await this.requestFacade.CreateRawMaterialRequest(requests.UserId, requests.Data);
            return this.Ok(response);
        }

        /// <summary>
        /// Update a raw material request.
        /// </summary>
        /// <param name="requests">New request to add.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        [HttpPut]
        [Route("/request/rawmaterial")]
        public async Task<IActionResult> UpdateRawMaterialRequest(UserActionDto<List<RawMaterialRequestDto>> requests)
        {
            var response = await this.requestFacade.UpdateRawMaterialRequest(requests.UserId, requests.Data);
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