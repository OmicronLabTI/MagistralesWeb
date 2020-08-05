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
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Omicron.Usuarios.Dtos.Models;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Facade.Catalogs.Users;
    using StackExchange.Redis;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserFacade userFacade;

        private readonly IDatabase database;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userFacade">User Facade.</param>
        /// <param name="redis">Redis Cache.</param>
        public UsersController(IUserFacade userFacade)
        {
            this.userFacade = userFacade ?? throw new ArgumentNullException(nameof(userFacade));
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
        /// The create user method.
        /// </summary>
        /// <param name="userDto">the userDto.</param>
        /// <returns>the status of the insert.</returns>
        [HttpPost]
        [Route("/createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var response = await this.userFacade.CreateUser(userDto);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets all the users.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>the users.</returns>
        [HttpGet]
        [Route("/getUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.userFacade.GetUsers(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// gets the qfb.
        /// </summary>
        /// <returns>the list.</returns>
        [HttpGet]
        [Route("/qfb")]
        public async Task<IActionResult> GetQfb()
        {
            var response = await this.userFacade.GetQfb();
            return this.Ok(response);
        }

        /// <summary>
        /// method to delete user.
        /// </summary>
        /// <param name="listIds">the list of id.</param>
        /// <returns>the response.</returns>
        [Route("/deactivateUser")]
        [HttpPatch]
        public async Task<IActionResult> DeleteUsers(string[] listIds)
        {
            var response = await this.userFacade.DeleteUser(listIds.ToList());
            return this.Ok(response);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">the user to update.</param>
        /// <returns>the response.</returns>
        [Route("/updateUser")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserDto user)
        {
            var response = await this.userFacade.UpdateUser(user);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get user By Id.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>User Model.</returns>
        [Route("/user/{userId}")]
        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var result = await this.userFacade.GetUser(userId.ToString());
            return this.Ok(result);
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