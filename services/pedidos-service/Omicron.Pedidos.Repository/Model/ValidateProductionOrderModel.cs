// <summary>
// <copyright file="ValidateProductionOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for Validations To Finalize Production Orders Result Dto.
    /// </summary>
    public class ValidateProductionOrderModel
    {
        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>The ProductionOrderId.</value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets ProcessId.
        /// </summary>
        /// <value>
        /// ProcessId.
        /// </value>
        public string ProcessId { get; set; }

        /// <summary>
        /// Gets or sets batches.
        /// </summary>
        /// <value>The order batches.</value>
        public List<BatchesConfigurationModel> Batches { get; set; }
    }
}
