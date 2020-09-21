// <summary>
// <copyright file="ICatalogService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Catalogs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Interface for the catalogServicer.
    /// </summary>
    public interface ICatalogService
    {
        /// <summary>
        /// Gets all the roles.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultModel> GetRoles();

        /// <summary>
        /// The values in the dictionary.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetParamsContains(Dictionary<string, string> parameters);
    }
}
