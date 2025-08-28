// <summary>
// <copyright file="CreateChildProductionOrdersDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// the order.
    /// </summary>
    public class CreateChildProductionOrdersDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Pieces { get; set; }

        /// <summary>
        /// Gets or sets lastStep.
        /// </summary>
        /// <value>
        /// lastStep.
        /// </value>
        public string LastStep { get; set; }

        /// <summary>
        /// Gets or sets productionOrderId.
        /// </summary>
        /// <value>
        /// ProductionOrderId.
        /// </value>
        public int ProductionOrderChildId { get; set; }
    }
}