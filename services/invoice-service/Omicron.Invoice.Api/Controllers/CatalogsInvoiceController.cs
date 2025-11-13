// <summary>
// <copyright file="CatalogsInvoiceController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Api.Controllers
{
    /// <summary>
    /// CatalogsInvoiceController class.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CatalogsInvoiceController"/> class.
    /// </remarks>
    /// <param name="catalogsInvoiceFacade">Catalogs Invoice Controller.</param>
    [Route("/")]
    [ApiController]
    public class CatalogsInvoiceController(ICatalogsInvoiceFacade catalogsInvoiceFacade)
        : ControllerBase
    {
        private readonly ICatalogsInvoiceFacade catalogsInvoiceFacade = catalogsInvoiceFacade ?? throw new ArgumentNullException(nameof(catalogsInvoiceFacade));

        /// <summary>
        /// Asynchronously retrieves classification data based on the provided parameters.
        /// </summary>
        /// <returns>A <see cref="Task{ResultDto}"/> contains invoice errors. </returns>
        [HttpPost]
        [Route("invoice/errors")]
        public async Task<IActionResult> InvoiceErrorsFromExcel()
            => this.Ok(await this.catalogsInvoiceFacade.InvoiceErrorsFromExcel());
    }
}
