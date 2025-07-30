// <summary>
// <copyright file="SeparateProductionOrderCommand.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.MediatR.Commands
{
    using global::MediatR;

    /// <summary>
    /// SeparateProductionOrderCommand.
    /// </summary>
    public class SeparateProductionOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeparateProductionOrderCommand"/> class.
        /// </summary>
        /// <param name="productionOrderId">Production Order Id.</param>
        /// <param name="pieces">Pieces.</param>
        /// <param name="separationId">SeparationId.</param>
        public SeparateProductionOrderCommand(int productionOrderId, int pieces, string separationId)
        {
            this.ProductionOrderId = productionOrderId;
            this.Pieces = pieces;
            this.SeparationId = separationId;
        }

        /// <summary>
        /// Gets or sets productionOrderId.
        /// </summary>
        /// <value>
        /// ProductionOrderId.
        /// </value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Pieces.
        /// </summary>
        /// <value>
        /// Pieces.
        /// </value>
        public int Pieces { get; set; }

        /// <summary>
        /// Gets or sets SeparationId.
        /// </summary>
        /// <value>
        /// SeparationId.
        /// </value>
        public string SeparationId { get; set; }

        /// <summary>
        /// Gets or sets RetryCount.
        /// </summary>
        /// <value>
        /// RetryCount.
        /// </value>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets MaxRetries.
        /// </summary>
        /// <value>
        /// MaxRetries.
        /// </value>
        public int MaxRetries { get; set; } = 3;
    }
}
