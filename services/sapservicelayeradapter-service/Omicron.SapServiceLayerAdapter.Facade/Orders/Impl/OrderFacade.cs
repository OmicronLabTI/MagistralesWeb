// <summary>
// <copyright file="OrderFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.Orders.Impl
{
    /// <summary>
    /// Class OrdersFacade.
    /// </summary>
    public class OrderFacade : IOrderFacade
    {
        private readonly IMapper mapper;
        private readonly IOrderService ordersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="ordersService">Orders Service.</param>
        public OrderFacade(IMapper mapper, IOrderService ordersService)
        {
            this.mapper = mapper;
            this.ordersService = ordersService.ThrowIfNull(nameof(ordersService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetLastGeneratedOrder()
            => this.mapper.Map<ResultDto>(await this.ordersService.GetLastGeneratedOrder());
    }
}
