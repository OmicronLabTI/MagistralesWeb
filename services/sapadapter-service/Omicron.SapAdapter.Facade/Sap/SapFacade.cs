// <summary>
// <copyright file="SapFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Facade.Sap
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Services.Sap;

    /// <summary>
    /// Class for the facade.
    /// </summary>
    public class SapFacade : ISapFacade
    {
        private readonly IMapper mapper;

        private readonly ISapService sapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFacade"/> class.
        /// </summary>
        /// <param name="sapService">the sap service.</param>
        /// <param name="mapper">the mapper.</param>
        public SapFacade(ISapService sapService, IMapper mapper)
        {
            this.mapper = mapper;
            this.sapService = sapService ?? throw new ArgumentNullException(nameof(sapService));
        }

        /// <summary>
        /// Method to return orders.
        /// </summary>
        /// <param name="parameters">The params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> GetOrders(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetOrders(parameters));
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="docEntry">the order ir.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> GetDetallePedidos(string docEntry)
        {
            int.TryParse(docEntry, out var docId);
            return this.mapper.Map<ResultDto>(await this.sapService.GetOrderDetails(docId));
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="pedidosId">the order ir.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> GetPedidoWithDetail(List<int> pedidosId)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetPedidoWithDetail(pedidosId));
        }

        /// <summary>
        /// Gets the production orders bu produc and id.
        /// </summary>
        /// <param name="pedidosId">list ids each elemente is orderId-producId.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetProdOrderByOrderItem(List<string> pedidosId)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetProdOrderByOrderItem(pedidosId));
        }

        /// <summary>
        /// gets the formula.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <param name="returnFirst">If it will return the first.</param>
        /// <param name="returnDetails">if it will look for details.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetOrderFormula(List<int> orderId, bool returnFirst, bool returnDetails)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetOrderFormula(orderId, returnFirst, returnDetails));
        }

        /// <summary>
        /// Get fabrication orders by criterial.
        /// </summary>
        /// <param name="salesOrderIds">Sales order ids.</param>
        /// <param name="fabricationOrderIds">Fabrication order ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetFabricationOrdersByCriterial(List<int> salesOrderIds, List<int> fabricationOrderIds)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetFabricationOrdersByCriterial(salesOrderIds, fabricationOrderIds));
        }

        /// <summary>
        /// Gets the componenets based in the dic.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetComponents(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetComponents(parameters));
        }

        /// <summary>
        /// Gets the componentes managed by batches.
        /// </summary>
        /// <param name="ordenId">the order id.</param>
        /// <returns>the components.</returns>
        public async Task<ResultDto> GetBatchesComponents(int ordenId)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetBatchesComponents(ordenId));
        }

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetlLastIsolatedProductionOrderId(string productId, string uniqueId)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetlLastIsolatedProductionOrderId(productId, uniqueId));
        }

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetNextBatchCode(string productCode)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetNextBatchCode(productCode));
        }

        /// <summary>
        /// Validate if exists batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <param name="batchCode">the batch code.</param>
        /// <returns>the validation result.</returns>
        public async Task<ResultDto> ValidateIfExistsBatchCodeByItemCode(string productCode, string batchCode)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.ValidateIfExistsBatchCodeByItemCode(productCode, batchCode));
        }

        /// <summary>
        /// Look for the orders.
        /// </summary>
        /// <param name="orderFabDto">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetFabOrders(GetOrderFabDto orderFabDto)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetFabOrders(this.mapper.Map<GetOrderFabModel>(orderFabDto)));
        }

        /// <summary>
        /// Get products management by batches with criterials.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetProductsManagmentByBatch(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetProductsManagmentByBatch(parameters));
        }

        /// <summary>
        /// Gets the orders by ordersId.
        /// </summary>
        /// <param name="listOrdersId">The orders ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetFabOrdersById(List<int> listOrdersId)
        {
            return this.mapper.Map<ResultDto>(await this.sapService.GetFabOrdersById(listOrdersId));
        }
    }
}
