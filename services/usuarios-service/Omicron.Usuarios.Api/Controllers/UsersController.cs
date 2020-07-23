// <summary>
// <copyright file="UsersController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Facade.Catalogs.Users;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using Omicron.Usuarios.Dtos.Models;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserFacade userFacade;

        private readonly IDatabase database;

        private readonly IConnectionMultiplexer redis;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userFacade">User Facade.</param>
        /// <param name="redis">Redis Cache.</param>
        public UsersController(IUserFacade userFacade, IConnectionMultiplexer redis)
        {
            this.userFacade = userFacade ?? throw new ArgumentNullException(nameof(userFacade));
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.database = redis.GetDatabase();
        }

        /// <summary>
        /// Method to validate the credentials.
        /// </summary>
        /// <param name="loginDto">the loginDto.</param>
        /// <returns>If the result of validation.</returns>
        [HttpPost]
        [Route("/validatecredentials")]
        public async Task<IActionResult> ValidateCredentials([FromBody] LoginDto loginDto)
        {
            var response = await this.userFacade.ValidateCredentials(loginDto);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get all users.
        /// </summary>
        /// <returns>List of users.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await this.userFacade.GetListUsersActive();
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get user By Id.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>User Model.</returns>
        [Route("{userId}")]
        [HttpGet]
        public async Task<IActionResult> Get(int userId)
        {
            UserDto response = null;

            ////Example to get value with Redis Cache
            var result = await this.database.StringGetAsync(userId.ToString());

            if (!result.HasValue)
            {
                response = await this.userFacade.GetListUserActive(userId);

                ////Example to set value with Redis Cache
                await this.database.StringSetAsync(userId.ToString(), JsonConvert.SerializeObject(response));
            }
            else
            {
                ////If key in Redis, deserialize response and return object
                response = JsonConvert.DeserializeObject<UserDto>(result);
            }

            return this.Ok(response);
        }

        /// <summary>
        /// Method to Add User.
        /// </summary>
        /// <param name="user">User Model.</param>
        /// <returns>Success or exception.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            var response = await this.userFacade.InsertUser(user);
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