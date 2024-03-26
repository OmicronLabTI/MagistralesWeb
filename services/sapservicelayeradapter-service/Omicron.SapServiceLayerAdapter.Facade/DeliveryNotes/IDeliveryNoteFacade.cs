// <summary>
// <copyright file="IDeliveryNoteFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.DeliveryNotes
{
    using Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes;
    using Omicron.SapServiceLayerAdapter.Model;

    /// <summary>
    /// Interface for Delivery Note Facade.
    /// </summary>
    public interface IDeliveryNoteFacade
    {
        /// <summary>
        /// Method to create delivery notes to order.
        /// </summary>
        /// <param name="createDelivery">Invoice Id.</param>
        /// <returns>Create delivery notes.</returns>
        Task<ResultDto> CreateDelivery(List<CreateDeliveryNoteDto> createDelivery);

        /// <summary>
        /// Method to create delivery notes partial to order.
        /// </summary>
        /// <param name="createDelivery">Invoice Id.</param>
        /// <returns>Create delivery notes.</returns>
        Task<ResultDto> CreateDeliveryPartial(List<CreateDeliveryNoteDto> createDelivery);

        /// <summary>
        /// Method to create delivery notes batch to order.
        /// </summary>
        /// <param name="createDelivery">Invoice Id.</param>
        /// <returns>Create delivery notes.</returns>
        Task<ResultDto> CreateDeliveryBatch(List<CreateDeliveryNoteDto> createDelivery);
    }
}