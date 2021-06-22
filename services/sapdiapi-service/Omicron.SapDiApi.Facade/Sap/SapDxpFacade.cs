// <summary>
// <copyright file="SapDxpFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapDiApi.Dtos.Models.Experience;
    using Omicron.SapDiApi.Dtos.Models;
    using AutoMapper;
    using Omicron.SapDiApi.Services.SapDiApi;
    using Omicron.SapDiApi.Entities.Models.Experience;

    /// <summary>
    /// class for dxp.
    /// </summary>
    public class SapDxpFacade : ISapDxpFacade
    {
        private readonly IMapper mapper;

        private readonly ISapCreateSaleOrder sapCreateSaleOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapDxpFacade"/> class.
        /// </summary>        
        /// <param name="mapper"></param>
        /// <param name="sapCreateSaleOrder">creates and order.</param>
        public SapDxpFacade(IMapper mapper, ISapCreateSaleOrder sapCreateSaleOrder)
        {
            this.mapper = mapper;
            this.sapCreateSaleOrder = sapCreateSaleOrder;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateSaleOrder(CreateSaleOrderDto saleOrderDto)
        {
            return this.mapper.Map<ResultDto>(await this.sapCreateSaleOrder.CreateSaleOrder(this.mapper.Map<CreateSaleOrderModel>(saleOrderDto)));
        }
    }
}
