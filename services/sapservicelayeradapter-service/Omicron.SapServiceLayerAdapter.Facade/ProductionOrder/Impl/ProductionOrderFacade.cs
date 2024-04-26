// <summary>
// <copyright file="ProductionOrderFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.ProductionOrder.Impl
{
    /// <summary>
    /// Class ProductionOrderFacade.
    /// </summary>
    public class ProductionOrderFacade : IProductionOrderFacade
    {
        private readonly IMapper mapper;
        private readonly IProductionOrderService productionOrderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionOrderFacade"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="productionOrderService">The service.</param>
        public ProductionOrderFacade(IMapper mapper, IProductionOrderService productionOrderService)
        {
            this.mapper = mapper;
            this.productionOrderService = productionOrderService.ThrowIfNull(nameof(productionOrderService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> FinishOrder(List<CloseProductionOrderDto> productionOrders)
            => this.mapper.Map<ResultDto>(await this.productionOrderService.FinishOrder(productionOrders));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateProductionOrdersBatches(List<AssignBatchDto> batchesToAssign)
            => this.mapper.Map<ResultDto>(await this.productionOrderService.UpdateProductionOrdersBatches(batchesToAssign));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula)
            => this.mapper.Map<ResultDto>(await this.productionOrderService.UpdateFormula(updateFormula));

        /// <inheritdoc/>
        public async Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetail)
            => this.mapper.Map<ResultDto>(await this.productionOrderService.CreateFabOrder(orderWithDetail));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateFabOrders(List<UpdateFabOrderDto> orderModels)
            => this.mapper.Map<ResultDto>(await this.productionOrderService.UpdateFabOrders(orderModels));

        /// <inheritdoc/>
        public async Task<ResultDto> CancelProductionOrder(CancelOrderDto productionOrder)
            => this.mapper.Map<ResultDto>(await this.productionOrderService.CancelProductionOrder(productionOrder));
    }
}