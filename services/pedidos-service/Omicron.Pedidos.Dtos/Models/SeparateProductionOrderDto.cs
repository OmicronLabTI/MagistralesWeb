// <summary>
// <copyright file="SeparateProductionOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// SeparateProductionOrderDto.
    /// </summary>
    public class SeparateProductionOrderDto
    {
        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// List ProductionOrderId.
        /// </value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Pieces.
        /// </summary>
        /// <value>
        /// List Pieces.
        /// </value>
        public int Pieces { get; set; }
    }
}
