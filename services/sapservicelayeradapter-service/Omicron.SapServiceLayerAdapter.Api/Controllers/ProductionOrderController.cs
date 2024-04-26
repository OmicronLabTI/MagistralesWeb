// <summary>
// <copyright file="ProductionOrderController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
{
    /// <summary>
    /// ProductionOrderController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionOrderController : ControllerBase
    {
        private readonly IProductionOrderFacade productionOrderFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrderController"/> class.
        /// </summary>
        /// <param name="productionOrderFacade">The facade.</param>
        public ProductionOrderController(IProductionOrderFacade productionOrderFacade)
        {
            this.productionOrderFacade = productionOrderFacade ?? throw new ArgumentNullException(nameof(productionOrderFacade));
        }

        /// <summary>
        /// Finish production order.
        /// </summary>
        /// <param name="productionOrdes">The productionOrdes.</param>
        /// <returns>Result.</returns>
        [HttpPost]
        [Route("/finishProducionOrders")]
        public async Task<IActionResult> FinishOrder([FromBody] List<CloseProductionOrderDto> productionOrdes)
            => this.Ok(await this.productionOrderFacade.FinishOrder(productionOrdes));

        /// <summary>
        /// Update Production Orders Batches.
        /// </summary>
        /// <param name="batchesToAssign">Batches to assign.</param>
        /// <returns>Operation results.</returns>
        [HttpPatch]
        [Route("/productionorders/batches")]
        public async Task<IActionResult> UpdateProductionOrdersBatches([FromBody] List<AssignBatchDto> batchesToAssign)
            => this.Ok(await this.productionOrderFacade.UpdateProductionOrdersBatches(batchesToAssign));

        /// <summary>
        /// updates a fabrication order.
        /// </summary>
        /// <param name="updateFormula">the list of fabrication orders.</param>
        /// <returns>the result.</returns>
        [HttpPost]
        [Route("/updateFormula")]
        public async Task<IActionResult> UpdateFormula([FromBody] UpdateFormulaDto updateFormula)
        {
            var result = await this.productionOrderFacade.UpdateFormula(updateFormula);
            return this.Ok(result);
        }

        /// <summary>
        /// the create order.
        /// </summary>
        /// <param name="orderWithDetailDto">the detail.</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("/createFabOrder")]
        public async Task<IActionResult> CreateFabOrder([FromBody] List<OrderWithDetailDto> orderWithDetailDto)
        {
            var result = await this.productionOrderFacade.CreateFabOrder(orderWithDetailDto);
            return this.Ok(result);
        }

        /// <summary>
        /// updates a fabrication order.
        /// </summary>
        /// <param name="updateFabOrderDto">the list of fabrication orders..</param>
        /// <returns>the result.</returns>
        [HttpPost]
        [Route("/updateFabOrder")]
        public async Task<IActionResult> UpdateFabOrder([FromBody] List<UpdateFabOrderDto> updateFabOrderDto)
        {
            var result = await this.productionOrderFacade.UpdateFabOrders(updateFabOrderDto);
            return this.Ok(result);
        }

        /// <summary>
        /// Cancel production order by id.
        /// </summary>
        /// <param name="productionOrder">Production order to update.</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("/cancelProductionOrder")]
        public async Task<IActionResult> CancelProcutionOrder([FromBody] CancelOrderDto productionOrder)
        {
            var result = await this.productionOrderFacade.CancelProductionOrder(productionOrder);
            return this.Ok(result);
        }
    }
}