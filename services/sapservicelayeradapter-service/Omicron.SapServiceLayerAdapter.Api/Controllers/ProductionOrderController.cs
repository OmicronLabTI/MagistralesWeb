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
        /// Primary Validation For Production Order Finalization In Sap.
        /// </summary>
        /// <param name="productionOrderInfoToValidate">Production Order Info To Validate.</param>
        /// <returns>Result.</returns>
        [HttpPost]
        [Route("/validation/productionorders/finalization")]
        public async Task<IActionResult> PrimaryValidationForProductionOrderFinalizationInSap([FromBody] List<CloseProductionOrderDto> productionOrderInfoToValidate)
            => this.Ok(await this.productionOrderFacade.PrimaryValidationForProductionOrderFinalizationInSap(productionOrderInfoToValidate));

        /// <summary>
        /// Finalize Production Order In Sap.
        /// </summary>
        /// <param name="productionOrdersToFinalize">Production Order Info To Finalize.</param>
        /// <returns>Result.</returns>
        [HttpPost]
        [Route("/finalize/productionorders")]
        public async Task<IActionResult> FinalizeProductionOrderInSap([FromBody] List<CloseProductionOrderDto> productionOrdersToFinalize)
            => this.Ok(await this.productionOrderFacade.FinalizeProductionOrderInSap(productionOrdersToFinalize));

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

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("/isolatedProductionOrder")]
        public async Task<IActionResult> CreateIsolatedProductionOrder([FromBody] CreateIsolatedFabOrderDto isolatedFabOrder)
        {
            var result = await this.productionOrderFacade.CreateIsolatedProductionOrder(isolatedFabOrder);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="isolatedFabOrder">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("/child/order")]
        public async Task<IActionResult> CreateChildFabOrders([FromBody] CreateChildProductionOrdersDto isolatedFabOrder)
        {
            var result = await this.productionOrderFacade.CreateChildFabOrders(isolatedFabOrder);
            return this.Ok(result);
        }

        /// <summary>
        /// CancelProductionOrderForSeparationProcess.
        /// </summary>
        /// <param name="cancelProductionOrder">cancelProductionOrder.</param>
        /// <returns>the reult.</returns>
        [HttpPost]
        [Route("/separationprocess/cancelproductionorder")]
        public async Task<IActionResult> CancelProductionOrderForSeparationProcess([FromBody] CancelProductionOrderDto cancelProductionOrder)
         => this.Ok(await this.productionOrderFacade.CancelProductionOrderForSeparationProcess(cancelProductionOrder));
    }
}