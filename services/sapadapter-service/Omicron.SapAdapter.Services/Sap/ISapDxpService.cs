// <summary>
// <copyright file="ISapDxpService.cs" company="Axity">
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
    /// The interface for sap.
    /// </summary>
    public interface ISapDxpService
    {
        /// <summary>
        /// Method to get orders active.
        /// </summary>
        /// <param name="ordersid">The parameters.</param>
        /// <returns>List of orders.</returns>
        Task<ResultModel> GetOrdersActive(List<int> ordersid);
    }
}
