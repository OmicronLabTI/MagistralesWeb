// <summary>
// <copyright file="IInvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.Users
{
    /// <summary>
    /// Interface IInvoiceFacade.
    /// </summary>
    public interface IInvoiceFacade
    {
        /// <summary>
        /// Method to Create invoice.
        /// </summary>
        /// <param name="request"> request invoice. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> CreateInvoice(CreateInvoiceDto request);

        /// <summary>
        /// Method for get invoices.
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
        /// Method to Create invoice.
        /// </summary>
        /// <param name="remissions"> request invoice. </param>
        /// <returns> order data. </returns>
        Task<ResultDto> GetInvoicesByRemissionId(List<int> remissions);

        /// <summary>
        /// Retrieves automatic billing data (AutoBilling) based on the provided filter parameters.
        /// </summary>
        /// <param name="parameters">
        /// A collection of key-value pairs used to query automatic billing records.
        /// These parameters may include filters such as date range, user, billing mode, or SAP invoice ID.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="ResultDto"/> object with the automatic billing information
        /// formatted for grid visualization in the UI.
        /// </returns>
        Task<ResultDto> GetAutoBilling(Dictionary<string, string> parameters);
    }
}