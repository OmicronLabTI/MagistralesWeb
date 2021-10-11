// <summary>
// <copyright file="ICancelService.cs" company="Axity">
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
    /// interface for cancel.
    /// </summary>
    public interface ICancelService
    {
        /// <summary>
        /// Cancels the delivery.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <param name="deliveryIds">the ids.</param>
        /// <returns>the dat.</returns>
        Task<ResultModel> CancelDelivery(string type, List<CancelDeliveryModel> deliveryIds);
    }
}
