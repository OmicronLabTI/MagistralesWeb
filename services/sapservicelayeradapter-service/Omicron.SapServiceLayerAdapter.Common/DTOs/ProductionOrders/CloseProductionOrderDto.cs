// <summary>
// <copyright file="CloseProductionOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// Close production order model.
    /// </summary>
    public class CloseProductionOrderDto
    {
        /// <summary>
        /// Gets or sets ProcessId.
        /// </summary>
        /// <value>
        /// ProcessId.
        /// </value>
        public string ProcessId { get; set; }

        /// <summary>
        /// Gets or sets ProcessId.
        /// </summary>
        /// <value>
        /// ProcessId.
        /// </value>
        public string LastStep { get; set; }

        /// <summary>
        /// Gets or sets FinalizeProductionOrder.
        /// </summary>
        /// <value>The FinalizeProductionOrder.</value>
        public FinalizeProductionOrderDto FinalizeProductionOrder { get; set; }
    }
}