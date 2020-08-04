// <summary>
// <copyright file="PedidoFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// the pedidos facade.
    /// </summary>
    public class PedidoFacade : IPedidoFacade
    {
        /// <summary>
        /// Mapper Object.
        /// </summary>
        private readonly IMapper mapper;

        private readonly IPedidosService pedidoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidoFacade"/> class.
        /// </summary>
        /// <param name="pedidoService">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        public PedidoFacade(IPedidosService pedidoService, IMapper mapper)
        {
            this.pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            this.mapper = mapper;
        }

        /// <summary>
        /// process the orders.
        /// </summary>
        /// <param name="orderDto">the pedidos list.</param>
        /// <returns>the result.</returns>
        public async Task<ResultDto> ProcessOrders(ProcessOrderDto orderDto)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.ProcessOrders(this.mapper.Map<ProcessOrderModel>(orderDto)));
        }
    }
}
