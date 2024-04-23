// <summary>
// <copyright file="InventoryTransferRequestController.cs" company="Axity">
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
    public class InventoryTransferRequestController : ControllerBase
    {
        private readonly IInventoryTransferRequestFacade inventoryTransferRequestFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryTransferRequestController"/> class.
        /// </summary>
        /// <param name="inventoryTransferRequestFacade">Inventory Transfer Request Facade.</param>
        public InventoryTransferRequestController(IInventoryTransferRequestFacade inventoryTransferRequestFacade)
            => this.inventoryTransferRequestFacade = inventoryTransferRequestFacade ?? throw new ArgumentNullException(nameof(inventoryTransferRequestFacade));

        /// <summary>
        /// Create transfer request.
        /// </summary>
        /// <param name="transferRequestHeader">Transfer Request Header.</param>
        /// <returns>Result.</returns>
        [HttpPost("/create/transferrequest")]
        public async Task<IActionResult> CreateTransferRequest([FromBody] List<TransferRequestHeaderDto> transferRequestHeader)
            => this.Ok(await this.inventoryTransferRequestFacade.CreateTransferRequest(transferRequestHeader));

    }
}
