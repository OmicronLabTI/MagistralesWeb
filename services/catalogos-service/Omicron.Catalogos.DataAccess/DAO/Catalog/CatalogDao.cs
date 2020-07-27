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
    }
}
