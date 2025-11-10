// <summary>
// <copyright file="InvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.Users.Impl
{
    /// <summary>
    /// Class InvoiceFacade.
    /// </summary>
    public class InvoiceFacade : IInvoiceFacade
    {
        private readonly IInvoiceService invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceFacade"/> class.
        /// </summary>
        /// <param name="invoiceService">Interface InvoiceService.</param>
        public InvoiceFacade(IInvoiceService invoiceService) =>
            this.invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));

        /// <inheritdoc/>
        public async Task<ResultDto> CreateInvoice(CreateInvoiceDto request)
        {
            return await this.invoiceService.RegisterInvoice(request);
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetInvoicesByRemissionId(List<int> remissions)
        {
            return await this.invoiceService.GetInvoicesByRemissionId(remissions);
        }
    }
}