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
        /// Method for GetInvoices.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<InvoiceModel>> GetInvoicesNotCreatedByStatus(List<string> status, int offset, int limit);

        /// <summary>
        /// Method for GetInvoicesCount.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> GetInvoicesCount(List<string> status);

        /// <summary>
        /// Method for GetInvoicesCount.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<InvoiceModel>> GetInvoicesById(List<string> id);

        /// <summary>
        /// Get invoice.
        /// </summary>
        /// <param name="id">the id.</param>
        /// <returns>the data.</returns>
        Task<InvoiceModel> GetInvoiceById(string id);

        /// <summary>
        /// Method for SaveChangesAsync.
        /// Get invoice.
        /// </summary>
        /// <returns>the SaveChangesAsync.</returns>
        Task<int> SaveChangesAsync();

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

        /// <summary>
        /// Method for GetInvoicesByRemissionId.
        /// </summary>
        /// <param name="remissionId">the ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<InvoiceRemissionJoinModel>> GetInvoicesByRemissionId(List<long> remissionId);

        /// <summary>
        /// Get existing error IDs.
        /// </summary>
        /// <param name="ids">List of IDs to check.</param>
        /// <returns>List of existing IDs.</returns>
        Task<List<int>> GetExistingErrorIds(List<int> ids);

        /// <summary>
        /// Insert invoice errors from catalog.
        /// </summary>
        /// <param name="invoicErrors">List of errors to insert.</param>
        /// <returns>Task.</returns>
        Task InsertInvoiceErrors(List<InvoiceErrorModel> invoicErrors);

        /// <summary>
        /// Update invoice errors from catalog.
        /// </summary>
        /// <param name="invoicErrors">List of errors to update.</param>
        /// <returns>Task.</returns>
        Task UpdateInvoiceErrors(List<InvoiceErrorModel> invoicErrors);
    }
}
