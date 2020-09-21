// <summary>
// <copyright file="CatalogDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.DataAccess.DAO.Catalog
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Catalogos.Entities.Context;
    using Omicron.Catalogos.Entities.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The catalogDao.
    /// </summary>
    public class CatalogDao : ICatalogDao
    {
        private readonly IDatabaseContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogDao"/> class.
        /// </summary>
        /// <param name="databaseContext">the database context.</param>
        public CatalogDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <summary>
        /// GEts all the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        public async Task<IEnumerable<RoleModel>> GetAllRoles()
        {
            return await this.databaseContext.RoleModel.ToListAsync();
        }

        /// <summary>
        /// Looks the values by field.
        /// </summary>
        /// <param name="fields">the data to look.</param>
        /// <returns>the data to return.</returns>
        public async Task<IEnumerable<ParametersModel>> GetParamsByField(List<string> fields)
        {
            var listParameters = new List<ParametersModel>();

            foreach (var field in fields)
            {
                var result = await this.databaseContext.ParametersModel.Where(x => x.Field.Contains(field)).ToListAsync();
                listParameters.AddRange(result);
            }

            return listParameters;
        }
    }
}
