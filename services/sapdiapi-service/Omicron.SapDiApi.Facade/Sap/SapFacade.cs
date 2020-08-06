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
    using SAPbobsCOM;
    public class SapFacade : ISapFacade
    {
        private readonly IMapper mapper;

        private readonly Company company;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFacade"/> class.
        /// </summary>        
        /// <param name="mapper"></param>
        public SapFacade(IMapper mapper)
        {
            this.mapper = mapper;
            this.company = Connection.Company; 
        }

        /// <summary>
        /// creates order.
        /// </summary>
        /// <returns>the result.</returns>
        public async Task<ResultDto> CreateFabOrder(List<OrderWithDetailDto> orderWithDetailDto)
        {
            return new ResultDto();
        }
    }
}
