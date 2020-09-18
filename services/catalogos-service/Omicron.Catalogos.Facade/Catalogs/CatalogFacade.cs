// <summary>
// <copyright file="CatalogFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Facade.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Catalogos.Dtos.Models;
    using Omicron.Catalogos.Services.Catalogs;

    /// <summary>
    /// Class for the catalog facade.
    /// </summary>
    public class CatalogFacade : ICatalogFacade
    {
        /// <summary>
        /// Mapper Object.
        /// </summary>
        private readonly IMapper mapper;

        private readonly ICatalogService catalogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogFacade"/> class.
        /// </summary>
        /// <param name="catalogService">the catalog.</param>
        /// <param name="mapper">the mapper.</param>
        public CatalogFacade(ICatalogService catalogService, IMapper mapper)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all the roles.
        /// </summary>
        /// <returns>Gets the roles.</returns>
        public async Task<ResultDto> GetRoles()
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.GetRoles());
        }

        /// <summary>
        /// Gets the values from parameters based in the dict.
        /// </summary>
        /// <param name="parameters">the dictionary.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetParamsContains(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.GetParamsContains(parameters));
        }
    }
}
