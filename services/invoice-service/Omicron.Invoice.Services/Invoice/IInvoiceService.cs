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
        Task<ResultDto> UpdateManualChange(string id);

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
    }
}