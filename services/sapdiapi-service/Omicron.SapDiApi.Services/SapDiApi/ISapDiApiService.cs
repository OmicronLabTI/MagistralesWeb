// <summary>
// <copyright file="ISapDiApiService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using Omicron.SapDiApi.Entities.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// the interface.
    /// </summary>
    public interface ISapDiApiService
    {
        /// <summary>
        /// insert the fab orders.
        /// </summary>
        /// <param name="orderWithDetail">the list of data.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> InsertOrdenFab(List<OrderWithDetailModel> orderWithDetail);

        /// <summary>
        /// Updates the fabrication orders.
        /// </summary>
        /// <param name="orderModels">the models to update.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> UpdateFabOrders(List<UpdateFabOrderModel> orderModels);

        /// <summary>
        /// Connects to SAP.
        /// </summary>
        /// <returns>the connection.</returns>
        Task<ResultModel> Connect();
    }
}
