// <summary>
// <copyright file="CancelOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class for the cancel order.
    /// </summary>
    public class CancelOrderModel
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The code.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets order id.
        /// </summary>
        /// <value>The order id.</value>
        public int OrderId { get; set; }
    }
}
