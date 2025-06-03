// <summary>
// <copyright file="BaseCreateProductionOrderLineDto.cs" company="Axity">
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
    public class BaseCreateProductionOrderLineDto
    {
        /// <summary>
        /// Gets or sets the DocumentAbsoluteEntry.
        /// </summary>
        /// <value>DocumentAbsoluteEntry.</value>
        [JsonProperty("DocumentAbsoluteEntry")]
        public int DocumentAbsoluteEntry { get; set; }

        /// <summary>
        /// Gets or sets the VisualOrder.
        /// </summary>
        /// <value>VisualOrder.</value>
        [JsonProperty("VisualOrder")]
        public int? VisualOrder { get; set; }

        /// <summary>
        /// Gets or sets the UoMEntry.
        /// </summary>
        /// <value>UoMEntry.</value>
        [JsonProperty("UoMEntry")]
        public int? UoMEntry { get; set; }

        /// <summary>
        /// Gets or sets the UoMCode.
        /// </summary>
        /// <value>UoMCode.</value>
        [JsonProperty("UoMCode")]
        public int? UoMCode { get; set; }

        /// <summary>
        /// Gets or sets the BaseQuantity.
        /// </summary>
        /// <value>BaseQuantity.</value>
        [JsonProperty("BaseQuantity")]
        public double BaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the PlannedQuantity.
        /// </summary>
        /// <value>PlannedQuantity.</value>
        [JsonProperty("PlannedQuantity")]
        public double PlannedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the IssuedQuantity.
        /// </summary>
        /// <value>IssuedQuantity.</value>
        [JsonProperty("IssuedQuantity")]
        public double IssuedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalQuantity.
        /// </summary>
        /// <value>AdditionalQuantity.</value>
        [JsonProperty("AdditionalQuantity")]
        public double AdditionalQuantity { get; set; }

        /// <summary>
        /// Gets or sets the RequiredDays.
        /// </summary>
        /// <value>RequiredDays.</value>
        [JsonProperty("RequiredDays")]
        public double RequiredDays { get; set; }

        /// <summary>
        /// Gets or sets the ItemNo.
        /// </summary>
        /// <value>ItemNo.</value>
        [JsonProperty("ItemNo")]
        public string ItemNo { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderIssueType.
        /// </summary>
        /// <value>ProductionOrderIssueType.</value>
        [JsonProperty("ProductionOrderIssueType")]
        public string ProductionOrderIssueType { get; set; }

        /// <summary>
        /// Gets or sets the Warehouse.
        /// </summary>
        /// <value>Warehouse.</value>
        [JsonProperty("Warehouse")]
        public string Warehouse { get; set; }

        /// <summary>
        /// Gets or sets the ItemName.
        /// </summary>
        /// <value>ItemName.</value>
        [JsonProperty("ItemName")]
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        /// <value>StartDate.</value>
        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        /// <value>EndDate.</value>
        [JsonProperty("EndDate")]
        public string EndDate { get; set; }

        /// <summary>
        /// Gets or sets the BatchNumbers.
        /// </summary>
        /// <value>BatchNumbers.</value>
        [JsonProperty("BatchNumbers")]
        public List<BaseBatchProductionOrderDto> BatchNumbers { get; set; }
    }
}