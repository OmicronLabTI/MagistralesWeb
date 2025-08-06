// <summary>
// <copyright file="ValidationsToFinalizeProductionOrdersResultModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// Class for Validations To Finalize Production Orders Result Dto.
    /// </summary>
    public class ValidationsToFinalizeProductionOrdersResultModel
    {
        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// ProductionOrderId.
        /// </value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        /// <value>
        /// ErrorMessage.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets LastStep.
        /// </summary>
        /// <value>
        /// LastStep.
        /// </value>
        public string LastStep { get; set; }
    }
}
