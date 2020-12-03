// <summary>
// <copyright file="ISapAlmacenFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// Interface for sapAlmacen.
    /// </summary>
    public interface ISapAlmacenFacade
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetOrders(Dictionary<string, string> parameters);
    }
}
