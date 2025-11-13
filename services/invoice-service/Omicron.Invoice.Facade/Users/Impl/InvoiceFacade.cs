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
        public async Task<ResultDto> GetInvoices(Dictionary<string, string> parameters)
            => await this.invoiceService.GetInvoices(parameters);

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateManualChange(UpdateManualChangeDto id)
            => await this.invoiceService.UpdateManualChange(id);

        /// <inheritdoc/>
        public async Task<ResultDto> GetInvoicesByRemissionId(List<int> remissions)
        {
            return await this.invoiceService.GetInvoicesByRemissionId(remissions);
        }

        /// <summary>
        /// Retrieves automatic billing data (AutoBilling) by invoking the corresponding service layer method.
        /// </summary>
        /// <param name="parameters">
        /// A collection of key-value pairs used to query automatic billing records.
        /// These may include filters such as date range, user, billing mode, or SAP invoice ID.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="ResultDto"/> object with the automatic billing data
        /// formatted for visualization in the frontend grid.
        /// </returns>
        public async Task<ResultDto> GetAutoBilling(Dictionary<string, string> parameters)
            => await this.invoiceService.GetAutoBillingAsync(parameters);
    }
}