// <summary>
// <copyright file="CreateDeliveryDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    /// <summary>
    /// the class to assign.
    /// </summary>
    public class CreateDeliveryDto
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
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string BatchName { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string OrderType { get; set; }
    }
}
