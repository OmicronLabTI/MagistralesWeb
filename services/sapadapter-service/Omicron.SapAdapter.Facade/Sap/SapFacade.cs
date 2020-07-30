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
    }
}
