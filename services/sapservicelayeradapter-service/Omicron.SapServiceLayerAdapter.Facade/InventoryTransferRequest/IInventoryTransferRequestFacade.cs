// <summary>
// <copyright file="IInventoryTransferRequestFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Facade.InventoryTransferRequest
{
    /// <summary>
    /// Interface for inventory transfer request facade.
    /// </summary>
    public interface IInventoryTransferRequestFacade
    {
        /// <summary>
        /// Create transfer request.
        /// </summary>
        /// <param name="transferRequestHeader">Transfer request detail.</param>
        /// <returns>Result model data.</returns>
        Task<ResultDto> CreateTransferRequest(List<TransferRequestHeaderDto> transferRequestHeader);
    }
}
