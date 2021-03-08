// <summary>
// <copyright file="BusquedaPedidoFacade.cs" company="Axity">
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
    /// class for the lookup.
    /// </summary>
    public class BusquedaPedidoFacade : IBusquedaPedidoFacade
    {
        /// <summary>
        /// Mapper Object.
        /// </summary>
        private readonly IMapper mapper;

        private readonly IBusquedaPedidoService busquedaPedidoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusquedaPedidoFacade"/> class.
        /// </summary>
        /// <param name="busquedaPedido">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        public BusquedaPedidoFacade(IMapper mapper, IBusquedaPedidoService busquedaPedido)
        {
            this.busquedaPedidoService = busquedaPedido ?? throw new ArgumentNullException(nameof(busquedaPedido));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrders(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.busquedaPedidoService.GetOrders(parameters));
        }
    }
}
