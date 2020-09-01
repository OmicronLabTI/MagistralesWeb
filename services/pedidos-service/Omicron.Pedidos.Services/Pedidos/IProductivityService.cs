// <summary>
// <copyright file="IProductivityService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// the interface for the productivity services.
    /// </summary>
    public interface IProductivityService
    {
        /// <summary>
        /// Gets the productivity by users.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetProductivityData(Dictionary<string, string> parameters);
    }
}
