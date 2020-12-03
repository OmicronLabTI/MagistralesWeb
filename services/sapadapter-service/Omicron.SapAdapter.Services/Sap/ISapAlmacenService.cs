// <summary>
// <copyright file="ISapAlmacenService.cs" company="Axity">
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
    /// interface for sap almacen service.
    /// </summary>
    public interface ISapAlmacenService
    {
        /// <summary>
        /// Gets the orders for almacen.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the data of the magistral scanned data.
        /// </summary>
        /// <param name="code">the code.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetMagistralScannedData(string code);

        /// <summary>
        /// Gets the data of the line scanned bar.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetLineScannedData(string code);
    }
}
