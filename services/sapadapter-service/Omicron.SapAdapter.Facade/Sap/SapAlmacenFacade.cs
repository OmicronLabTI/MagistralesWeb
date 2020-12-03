// <summary>
// <copyright file="SapAlmacenFacade.cs" company="Axity">
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
    /// Class for sap almacen facade.
    /// </summary>
    public class SapAlmacenFacade : ISapAlmacenFacade
    {
        private readonly IMapper mapper;

        private readonly ISapAlmacenService almacenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAlmacenFacade"/> class.
        /// </summary>
        /// <param name="mapper">the mapper.</param>
        /// <param name="sapAlmacenService">the sap almacen service.</param>
        public SapAlmacenFacade(IMapper mapper, ISapAlmacenService sapAlmacenService)
        {
            this.mapper = mapper;
            this.almacenService = sapAlmacenService ?? throw new ArgumentNullException(nameof(sapAlmacenService));
        }

        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetOrders(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.almacenService.GetOrders(parameters));
        }
    }
}
