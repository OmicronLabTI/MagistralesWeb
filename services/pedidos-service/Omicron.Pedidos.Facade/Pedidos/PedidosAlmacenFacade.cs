// <summary>
// <copyright file="PedidosAlmacenFacade.cs" company="Axity">
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
    /// Class for the petitions for almacen.
    /// </summary>
    public class PedidosAlmacenFacade : IPedidosAlmacenFacade
    {
        private readonly IMapper mapper;

        private readonly IPedidosAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosAlmacenFacade"/> class.
        /// </summary>
        /// <param name="almacenService">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        public PedidosAlmacenFacade(IPedidosAlmacenService almacenService, IMapper mapper)
        {
            this.almacenService = almacenService ?? throw new ArgumentNullException(nameof(almacenService));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrdersForAlmacen()
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetOrdersForAlmacen());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateUserOrders(List<UserOrderDto> userOrders)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.UpdateUserOrders(this.mapper.Map<List<UserOrderModel>>(userOrders)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrdersForDelivery()
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetOrdersForDelivery());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrdersForInvoice()
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetOrdersForInvoice());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrdersForPackages(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetOrdersForPackages(parameters));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateSentOrders(List<UserOrderDto> usersToUpdate)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.UpdateSentOrders(this.mapper.Map<List<UserOrderModel>>(usersToUpdate)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetAlmacenGraphData(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetAlmacenGraphData(parameters));
        }
    }
}
