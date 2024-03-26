// <summary>
// <copyright file="DeliveryNoteFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.DeliveryNotes.Impl
{
    using Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes;
    using Omicron.SapServiceLayerAdapter.Services.DeliveryNotes;

    /// <summary>
    /// Class for Invoice Facade.
    /// </summary>
    public class DeliveryNoteFacade : IDeliveryNoteFacade
    {
        private readonly IMapper mapper;
        private readonly IDeliveryNoteService deliveryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryNoteFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="deliveryService">Delivery Service.</param>
        public DeliveryNoteFacade(IMapper mapper, IDeliveryNoteService deliveryService)
        {
            this.mapper = mapper;
            this.deliveryService = deliveryService.ThrowIfNull(nameof(deliveryService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CancelDelivery(string type, List<CancelDeliveryDto> deliveryNotesToCancel)
            => this.mapper.Map<ResultDto>(await this.deliveryService.CancelDelivery(type, deliveryNotesToCancel));

        /// <inheritdoc/>
        public async Task<ResultDto> CreateDelivery(List<CreateDeliveryNoteDto> createDelivery)
            => this.mapper.Map<ResultDto>(await this.deliveryService.CreateDelivery(createDelivery));

        /// <inheritdoc/>
        public async Task<ResultDto> CreateDeliveryPartial(List<CreateDeliveryNoteDto> createDelivery)
            => this.mapper.Map<ResultDto>(await this.deliveryService.CreateDeliveryPartial(createDelivery));
    }
}
