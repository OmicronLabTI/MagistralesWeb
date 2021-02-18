// <summary>
// <copyright file="RejectOrdersModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// class for the order operation.
    /// </summary>
    public class RejectOrdersModel
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The code.</value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets list orders.
        /// </summary>
        /// <value>The list orders.</value>
        public List<object> OrdersId { get; set; }
    }
}
