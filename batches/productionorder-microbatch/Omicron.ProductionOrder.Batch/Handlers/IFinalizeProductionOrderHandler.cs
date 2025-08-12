// <summary>
// <copyright file="IFinalizeProductionOrderHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Batch.Handlers
{
    /// <summary>
    /// IFinalizeProductionOrderHandler.
    /// </summary>
    public interface IFinalizeProductionOrderHandler
    {
        /// <summary>
        /// Handle.
        /// </summary>
        /// <returns>Task.</returns>
        Task Handle();
    }
}
