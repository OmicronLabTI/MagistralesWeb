// <summary>
// <copyright file="IInvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.Invoice
{
    /// <summary>
    /// Interface for Invoice Facade.
    /// </summary>
    public interface IInvoiceFacade
    {
        /// <summary>
        /// Method to get the last generated order.
        /// </summary>
        /// <param name="invoiceId">Invoice Id.</param>
        /// <param name="packageInformationSend">Package Information Send.</param>
        /// <returns>Last generated order.</returns>
        Task<ResultDto> UpdateInvoiceTrackingInfo(int invoiceId, TrackingInformationDto packageInformationSend);

        /// <summary>
        /// Method to get the last generated order.
        /// </summary>
        /// <param name="deliveriesId">Invoice Id.</param>
        /// <returns>Last generated order.</returns>
        Task<ResultDto> CreateInvoiceByDeliveries(List<int> deliveriesId);
    }
}
