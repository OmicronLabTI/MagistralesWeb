// <summary>
// <copyright file="SeparateOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// SeparateOrderDto.
    /// </summary>
    public class SeparateOrderDto
    {
        /// <summary>
        /// Gets or sets BatchProcessId.
        /// </summary>
        /// <value>
        /// List BatchProcessId.
        /// </value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderProcessingPayload.
        /// </summary>
        /// <value>
        /// List ProductionOrderProcessingPayload.
        /// </value>
        public int Pieces { get; set; }
    }
}
