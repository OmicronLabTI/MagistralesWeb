// <summary>
// <copyright file="IInventoryTransferRequestService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.InventoryTransferRequest
{
    /// <summary>
    /// Interface for inventory transfer request service.
    /// </summary>
    public interface IInventoryTransferRequestService
    {
        /// <summary>
        /// Create transfer request.
        /// </summary>
        /// <param name="transferRequestHeader">Transfer request detail.</param>
        /// <returns>Result model data.</returns>
        Task<ResultModel> CreateTransferRequest(List<TransferRequestHeaderDto> transferRequestHeader);
    }
}
