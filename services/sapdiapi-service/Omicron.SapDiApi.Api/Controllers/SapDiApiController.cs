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
    using AutoMapper;
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
            var result = await this.sapFacade.CreateFabOrder(orderWithDetailDto);
            return this.Ok(result);
        }

        /// <summary>
        /// updates a fabrication order.
        /// </summary>
        /// <param name="updateFabOrderDto">the list of fabrication orders..</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("updateFabOrder")]
        public async Task<IHttpActionResult> UpdateFabOrder([FromBody] List<UpdateFabOrderDto> updateFabOrderDto)
        {
            var result = await this.sapFacade.UpdateFabOrder(updateFabOrderDto);
            return this.Ok(result);
        }

        /// <summary>
        /// updates a fabrication order.
        /// </summary>
        /// <param name="updateFormula">the list of fabrication orders..</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("updateFormula")]
        public async Task<IHttpActionResult> UpdateFormula([FromBody] UpdateFormulaDto updateFormula)
        {
            var result = await this.sapFacade.UpdateFormula(updateFormula);
            return this.Ok(result);
        }

        /// <summary>
        /// Cancel production order by id
        /// </summary>
        /// <param name="productionOrder">Production order to update</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("cancelProductionOrder")]
        public async Task<IHttpActionResult> CancelProcutionOrder([FromBody] CancelOrderDto productionOrder)
        {
            var result = await this.sapFacade.CancelProductionOrder(productionOrder);
            return this.Ok(result);
        }

        /// <summary>
        /// Cancel production order by id
        /// </summary>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("batches")]
        public async Task<IHttpActionResult> UpdateBatches(List<AssginBatchDto> assginBatches)
        {
            var result = await this.sapFacade.UpdateBatches(assginBatches);
            return this.Ok(result);
        }

        /// <summary>
        /// the ping pong.
        /// </summary>
        /// <returns>rturn pong.</returns>
        [HttpGet]
        [Route("connect")]
        public async Task<IHttpActionResult> GetConnection()
        {
            var result = await this.sapFacade.Connect();
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