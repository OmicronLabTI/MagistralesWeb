// <summary>
// <copyright file="InvoiceRetryFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.InvoiceRetry.Impl
{
    /// <summary>
    /// Class InvoiceRetryFacade.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="InvoiceRetryFacade"/> class.
    /// </remarks>
    /// <param name="invoiceRetryService">Interface InvoiceRetryService.</param>
    public class InvoiceRetryFacade(IInvoiceRetryService invoiceRetryService)
        : IInvoiceRetryFacade
    {
        private readonly IInvoiceRetryService invoiceRetryService = invoiceRetryService;

        /// <inheritdoc/>
        public async Task<ResultDto> GetDataToRetryCreateInvoicesAsync()
            => await this.invoiceRetryService.GetDataToRetryCreateInvoicesAsync();

        /// <inheritdoc/>
        public async Task<ResultDto> RetryCreateInvoicesAsync(InvoiceRetryRequestDto invoiceRetry, string executionType)
            => await this.invoiceRetryService.RetryCreateInvoicesAsync(invoiceRetry, executionType);
    }
}
