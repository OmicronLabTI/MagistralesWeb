// <summary>
// <copyright file="ReportingController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingController"/> class.
        /// </summary>
        public ReportingController()
        {
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