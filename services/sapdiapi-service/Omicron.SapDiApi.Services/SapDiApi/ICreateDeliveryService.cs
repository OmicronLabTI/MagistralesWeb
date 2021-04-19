// <summary>
// <copyright file="ICreateDeliveryService.cs" company="Axity">
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
    /// Interface for delivery.
    /// </summary>
    public interface ICreateDeliveryService
    {
        /// <summary>
        /// Creates the delivery.
        /// </summary>
        /// <param name="createDelivery">the objects to create.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateDelivery(List<CreateDeliveryModel> createDelivery);

        /// <summary>
        /// Creates the delivery.
        /// </summary>
        /// <param name="createDelivery">the objects to create.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateDeliveryPartial(List<CreateDeliveryModel> createDelivery);

        /// <summary>
        /// Creates the delivery by multiple sales.
        /// </summary>
        /// <param name="createDelivery">the delivery.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateDeliveryBatch(List<CreateDeliveryModel> createDelivery);
    }
}
