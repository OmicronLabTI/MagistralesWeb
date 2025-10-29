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
        /// <param name="usersFacade">User Facade.</param>
        public InvoiceController(IInvoiceFacade usersFacade)
            => this.invoiceFacade = usersFacade ?? throw new ArgumentNullException(nameof(usersFacade));

        /// <summary>
        /// Method for get all users.
        /// </summary>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("/sample/getallasync")]
        public async Task<IActionResult> GetAllAsync()
            => this.Ok(await this.invoiceFacade.GetAllAsync());

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
