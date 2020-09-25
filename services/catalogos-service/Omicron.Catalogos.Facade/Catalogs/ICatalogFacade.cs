// <summary>
// <copyright file="ICatalogFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Facade.Catalogs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Catalogos.Dtos.Models;

    /// <summary>
    /// The interface for catalogs.
    /// </summary>
    public interface ICatalogFacade
    {
        /// <summary>
        /// Get roles.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetRoles();

        /// <summary>
        /// Gets the values from parameters based in the dict.
        /// </summary>
        /// <param name="parameters">the dictionary.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetParamsContains(Dictionary<string, string> parameters);
    }
}
