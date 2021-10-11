// <summary>
// <copyright file="PedidosDxpFacade.cs" company="Axity">
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
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// class for dxp facade.
    /// </summary>
    public class PedidosDxpFacade : IPedidosDxpFacade
    {
        /// <summary>
        /// Mapper Object.
        /// </summary>
        private readonly IMapper mapper;

        private readonly IPedidosDxpService pedidosDxpService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosDxpFacade"/> class.
        /// </summary>
        /// <param name="pedidosDxpService">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        public PedidosDxpFacade(IPedidosDxpService pedidosDxpService, IMapper mapper)
        {
            this.pedidosDxpService = pedidosDxpService ?? throw new ArgumentNullException(nameof(pedidosDxpService));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetDeliveredPayments(List<int> ordersId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidosDxpService.GetDeliveredPayments(ordersId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrdersHeaderStatus(List<string> orders)
        {
            return this.mapper.Map<ResultDto>(await this.pedidosDxpService.GetOrdersHeaderStatus(orders));
        }
    }
}
