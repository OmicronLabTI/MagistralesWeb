// <summary>
// <copyright file="UsersController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Omicron.Logs.Dtos.OrderLog;
    using Omicron.Logs.Facade.OrderLogs;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using StackExchange.Redis;  

    /// <summary>
    /// Class OrdersLogs Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersLogsController : ControllerBase
    {
        private readonly IOrdersLogsFacade logicFacade;

        private readonly IDatabase database;

        private readonly IConnectionMultiplexer redis;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersLogsController"/> class.
        /// </summary>
        /// <param name="logicFacade">OrderLog Facade.</param>
        /// <param name="redis">Redis Cache.</param>
        public OrdersLogsController(IOrderLogFacade logicFacade, IConnectionMultiplexer redis)
        {
            this.logicFacade = logicFacade ?? throw new ArgumentNullException(nameof(logicFacade));
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.database = redis.GetDatabase();
        }

        /// <summary>
        /// Method to Add OrderLog registry.
        /// </summary>
        /// <param name="orderlog">OrderLog Model.</param>
        /// <returns>Success or exception.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderLogDto orderlog)
        {
            var response = await this.logicFacade.InsertOrderLog(orderlog);
            return this.Ok(response);
        }
    }
}