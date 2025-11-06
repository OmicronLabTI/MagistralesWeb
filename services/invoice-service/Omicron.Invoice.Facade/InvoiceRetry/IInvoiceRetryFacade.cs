// <summary>
// <copyright file="IInvoiceRetryFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.InvoiceRetry
{
    /// <summary>
    /// Interface IInvoiceRetryFacade.
    /// </summary>
    public interface IInvoiceRetryFacade
    {
        /// <summary>
        /// Get Data To Retry Create Invoices Async.
        /// </summary>
        /// <returns>A <see cref="Task{ResultDto}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> GetDataToRetryCreateInvoicesAsync();

        /// <summary>
        /// Retries invoice creation while controlling concurrency between manual and automatic processes.
        /// </summary>
        /// <param name="invoiceRetry">Invoice Retry Request.</param>
        /// <param name="executionType">Execution Type (Automatic - Manual).</param>
        /// <returns>A <see cref="Task{ResultDto}"/> representing the result of the asynchronous operation.</returns>
        Task<ResultDto> RetryCreateInvoicesAsync(InvoiceRetryRequestDto invoiceRetry, string executionType);
    }
}
