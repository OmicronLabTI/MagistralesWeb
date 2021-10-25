// <summary>
// <copyright file="IComponentsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;

    /// <summary>
    /// interface for componentsService.
    /// </summary>
    public interface IComponentsService
    {
        /// <summary>
        /// Get most common components.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the dta.</returns>
        Task<ResultModel> GetMostCommonComponents(Dictionary<string, string> parameters);
    }
}
