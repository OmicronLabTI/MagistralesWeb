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
        /// <param name="assginBatches">The assign batches mnodel.</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("batches")]
        public async Task<IHttpActionResult> UpdateBatches(List<AssginBatchDto> assginBatches)
        {
            var result = await this.sapFacade.UpdateBatches(assginBatches);
            return this.Ok(result);
        }

        /// <summary>
        /// Finish production order.
        /// </summary>
        /// <param name="productionOrdes">Production orders to finish.</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("finishProducionOrders")]
        public IHttpActionResult FinishProductionOrders([FromBody] List<CloseProductionOrderDto> productionOrdes)
        {
            var result = this.sapFacade.FinishOrder(productionOrdes);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("isolatedProductionOrder")]
        public async Task<IHttpActionResult> CreateIsolatedProductionOrder([FromBody] CreateIsolatedFabOrderDto isolatedFabOrder)
        {
            var result = await this.sapFacade.CreateIsolatedProductionOrder(isolatedFabOrder);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="createDelivery">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("delivery")]
        public async Task<IHttpActionResult> CreateDelivery([FromBody] List<CreateDeliveryDto> createDelivery)
        {
            var result = await this.sapFacade.CreateDelivery(createDelivery);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="createDelivery">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("delivery/partial")]
        public async Task<IHttpActionResult> CreateDeliveryPartial([FromBody] List<CreateDeliveryDto> createDelivery)
        {
            var result = await this.sapFacade.CreateDeliveryPartial(createDelivery);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="updateTracking">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("invoice/tracking")]
        public async Task<IHttpActionResult> UpdateTracking([FromBody] SendPackageDto updateTracking)
        {
            var result = await this.sapFacade.UpdateTracking(updateTracking);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="deliveries">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("cancel/delivery")]
        public async Task<IHttpActionResult> CancelDelivery([FromBody] List<int> deliveries)
        {
            var result = await this.sapFacade.CancelDelivery(deliveries);
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