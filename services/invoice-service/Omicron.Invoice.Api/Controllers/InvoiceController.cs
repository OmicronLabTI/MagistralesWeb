// <summary>
// <copyright file="InvoiceController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Api.Controllers
{
    /// <summary>
    /// InvoiceController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceFacade invoiceFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="invoiceFacade">Invoice Facade.</param>
        public InvoiceController(IInvoiceFacade invoiceFacade)
            => this.invoiceFacade = invoiceFacade ?? throw new ArgumentNullException(nameof(invoiceFacade));

        /// <summary>
        /// Method for get all users.
        /// </summary>
        /// <param name="request">the request.</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Route("/create")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto request)
            => this.Ok(await this.invoiceFacade.CreateInvoice(request));

        /// <summary>
        /// Method Ping.
        /// </summary>
        /// <returns>Pong.</returns>
        [Route("/ping")]
        [HttpGet]
        public IActionResult Ping()
            => this.Ok("Pong");
    }
}
