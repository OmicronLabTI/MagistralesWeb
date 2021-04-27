// <summary>
// <copyright file="CloseSampleOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// class for close.
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
    }
}
