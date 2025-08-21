// <summary>
// <copyright file="CancelProductionOrderCommand.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.MediatR.Commands
{
    using global::MediatR;

    /// <summary>
    /// CancelProductionOrderCommand.
    /// </summary>
    public class CancelProductionOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelProductionOrderCommand"/> class.
        /// </summary>
        /// <param name="productionOrderId">Production Order Id.</param>
        /// <param name="pieces">Pieces.</param>
        /// <param name="separationId">SeparationId.</param>
        /// <param name="userId">userId.</param>
        /// <param name="dxpOrder">dxpOrder.</param>
        /// <param name="sapOrder">sapOrder.</param>
        /// <param name="totalPieces">totalPieces.</param>
        public CancelProductionOrderCommand(
            int productionOrderId,
            int pieces,
            string separationId,
            string userId,
            string dxpOrder,
            int? sapOrder,
            int totalPieces)
        {
            this.ProductionOrderId = productionOrderId;
            this.Pieces = pieces;
            this.SeparationId = separationId;
            this.UserId = userId;
            this.DxpOrder = dxpOrder;
            this.SapOrder = sapOrder;
            this.TotalPieces = totalPieces;
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

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        /// <value>
        /// UserId.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets DxpOrder.
        /// </summary>
        /// <value>
        /// DxpOrder.
        /// </value>
        public string DxpOrder { get; set; }

        /// <summary>
        /// Gets or sets SapOrder.
        /// </summary>
        /// <value>
        /// SapOrder.
        /// </value>
        public int? SapOrder { get; set; }

        /// <summary>
        /// Gets or sets TotalPieces.
        /// </summary>
        /// <value>
        /// TotalPieces.
        /// </value>
        public int TotalPieces { get; set; }
    }
}
