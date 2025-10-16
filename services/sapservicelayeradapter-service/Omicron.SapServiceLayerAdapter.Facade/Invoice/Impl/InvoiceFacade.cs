// <summary>
// <copyright file="InvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.Invoice.Impl
{
    /// <summary>
    /// Class for Invoice Facade.
    /// </summary>
    public class InvoiceFacade : IInvoiceFacade
    {
        private readonly IMapper mapper;
        private readonly IInvoiceService invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="invoiceService">Invoice Service.</param>
        public InvoiceFacade(IMapper mapper, IInvoiceService invoiceService)
        {
            this.mapper = mapper;
            this.invoiceService = invoiceService.ThrowIfNull(nameof(invoiceService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateInvoice(CreateInvoiceDocumentDto createInvoiceDocumentInfo)
            => this.mapper.Map<ResultDto>(await this.invoiceService.CreateInvoice(createInvoiceDocumentInfo));

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateInvoiceTrackingInfo(int invoiceId, TrackingInformationDto packageInformationSend)
            => this.mapper.Map<ResultDto>(await this.invoiceService.UpdateInvoiceTrackingInfo(invoiceId, packageInformationSend));
    }
}
