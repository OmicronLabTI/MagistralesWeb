// <summary>
// <copyright file="CatalogsInvoiceFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.CatalogsInvoice.Impl
{
    /// <summary>
    /// Class CatalogsInvoiceFacade.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CatalogsInvoiceFacade"/> class.
    /// </remarks>
    /// <param name="catalogsInvoiceService">Interface catalogsInvoiceService.</param>
    public class CatalogsInvoiceFacade(ICatalogsInvoiceService catalogsInvoiceService)
        : ICatalogsInvoiceFacade
    {
        private readonly ICatalogsInvoiceService catalogsInvoiceService = catalogsInvoiceService ?? throw new ArgumentNullException(nameof(catalogsInvoiceService));

        /// <inheritdoc/>
        public async Task<ResultDto> InvoiceErrorsFromExcel()
            => await this.catalogsInvoiceService.InvoiceErrorsFromExcel();
    }
}
