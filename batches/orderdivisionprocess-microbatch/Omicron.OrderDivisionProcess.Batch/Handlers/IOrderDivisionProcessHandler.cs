// <summary>
// <copyright file="IOrderDivisionProcessHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Batch.Handlers
{
    /// <summary>
    /// IOrderDivisionProcessHandler.
    /// </summary>
    public interface IOrderDivisionProcessHandler
    {
        /// <summary>
        /// Handle.
        /// </summary>
        /// <returns>Task.</returns>
        Task Handle();

    }
}
