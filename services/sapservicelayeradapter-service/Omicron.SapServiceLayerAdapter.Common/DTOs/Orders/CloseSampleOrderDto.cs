// <summary>
// <copyright file="CloseSampleOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Orders
{
    /// <summary>
    /// Class for Close Sample Order Dto.
    /// </summary>
    public class CloseSampleOrderDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public int SaleOrderId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public List<int> FabOrdersId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public List<CreateDeliveryDto> ItemsList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets CloseOrder.
        /// </summary>
        /// <value>The CloseOrder.</value>
        public bool CloseOrder { get; set; }
    }
}
