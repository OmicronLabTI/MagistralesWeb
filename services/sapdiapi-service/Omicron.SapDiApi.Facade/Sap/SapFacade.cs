// <summary>
// <copyright file="SapFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.SapDiApi.Dtos.Models;
    using Omicron.SapDiApi.Entities.Context;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Services.SapDiApi;
    using SAPbobsCOM;
    public class SapFacade : ISapFacade
    {
        private readonly IMapper mapper;

        private readonly ISapDiApiService sapDiApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFacade"/> class.
        /// </summary>        
        /// <param name="mapper"></param>
        public SapFacade(IMapper mapper, ISapDiApiService sapDiApiService)
        {
            this.mapper = mapper;
            this.sapDiApiService = sapDiApiService;
        }

        /// <summary>
        /// connecto to sap.
        /// </summary>
        /// <returns>connects.</returns>
        public async Task<ResultDto> Connect()
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.Connect());
        }

        /// <summary>
        /// creates order.
        /// </summary>
        /// <returns>the result.</returns>
        public async Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetailDto)
        {
            var model = this.mapper.Map<List<OrderWithDetailModel>>(orderWithDetailDto);
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.InsertOrdenFab(model));
        }

        /// <summary>
        /// updates the fabriction orders.
        /// </summary>
        /// <param name="updateFabOrderDtos">the orders to update.</param>
        /// <returns>the reult.</returns>
        public async Task<ResultDto> UpdateFabOrder(List<UpdateFabOrderDto> updateFabOrderDtos)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.UpdateFabOrders(this.mapper.Map<List<UpdateFabOrderModel>>(updateFabOrderDtos)));
        }

        /// <summary>
        /// Updates the formula.
        /// </summary>
        /// <param name="updateFormula">the object to update.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> UpdateFormula(UpdateFormulaDto updateFormula)
        {
            return this.mapper.Map<ResultDto>(await this.sapDiApiService.UpdateFormula(this.mapper.Map<UpdateFormulaModel>(updateFormula)));
        }
    }
}
