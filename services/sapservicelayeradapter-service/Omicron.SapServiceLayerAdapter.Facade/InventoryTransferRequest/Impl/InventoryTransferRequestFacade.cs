// <summary>
// <copyright file="InventoryTransferRequestFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.InventoryTransferRequest.Impl
{
    /// <summary>
    /// Class for inventory transfer request facade.
    /// </summary>
    public class InventoryTransferRequestFacade : IInventoryTransferRequestFacade
    {
        private readonly IMapper mapper;
        private readonly IInventoryTransferRequestService inventoryTransferRequestService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryTransferRequestFacade"/> class.
        /// </summary>
        /// <param name="mapper">Mapper</param>
        /// <param name="inventoryTransferRequestService">Inventory Transfer Request Service.</param>
        public InventoryTransferRequestFacade(IMapper mapper, IInventoryTransferRequestService inventoryTransferRequestService)
        {
            this.mapper = mapper;
            this.inventoryTransferRequestService = inventoryTransferRequestService.ThrowIfNull(nameof(inventoryTransferRequestService));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateTransferRequest(List<TransferRequestHeaderDto> transferRequestHeader)
            => this.mapper.Map<ResultDto>(await this.inventoryTransferRequestService.CreateTransferRequest(transferRequestHeader));
    }
}
