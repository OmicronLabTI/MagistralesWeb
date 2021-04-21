// <summary>
// <copyright file="CancelDeliveryDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for canceling.
    /// </summary>
    public class CancelDeliveryDto
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public int Delivery { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public List<int> SaleOrderId { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public List<ProductDeliveryDto> MagistralProducts { get; set; }
    }
}
