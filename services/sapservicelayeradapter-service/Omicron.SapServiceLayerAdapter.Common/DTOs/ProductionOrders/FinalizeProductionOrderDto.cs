// <summary>
// <copyright file="FinalizeProductionOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    using System.Collections.Generic;

    /// <summary>
    /// Close production order model.
    /// </summary>
    public class FinalizeProductionOrderDto
    {
        /// <summary>
        /// Gets or sets user UserId.
        /// </summary>
        /// <value>The UserId.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>The ProductionOrderId.</value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets user SourceProcess.
        /// </summary>
        /// <value>The SourceProcess.</value>
        public string SourceProcess { get; set; }

        /// <summary>
        /// Gets or sets batches.
        /// </summary>
        /// <value>The order batches.</value>
        public List<BatchesConfigurationDto> Batches { get; set; }
    }
}
