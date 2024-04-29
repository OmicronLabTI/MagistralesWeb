// <summary>
// <copyright file="CreateIsolateProductionOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// the class ProductionOrderDto.
    /// </summary>
    public class CreateIsolateProductionOrderDto : BaseProductionOrder
    {
        /// <summary>
        /// Gets or sets the ProductionOrderType.
        /// </summary>
        /// <value>ProductionOrderType.</value>
        [JsonProperty("ProductionOrderType")]
        public string ProductionOrderType { get; set; }

        /// <summary>
        /// Gets or sets the DistributionRule.
        /// </summary>
        /// <value>DistributionRule.</value>
        [JsonProperty("DistributionRule")]
        public string DistributionRule { get; set; }

        /// <summary>
        /// Gets or sets the DistributionRule2.
        /// </summary>
        /// <value>DistributionRule2.</value>
        [JsonProperty("DistributionRule2")]
        public string DistributionRule2 { get; set; }

        /// <summary>
        /// Gets or sets the DistributionRule3.
        /// </summary>
        /// <value>DistributionRule3.</value>
        [JsonProperty("DistributionRule3")]
        public string DistributionRule3 { get; set; }

        /// <summary>
        /// Gets or sets the DistributionRule4.
        /// </summary>
        /// <value>DistributionRule4.</value>
        [JsonProperty("DistributionRule4")]
        public string DistributionRule4 { get; set; }

        /// <summary>
        /// Gets or sets the DistributionRule5.
        /// </summary>
        /// <value>DistributionRule5.</value>
        [JsonProperty("DistributionRule5")]
        public string DistributionRule5 { get; set; }

        /// <summary>
        /// Gets or sets the Project.
        /// </summary>
        /// <value>Project.</value>
        [JsonProperty("Project")]
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        /// <value>Remarks.</value>
        [JsonProperty("Remarks")]
        public string Remarks { get; set; }
    }
}