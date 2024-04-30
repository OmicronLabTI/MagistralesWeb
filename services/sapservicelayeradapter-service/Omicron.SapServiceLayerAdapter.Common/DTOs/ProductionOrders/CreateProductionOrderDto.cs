// <summary>
// <copyright file="CreateProductionOrderDto.cs" company="Axity">
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
    public class CreateProductionOrderDto : BaseProductionOrder
    {
        /// <summary>
        /// Gets or sets the ProductionOrderOriginEntry.
        /// </summary>
        /// <value>ProductionOrderOriginEntry.</value>
        [JsonProperty("ProductionOrderOriginEntry")]
        public int ProductionOrderOriginEntry { get; set; }
    }
}