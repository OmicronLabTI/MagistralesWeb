// <summary>
// <copyright file="IInvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Invoices
{
    /// <summary>
    /// Interface for Invoice Service.
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Method to get the last generated order.
        /// </summary>
        /// <param name="invoiceId">Invoice Id.</param>
        /// <param name="packageInformationSend">Package Information Send.</param>
        /// <returns>Last generated order.</returns>
        Task<ResultModel> UpdateInvoiceTrackingInfo(int invoiceId, TrackingInformationDto packageInformationSend);

        /// <summary>
        /// Method to Create Invoice On SAP.
        /// </summary>
        /// <param name="createInvoiceDocumentInfo">Create Invoice Document Info.</param>
        /// <returns>Last generated order.</returns>
        Task<ResultModel> CreateInvoice(CreateInvoiceDocumentDto createInvoiceDocumentInfo);
    }
}
