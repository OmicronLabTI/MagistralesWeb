// <summary>
// <copyright file="ISapAlmacenDeliveryService.cs" company="Axity">
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
    /// Interface for the almacen delivery.
    /// </summary>
    public interface ISapAlmacenDeliveryService
    {
        /// <summary>
        /// Gets the deliveries to return.
        /// </summary>
        /// <param name="parameters">the parameters to look.</param>
        /// <returns>The data.</returns>
        Task<ResultModel> GetDelivery(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the products by sales.
        /// </summary>
        /// <param name="saleId">the sale ids.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetProductsDelivery(string saleId);
    }
}
