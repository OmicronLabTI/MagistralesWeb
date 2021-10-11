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
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// Class for the petitions for almacen.
    /// </summary>
    public class PedidosAlmacenFacade : IPedidosAlmacenFacade
    {
        private readonly IMapper mapper;

        private readonly IPedidosAlmacenService almacenService;

        private readonly ICancelPedidosService cancelPedidosService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosAlmacenFacade"/> class.
        /// </summary>
        /// <param name="almacenService">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        /// <param name="cancelPedidosService">the cancel service.</param>
        public PedidosAlmacenFacade(IPedidosAlmacenService almacenService, IMapper mapper, ICancelPedidosService cancelPedidosService)
        {
            this.almacenService = almacenService ?? throw new ArgumentNullException(nameof(almacenService));
            this.cancelPedidosService = cancelPedidosService ?? throw new ArgumentNullException(nameof(cancelPedidosService));
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

        /// <inheritdoc/>
        public async Task<ResultDto> GetUserOrderByDeliveryOrder(List<int> deliveryIds)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetUserOrderByDeliveryOrder(deliveryIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreatePdf(string type, List<int> invoiceIds)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.CreatePdf(type, invoiceIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CancelDelivery(string type, CancelDeliveryPedidoCompleteDto deliveryIds)
        {
            return this.mapper.Map<ResultDto>(await this.cancelPedidosService.CancelDelivery(type, this.mapper.Map<CancelDeliveryPedidoCompleteModel>(deliveryIds)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CleanInvoices(List<int> invoiceIds)
        {
            return this.mapper.Map<ResultDto>(await this.cancelPedidosService.CleanInvoices(invoiceIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> AdvanceLook(List<int> docNums)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.AdvanceLook(docNums));
        }
    }
}
