// <summary>
// <copyright file="CancelProductionOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// CancelProductionOrderDto.
    /// </summary>
    public class CancelProductionOrderDto
    {
        /// <summary>
        /// Gets or sets productionOrderId.
        /// </summary>
        /// <value>
        /// ProductionOrderId.
        /// </value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets SeparationId.
        /// </summary>
        /// <value>
        /// SeparationId.
        /// </value>
        public string SeparationId { get; set; }

        /// <summary>
        /// Gets or sets the last step executed in the process.
        /// </summary>
        /// <value>The LastStep.</value>
        public string LastStep { get; set; }

        /// <summary>
        /// Gets or sets the error message if the process failed.
        /// </summary>
        /// <value>The ErrorMessage.</value>
        public string ErrorMessage { get; set; }
    }
}
