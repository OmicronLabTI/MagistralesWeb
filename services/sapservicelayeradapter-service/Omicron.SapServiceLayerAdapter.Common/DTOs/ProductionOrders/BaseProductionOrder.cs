// <summary>
// <copyright file="BaseProductionOrder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// the class BaseProductionOrder.
    /// </summary>
    public class BaseProductionOrder
    {
        /// <summary>
        /// Gets or sets the PlannedQuantity.
        /// </summary>
        /// <value>PlannedQuantity.</value>
        [JsonProperty("PlannedQuantity")]
        public double PlannedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the ItemNo.
        /// </summary>
        /// <value>ItemNo.</value>
        [JsonProperty("ItemNo")]
        public string ItemNo { get; set; }

        /// <summary>
        /// Gets or sets the ProductDescription.
        /// </summary>
        /// <value>ProductDescription.</value>
        [JsonProperty("ProductDescription")]
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the DueDate.
        /// </summary>
        /// <value>DueDate.</value>
        [JsonProperty("DueDate")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        /// <value>StartDate.</value>
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the Remarks.
        /// </summary>
        /// <value>Remarks.</value>
        [JsonProperty("Remarks")]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the IsParentRecord.
        /// </summary>
        /// <value>IsParentRecord.</value>
        [JsonProperty("U_OPadre")]
        public string IsParentRecord { get; set; }
    }
}