// <summary>
// <copyright file="DeliveryNoteController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
{
    /// <summary>
    /// DeliveryNoteController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryNoteController : ControllerBase
    {
        private readonly IDeliveryNoteFacade deliveryNoteFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryNoteController"/> class.
        /// </summary>
        /// <param name="deliveryNoteFacade">Delivery Note Facade.</param>
        public DeliveryNoteController(IDeliveryNoteFacade deliveryNoteFacade)
            => this.deliveryNoteFacade = deliveryNoteFacade ?? throw new ArgumentNullException(nameof(deliveryNoteFacade));

        /// <summary>
        /// Create delivery notes.
        /// </summary>
        /// <param name="createDelivery">The delivery notes.</param>
        /// <returns>Result.</returns>
        [Route("/complete")]
        [HttpPost]
        public async Task<IActionResult> CreateDelivery(List<CreateDeliveryNoteDto> createDelivery)
            => this.Ok(await this.deliveryNoteFacade.CreateDelivery(createDelivery));

        /// <summary>
        /// Create delivery notes partial.
        /// </summary>
        /// <param name="createDelivery">The delivery notes.</param>
        /// <returns>Result.</returns>
        [Route("/partial")]
        [HttpPost]
        public async Task<IActionResult> CreateDeliveryPartial(List<CreateDeliveryNoteDto> createDelivery)
            => this.Ok(await this.deliveryNoteFacade.CreateDeliveryPartial(createDelivery));

        /// <summary>
        /// Create delivery notes partial.
        /// </summary>
        /// <param name="createDelivery">The delivery notes.</param>
        /// <returns>Result.</returns>
        [Route("/batch")]
        [HttpPost]
        public async Task<IActionResult> CreateDeliveryBatch(List<CreateDeliveryNoteDto> createDelivery)
            => this.Ok(await this.deliveryNoteFacade.CreateDeliveryBatch(createDelivery));

        /// <summary>
        /// Update tracking process.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="deliveryNotesToCancel">Delivery Notes To Cancel.</param>
        /// <returns>Result.</returns>
        [HttpPost("/cancel/{type}/delivery")]
        public async Task<IActionResult> CancelDelivery(string type, [FromBody] List<CancelDeliveryDto> deliveryNotesToCancel)
            => this.Ok(await this.deliveryNoteFacade.CancelDelivery(type, deliveryNotesToCancel));
    }
}