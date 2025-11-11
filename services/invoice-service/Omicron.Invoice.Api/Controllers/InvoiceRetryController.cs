// <summary>
// <copyright file="InvoiceRetryController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Api.Controllers
{
    /// <summary>
    /// InvoiceRetryController class.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="InvoiceRetryController"/> class.
    /// </remarks>
    /// <param name="invoiceRetryFacade">Invoice Retry Facade.</param>
    [Route("/")]
    [ApiController]
    public class InvoiceRetryController(IInvoiceRetryFacade invoiceRetryFacade)
        : ControllerBase
    {
        private readonly IInvoiceRetryFacade invoiceRetryFacade = invoiceRetryFacade ?? throw new ArgumentNullException(nameof(invoiceRetryFacade));

        /// <summary>
        /// Method for GetDataToRetryCreateInvoicesAsync.
        /// </summary>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("data/automatic/retry/createinvoice")]
        public async Task<IActionResult> GetDataToRetryCreateInvoicesAsync()
            => this.Ok(await this.invoiceRetryFacade.GetDataToRetryCreateInvoicesAsync());

        /// <summary>
        /// Method for GetDataToRetryCreateInvoicesAsync.
        /// </summary>
        /// <param name="invoiceRetry">Invoice Retry.</param>
        /// <param name="executionType">Execution Type (Automatic - Manual).</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("retry/{executionType}/createinvoice")]
        public async Task<IActionResult> RetryCreateInvoicesAsync([FromBody] InvoiceRetryRequestDto invoiceRetry, string executionType)
            => this.Ok(await this.invoiceRetryFacade.RetryCreateInvoicesAsync(invoiceRetry, executionType));
    }
}