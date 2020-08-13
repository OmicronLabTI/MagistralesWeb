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

        /// <summary>
        /// returns the list of userOrder by sales order.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetUserOrderBySalesOrder(List<int> listIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetUserOrderBySalesOrder(listIds));
        }

        /// <summary>
        /// Gets the orders of a specific QFB (ipad).
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the list to returns.</returns>
        public async Task<ResultDto> GetFabOrderByUserID(string userId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetFabOrderByUserID(userId));
        }

        /// <summary>
        /// Gets the user orders by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetUserOrdersByUserId(List<string> listIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetUserOrdersByUserId(listIds));
        }

        /// <summary>
        /// Assigns the order.
        /// </summary>
        /// <param name="manualAssign">the dto to assign.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> AssignHeader(ManualAssignDto manualAssign)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.AssignOrder(this.mapper.Map<ManualAssignModel>(manualAssign)));
        }

        /// <summary>
        /// updates the formulas for the order.
        /// </summary>
        /// <param name="updateFormula">the update object.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> UpdateComponents(UpdateFormulaDto updateFormula)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateComponents(this.mapper.Map<UpdateFormulaModel>(updateFormula)));
        }
    }
}
