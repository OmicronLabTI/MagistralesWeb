// <summary>
// <copyright file="IInvoiceDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.DAO.Invoice
{
    /// <summary>
    /// Interface IInvoiceDao.
    /// </summary>
    public interface IInvoiceDao
    {
        /// <summary>
        /// Method for GetInvoiceModelById.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<InvoiceModel> GetInvoiceModelById(string id);

        /// <summary>
        /// Method for UpdateInvoiceAsync.
        /// </summary>
        /// <param name="invoiceModel">The model.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateInvoiceAsync(InvoiceModel invoiceModel);

        /// <summary>
        /// Method for GetInvoicesForRetryProcessAsync.
        /// </summary>
        /// <param name="status">Status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<InvoiceModel>> GetInvoicesForRetryProcessAsync(string status);

        /// <summary>
        /// Method for GetAllAsync.
        /// Create invoice.
        /// </summary>
        /// <param name="invoices">the invoices.</param>
        /// <returns>the data.</returns>
        Task InsertInvoices(List<InvoiceModel> invoices);

        /// <summary>
        /// Get invoice.
        /// </summary>
        /// <param name="id">the id.</param>
        /// <returns>the data.</returns>
        Task<InvoiceModel> GetInvoiceById(string id);

        /// <summary>
        /// Update invoice.
        /// </summary>
        /// <param name="invoices">the id.</param>
        /// <returns>the data.</returns>
        Task UpdateInvoices(List<InvoiceModel> invoices);

        /// <summary>
        /// Method for GetInvoicesForRetryProcessAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<InvoiceErrorModel>> GetAllErrors();
    }
}
