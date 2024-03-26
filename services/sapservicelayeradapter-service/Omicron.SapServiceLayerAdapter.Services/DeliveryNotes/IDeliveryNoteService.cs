// <summary>
// <copyright file="IDeliveryNoteService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.DeliveryNotes
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes;

    /// <summary>
    /// Interface for delivery.
    /// </summary>
    public interface IDeliveryNoteService
    {
        /// <summary>
        /// Creates the delivery.
        /// </summary>
        /// <param name="createDelivery">the objects to create.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateDelivery(List<CreateDeliveryNoteDto> createDelivery);

        /// <summary>
        /// Creates the delivery partial.
        /// </summary>
        /// <param name="createDelivery">the objects to create.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateDeliveryPartial(List<CreateDeliveryNoteDto> createDelivery);

        /// <summary>
        /// Creates the delivery batch.
        /// </summary>
        /// <param name="createDelivery">the objects to create.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> CreateDeliveryBatch(List<CreateDeliveryNoteDto> createDelivery);
    }
}