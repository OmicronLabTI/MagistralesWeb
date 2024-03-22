// <summary>
// <copyright file="OrdersFacade.cs" company="Axity">
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
    public class OrdersFacade : IOrdersFacade
    {
        private readonly IMapper mapper;
        private readonly IOrdersService ordersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="ordersService">Orders Service.</param>
        public OrdersFacade(IMapper mapper, IOrdersService ordersService)
        {
            this.mapper = mapper;
            this.ordersService = ordersService.ThrowIfNull(nameof(ordersService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetLastGeneratedOrder()
            => this.mapper.Map<ResultDto>(await this.ordersService.GetLastGeneratedOrder());
    }
}
