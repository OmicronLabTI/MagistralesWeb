// <summary>
// <copyright file="RetryFailedProductionOrderFinalizationModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Entities.Model
{
    /// <summary>
    /// RetryFailedProductionOrderFinalizationModel.
    /// </summary>
    public class RetryFailedProductionOrderFinalizationModel
    {
        /// <summary>
        /// Gets or sets BatchProcessId.
        /// </summary>
        /// <value>
        /// List BatchProcessId.
        /// </value>
        public string BatchProcessId { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderProcessingPayload.
        /// </summary>
        /// <value>
        /// List ProductionOrderProcessingPayload.
        /// </value>
        public List<ProductionOrderProcessingStatusModel> ProductionOrderProcessingPayload { get; set; }
    }
}
