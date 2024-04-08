// <summary>
// <copyright file="CreateDeliveryDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.SapServiceLayerAdapter.Common.DTOs.Batches;

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Delivery
{
    /// <summary>
    /// Class for the delivery model.
    /// </summary>
    public class CreateDeliveryDto
    {
        /// <summary>
        /// Gets or sets SaleOrderId.
        /// </summary>
        /// <value>SaleOrderId.</value>
        public int SaleOrderId { get; set; }

        /// <summary>
        /// Gets or sets ShippingCostOrderId.
        /// </summary>
        /// <value>ShippingCostOrderId.</value>
        public int ShippingCostOrderId { get; set; }

        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>ItemCode.</value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets OrderType.
        /// </summary>
        /// <value>OrderType.</value>
        public string OrderType { get; set; }

        /// <summary>
        /// Gets or sets Batches.
        /// </summary>
        /// <value>Batches.</value>
        public List<AlmacenBatchDto> Batches { get; set; }

        /// <summary>
        /// Gets or sets IsPackage.
        /// </summary>
        /// <value>IsPackage.</value>
        public string IsPackage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value IsOmigenomics.
        /// </summary>
        /// <value>IsOmigenomics.</value>
        public bool IsOmigenomics { get; set; }
    }
}
