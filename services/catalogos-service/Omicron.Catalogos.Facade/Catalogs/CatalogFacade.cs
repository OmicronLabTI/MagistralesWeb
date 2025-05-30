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
    using Omicron.Catalogos.Dtos.User;
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

        /// <inheritdoc/>
        public async Task<ResultDto> GetActiveClassificationQfb()
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.GetActiveClassificationQfb());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UploadWarehouseFromExcel()
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.UploadWarehouseFromExcel());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetActivesWarehouses(List<ActiveWarehouseDto> products)
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.GetActivesWarehouses(products));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetClassifications()
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.GetClassifications());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UploadSortingRouteFromExcel()
        {
            return this.mapper.Map<ResultDto>(await this.catalogService.UploadSortingRouteFromExcel());
        }
    }
}
