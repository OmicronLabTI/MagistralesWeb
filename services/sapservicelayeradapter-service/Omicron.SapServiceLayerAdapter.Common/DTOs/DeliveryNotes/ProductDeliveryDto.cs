// <summary>
// <copyright file="ProductDeliveryDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes
{
    /// <summary>
    /// Product Delivery Dto.
    /// </summary>
    public class ProductDeliveryDto
    {
        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Pieces.
        /// </summary>
        /// <value>
        /// Pieces.
        /// </value>
        public int Pieces { get; set; }
    }
}
