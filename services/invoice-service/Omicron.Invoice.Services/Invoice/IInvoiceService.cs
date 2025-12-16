// <summary>
// <copyright file="IInvoiceService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Invoice
{
    /// <summary>
    /// Interface IProjectService.
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Method to carry out the order process.
        /// </summary>
        /// <param name="request"> request order. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> RegisterInvoice(CreateInvoiceDto request);

        /// <summary>
        /// Method to carry out the order process.
        /// </summary>
        /// <param name="request"> request order. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> CreateInvoice(CreateInvoiceDto request);

        /// <summary>
        /// Method for get all users.
        /// </summary>
        /// <param name="parameters">parameters.</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetInvoices(Dictionary<string, string> parameters);

        /// <summary>
        /// Method for update colum manualchangeapplied.
        /// </summary>
        /// <param name="id">id invoice.</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        Task<ResultDto> UpdateManualChange(UpdateManualChangeDto id);

        /// <summary>
        /// Method to carry out the order process.
        /// </summary>
        /// <param name="request"> request order. </param>
        /// <returns> order data. </returns>
        bool PublishProcessToMediatR(CreateInvoiceDto request);

        /// <summary>
        /// Method to Create invoice.
        /// </summary>
        /// <param name="remissions"> request invoice. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> GetInvoicesByRemissionId(List<int> remissions);

        /// <summary>
        /// Method for get all users.
        /// </summary>
        /// <param name="parameters">parameters.</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetUninvoicedSapOrders(Dictionary<string, string> parameters);

        /// <summary>
        /// Retrieves automatic billing (AutoBilling) information based on the specified parameters.
        /// This method serves the Automatic Billing module, returning data formatted
        /// for grid display and including related invoice and SAP order counts.
        /// </summary>
        /// <param name="parameters">
        /// A dictionary containing key-value pairs for filtering AutoBilling records,
        /// such as status, pagination offsets, date range, or user identifiers.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="ResultDto"/> object with automatic billing data.
        /// </returns>
        Task<ResultDto> GetAutoBillingAsync(Dictionary<string, string> parameters);
    }
}