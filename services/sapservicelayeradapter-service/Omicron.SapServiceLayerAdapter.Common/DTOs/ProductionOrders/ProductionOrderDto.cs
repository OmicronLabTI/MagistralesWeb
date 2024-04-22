// <summary>
// <copyright file="ProductionOrderDto.cs" company="Axity">
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
    public class ProductionOrderDto
    {
        /// <summary>
        /// Gets or sets the AbsoluteEntry.
        /// </summary>
        /// <value>AbsoluteEntry.</value>
        [JsonProperty("AbsoluteEntry")]
        public int AbsoluteEntry { get; set; }

        /// <summary>
        /// Gets or sets the DocumentNumber.
        /// </summary>
        /// <value>DocumentNumber.</value>
        [JsonProperty("DocumentNumber")]
        public int DocumentNumber { get; set; }

        /// <summary>
        /// Gets or sets the Series.
        /// </summary>
        /// <value>Series.</value>
        [JsonProperty("Series")]
        public int Series { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderOriginNumber.
        /// </summary>
        /// <value>ProductionOrderOriginNumber.</value>
        [JsonProperty("ProductionOrderOriginNumber")]
        public int ProductionOrderOriginNumber { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderOriginEntry.
        /// </summary>
        /// <value>ProductionOrderOriginEntry.</value>
        [JsonProperty("ProductionOrderOriginEntry")]
        public int ProductionOrderOriginEntry { get; set; }

        /// <summary>
        /// Gets or sets the TransactionNumber.
        /// </summary>
        /// <value>TransactionNumber.</value>
        [JsonProperty("TransactionNumber")]
        public int TransactionNumber { get; set; }

        /// <summary>
        /// Gets or sets the Priority.
        /// </summary>
        /// <value>Priority.</value>
        [JsonProperty("Priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the UoMEntry.
        /// </summary>
        /// <value>UoMEntry.</value>
        [JsonProperty("UoMEntry")]
        public int UoMEntry { get; set; }

        /// <summary>
        /// Gets or sets the UserSignature.
        /// </summary>
        /// <value>UserSignature.</value>
        [JsonProperty("UserSignature")]
        public int UserSignature { get; set; }

        /// <summary>
        /// Gets or sets the PlannedQuantity.
        /// </summary>
        /// <value>PlannedQuantity.</value>
        [JsonProperty("PlannedQuantity")]
        public double PlannedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the CompletedQuantity.
        /// </summary>
        /// <value>CompletedQuantity.</value>
        [JsonProperty("CompletedQuantity")]
        public double CompletedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the RejectedQuantity.
        /// </summary>
        /// <value>RejectedQuantity.</value>
        [JsonProperty("RejectedQuantity")]
        public double RejectedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the ItemNo.
        /// </summary>
        /// <value>ItemNo.</value>
        [JsonProperty("ItemNo")]
        public string ItemNo { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderStatus.
        /// </summary>
        /// <value>ProductionOrderStatus.</value>
        [JsonProperty("ProductionOrderStatus")]
        public string ProductionOrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderType.
        /// </summary>
        /// <value>ProductionOrderType.</value>
        [JsonProperty("ProductionOrderType")]
        public string ProductionOrderType { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderOrigin.
        /// </summary>
        /// <value>ProductionOrderOrigin.</value>
        [JsonProperty("ProductionOrderOrigin")]
        public string ProductionOrderOrigin { get; set; }

        /// <summary>
        /// Gets or sets the CustomerCode.
        /// </summary>
        /// <value>CustomerCode.</value>
        [JsonProperty("CustomerCode")]
        public string CustomerCode { get; set; }

        /// <summary>
        /// Gets or sets the Warehouse.
        /// </summary>
        /// <value>Warehouse.</value>
        [JsonProperty("Warehouse")]
        public string Warehouse { get; set; }

        /// <summary>
        /// Gets or sets the InventoryUOM.
        /// </summary>
        /// <value>InventoryUOM.</value>
        [JsonProperty("InventoryUOM")]
        public string InventoryUOM { get; set; }

        /// <summary>
        /// Gets or sets the JournalRemarks.
        /// </summary>
        /// <value>JournalRemarks.</value>
        [JsonProperty("JournalRemarks")]
        public string JournalRemarks { get; set; }

        /// <summary>
        /// Gets or sets the Printed.
        /// </summary>
        /// <value>Printed.</value>
        [JsonProperty("Printed")]
        public string Printed { get; set; }

        /// <summary>
        /// Gets or sets the ProductDescription.
        /// </summary>
        /// <value>ProductDescription.</value>
        [JsonProperty("ProductDescription")]
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the RoutingDateCalculation.
        /// </summary>
        /// <value>RoutingDateCalculation.</value>
        [JsonProperty("RoutingDateCalculation")]
        public string RoutingDateCalculation { get; set; }

        /// <summary>
        /// Gets or sets the UpdateAllocation.
        /// </summary>
        /// <value>UpdateAllocation.</value>
        [JsonProperty("UpdateAllocation")]
        public string UpdateAllocation { get; set; }

        /// <summary>
        /// Gets or sets the PostingDate.
        /// </summary>
        /// <value>PostingDate.</value>
        [JsonProperty("PostingDate")]
        public string PostingDate { get; set; }

        /// <summary>
        /// Gets or sets the DueDate.
        /// </summary>
        /// <value>DueDate.</value>
        [JsonProperty("DueDate")]
        public string DueDate { get; set; }

        /// <summary>
        /// Gets or sets the CreationDate.
        /// </summary>
        /// <value>CreationDate.</value>
        [JsonProperty("CreationDate")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        /// <value>StartDate.</value>
        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the ClosingDate.
        /// </summary>
        /// <value>ClosingDate.</value>
        [JsonProperty("ClosingDate")]
        public string ClosingDate { get; set; }

        /// <summary>
        /// Gets or sets the ReleaseDate.
        /// </summary>
        /// <value>ReleaseDate.</value>
        [JsonProperty("ReleaseDate")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the ProductionOrderLines.
        /// </summary>
        /// <value>ProductionOrderLines.</value>
        [JsonProperty("ProductionOrderLines")]
        public List<ProductionOrderLineDto> ProductionOrderLines { get; set; }
    }
}