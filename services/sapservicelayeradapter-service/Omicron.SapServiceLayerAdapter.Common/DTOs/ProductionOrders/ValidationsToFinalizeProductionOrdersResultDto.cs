// <summary>
// <copyright file="ValidationsToFinalizeProductionOrdersResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// Class for Validations To Finalize Production Orders Result Dto.
    /// </summary>
    public class ValidationsToFinalizeProductionOrdersResultDto
    {
        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets LastStep.
        /// </summary>
        public string LastStep { get; set; }
    }
}
