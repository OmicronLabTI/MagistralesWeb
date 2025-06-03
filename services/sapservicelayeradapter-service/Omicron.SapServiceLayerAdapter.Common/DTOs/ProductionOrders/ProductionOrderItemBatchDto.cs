// <summary>
// <copyright file="ProductionOrderItemBatchDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// the class ProductionOrderItemBatchDto.
    /// </summary>
    public class ProductionOrderItemBatchDto : BaseBatchProductionOrderDto
    {
        /// <summary>
        /// Gets or sets the BaseLineNumber.
        /// </summary>
        /// <value>BaseLineNumber.</value>
        [JsonProperty("BaseLineNumber")]
        public int BaseLineNumber { get; set; }
    }
}