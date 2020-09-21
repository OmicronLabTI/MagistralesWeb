// <summary>
// <copyright file="CatalogService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Omicron.Catalogos.DataAccess.DAO.Catalog;
    using Omicron.Catalogos.Entities.Model;
    using Omicron.Catalogos.Services.Utils;

    /// <summary>
    /// The class for the catalog service.
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogDao catalogDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogService"/> class.
        /// </summary>
        /// <param name="catalogDao">the catalog dao.</param>
        public CatalogService(ICatalogDao catalogDao)
        {
            this.catalogDao = catalogDao ?? throw new ArgumentNullException(nameof(catalogDao));
        }

        /// <summary>
        /// Gets all the roles.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> GetRoles()
        {
            var listRoles = await this.catalogDao.GetAllRoles();

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, listRoles, null);
        }

        /// <summary>
        /// The values in the dictionary.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetParamsContains(Dictionary<string, string> parameters)
        {
            var dictKeys = parameters.Keys.ToList();
            var dataParams = (await this.catalogDao.GetParamsByField(dictKeys)).DistinctBy(x => x.Id).ToList();
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, dataParams, null);
        }
    }
}
