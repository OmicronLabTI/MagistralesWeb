// ------------------------------------------------------------------------------------------------
// <copyright file="SapDiApiController.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;    
    using Omicron.SapDiApi.Dtos.Models;
    using Omicron.SapDiApi.Facade.Sap;
    
    /// <summary>
    /// Layout Element controller for any layout element which belongs to a specific layout
    /// </summary>
    [RoutePrefix("SapDiApi")]
    public class SapDiApiController : BaseController
    {
        /// <summary>
        /// Gets or sets the sa.
        /// </summary>
        private readonly ISapFacade sapFacade;

        /// <summary>
        /// /// Initializes a new instance of the <see cref="SapDiApiController"/> class.
        /// </summary>
        /// <param name="sapFacade">the facade.</param>
        public SapDiApiController(ISapFacade sapFacade)
        {
            this.sapFacade = sapFacade;
        }

        /// <summary>
        /// the createorder
        /// </summary>
        /// <param name="orderWithDetailDto">the detial.</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("createFabOrder")]
        public async Task<IHttpActionResult> CreateFabOrder([FromBody] List<OrderWithDetailDto> orderWithDetailDto)
        {
            var result = this.sapFacade.CreateFabOrder(orderWithDetailDto);
            return this.Ok(result);
        }

        /// <summary>
        /// the ping pong.
        /// </summary>
        /// <returns>rturn pong.</returns>
        [HttpGet]
        [Route("ping")]
        public async Task<IHttpActionResult> Get()
        {
            return this.Ok("pong");
        }
    }
}