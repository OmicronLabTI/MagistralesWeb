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
        /// Retrieves all SAP order objects grouped by their associated invoice identifiers.
        /// </summary>
        /// <param name="invoiceIds">A list of invoice identifiers.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a dictionary where each key is an invoice ID and the value
        /// is a list of <see cref="InvoiceSapOrderModel"/> objects linked to that invoice.
        /// </returns>
        Task<Dictionary<string, List<InvoiceSapOrderModel>>> GetSapOrdersByInvoiceIdsAsync(List<string> invoiceIds);

        /// <summary>
        /// Retrieves automatic billing records (AutoBilling base data) filtered by status and paginated.
        /// </summary>
        /// <param name="status">A list of AutoBilling statuses used as filters.</param>
        /// <param name="offset">The starting position of the result set (for pagination).</param>
        /// <param name="limit">The maximum number of records to return.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="List{InvoiceModel}"/> with AutoBilling base data.
        /// </returns>
        Task<List<InvoiceModel>> GetAutoBillingBaseAsync(List<string> status, int offset, int limit);

        /// <summary>
        /// Retrieves all remission objects grouped by their associated invoice identifiers.
        /// </summary>
        /// <param name="invoiceIds">A list of invoice identifiers.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a dictionary where each key is an invoice ID and the value
        /// is a list of <see cref="InvoiceRemissionModel"/> objects linked to that invoice.
        /// </returns>
        Task<Dictionary<string, List<InvoiceRemissionModel>>> GetRemissionsByInvoiceIdsAsync(List<string> invoiceIds);

        /// <summary>
        /// Retrieves the total number of AutoBilling records that match the given status filters.
        /// </summary>
        /// <param name="status">A list of AutoBilling statuses used as filters.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the total count of AutoBilling records.
        /// </returns>
        Task<int> GetAutoBillingCountAsync(List<string> status);

        /// <summary>
        /// Get existing error IDs.
        /// </summary>
        /// <param name="codes">List of IDs to check.</param>
        /// <returns>List of existing IDs.</returns>
        Task<List<InvoiceErrorModel>> GetExistingErrorsByCodes(List<string> codes);

        /// <summary>
        /// Insert invoice errors from catalog.
        /// </summary>
        /// <param name="invoicErrors">List of errors to insert.</param>
        /// <returns>Task.</returns>
        Task InsertInvoiceErrors(List<InvoiceErrorModel> invoicErrors);

        /// <summary>
        /// Update invoice errors from catalog.
        /// </summary>
        /// <param name="invoiceErrors">List of errors to update.</param>
        /// <returns>Task.</returns>
        Task UpdateInvoiceErrors(List<InvoiceErrorModel> invoiceErrors);

        /// <summary>
        /// Update invoice errors from catalog.
        /// </summary>
        /// <param name="pedidosSap">List of pedidos sap.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task.</returns>
        Task<List<InvoiceModel>> GetInvoicesByPedidoSap(List<int> pedidosSap, int offset, int limit);

        /// <summary>
        /// Update invoice errors from catalog.
        /// </summary>
        /// <param name="pedidoDxp">pedido dxp.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task.</returns>
        Task<List<InvoiceModel>> GetInvoicesByPedidoDxp(string pedidoDxp, int offset, int limit);

        /// <summary>
        /// Update invoice errors from catalog.
        /// </summary>
        /// <param name="id">pedido dxp.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task.</returns>
        Task<List<InvoiceModel>> GetInvoicesByInvoiceId(string id, int offset, int limit);
    }
}
