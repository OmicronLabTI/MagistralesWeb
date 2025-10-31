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
    }
}
