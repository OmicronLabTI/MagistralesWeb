// <summary>
// <copyright file="CreateChildOrderResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// the order.
    /// </summary>
    public class CreateChildOrderResultDto
    {
        /// <summary>
        /// Gets or sets the Id of the created child production order.
        /// </summary>
        /// <value>The ProductionOrderChildId.</value>
        public int ProductionOrderChildId { get; set; }

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
