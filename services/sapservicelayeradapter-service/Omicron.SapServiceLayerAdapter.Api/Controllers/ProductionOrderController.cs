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
    }
}