// <summary>
// <copyright file="InvoiceController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
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
        /// Update tracking process.
        /// </summary>
        /// <param name="invoiceId">Invoice id.</param>
        /// <param name="packageInformationSend">Package Information Send.</param>
        /// <returns>Result.</returns>
        [HttpPatch("/invoice/tracking/{invoiceId}")]
        public async Task<IActionResult> UpdateInvoiceTrackingInfo(int invoiceId, [FromBody] TrackingInformationDto packageInformationSend)
            => this.Ok(await this.invoiceFacade.UpdateInvoiceTrackingInfo(invoiceId, packageInformationSend));

        /// <summary>
        /// Update tracking process.
        /// </summary>
        /// <param name="deliveriesId">Package Information Send.</param>
        /// <returns>Result.</returns>
        [HttpPost("/invoice/delivery")]
        public async Task<IActionResult> CreateInvoiceByDeliveries([FromBody] List<int> deliveriesId)
            => this.Ok(await this.invoiceFacade.CreateInvoiceByDeliveries(deliveriesId));

        /// <summary>
        /// CreateInvoice.
        /// </summary>
        /// <param name="createInvoiceDocumentInfo">Invoice Info to create.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("/create/invoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDocumentDto createInvoiceDocumentInfo)
            => this.Ok(await this.invoiceFacade.CreateInvoice(createInvoiceDocumentInfo));
    }
}
