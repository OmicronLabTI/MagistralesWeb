// <summary>
// <copyright file="ICatalogDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.DataAccess.DAO.Catalog
{
    using Omicron.Catalogos.Entities.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The catalog Dao
    /// </summary>
    public interface ICatalogDao
    {
        /// <summary>
        /// Get all the roles
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<RoleModel>> GetAllRoles();

        /// <summary>
        /// Looks the values by field.
        /// </summary>
        /// <param name="fields">the data to look.</param>
        /// <returns>the data to return.</returns>
        Task<IEnumerable<ParametersModel>> GetParamsByField(List<string> fields);
    }
}
