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
    using Omicron.Pedidos.Resources.Enums;
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

        /// <summary>
        /// updates the status of the orders.
        /// </summary>
        /// <param name="updateStatus">the status object.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> UpdateStatusOrder(List<UpdateStatusOrderDto> updateStatus)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateStatusOrder(this.mapper.Map<List<UpdateStatusOrderModel>>(updateStatus)));
        }

        /// <summary>
        /// gets the connection to DI api.
        /// </summary>
        /// <returns>the connectin.</returns>
        public async Task<ResultDto> ConnectDiApi()
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.ConnectDiApi());
        }

        /// <summary>
        /// Process by order.
        /// </summary>
        /// <param name="processByOrder">process by order dto.</param>
        /// <returns>the order.</returns>
        public async Task<ResultDto> ProcessByOrder(ProcessByOrderDto processByOrder)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.ProcessByOrder(this.mapper.Map<ProcessByOrderModel>(processByOrder)));
        }

        /// <summary>
        /// Change order status to cancel.
        /// </summary>
        /// <param name="cancelOrders">Update order info.</param>
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultDto> CancelOrder(List<CancelOrderDto> cancelOrders)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CancelOrder(this.mapper.Map<List<CancelOrderModel>>(cancelOrders)));
        }

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</para
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultDto> CancelFabOrder(List<CancelOrderDto> cancelOrders)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CancelFabOrder(this.mapper.Map<List<CancelOrderModel>>(cancelOrders)));
        }

        /// <summary>
        /// the automatic assign.
        /// </summary>
        /// <param name="automaticAssing">the assign object.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> AutomaticAssign(AutomaticAssingDto automaticAssing)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.AutomaticAssign(this.mapper.Map<AutomaticAssingModel>(automaticAssing)));
        }

        /// <summary>
        /// Save signatures.
        /// </summary>
        /// <param name="signatureType">The signature type.</param>
        /// <param name="signatureModel">The signature info.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultDto> UpdateOrderSignature(SignatureTypeEnum signatureType, UpdateOrderSignatureDto signatureModel)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateOrderSignature(signatureType, this.mapper.Map<UpdateOrderSignatureModel>(signatureModel)));
        }

        /// <summary>
        /// Get production order signatures.
        /// </summary>
        /// <param name="productionOrderId">Production order id.</param>
        /// <returns>Operation result.</returns>
        public async Task<ResultDto> GetOrderSignatures(int productionOrderId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetOrderSignatures(productionOrderId));
        }
    }
}
