// <summary>
// <copyright file="FabOrderDetail.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// the fabrication order detail.
    /// </summary>
    public class FabOrderDetail
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int BaseDocument { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal PlannedQuantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string FinishDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DescriptionProduct { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Destiny { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool HasMissingStock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int FinishedLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool AreBatchesComplete { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ClientDxp { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ShopTransaction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets if technical sign the order.
        /// </summary>
        /// <value>The code.</value>
        public bool TechnicalSign { get; set; }

        /// <summary>
        /// Gets or sets QFB Name.
        /// </summary>
        /// <value>String QFB Name.</value>
        public string QfbName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets if the order has Technical assigned.
        /// </summary>
        /// <value>The code.</value>
        public bool HasTechnicalAssigned { get; set; }

        /// <summary>
        /// Gets or sets a OrderRelationType.
        /// </summary>
        /// <value>The OrderRelationType.</value>
        public string OrderRelationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool HasBatches { get; set; }
    }
}
