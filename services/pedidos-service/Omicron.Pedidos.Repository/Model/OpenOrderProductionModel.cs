// <summary>
// <copyright file="OpenOrderProductionModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// the open orders productions.
    /// </summary>
    public class OpenOrderProductionModel
    {
        /// <summary>
        /// Gets or sets the parent order identifier.
        /// </summary>
        /// <value>The parent order id.</value>
        public string OrderProductionId { get; set; }

        /// <summary>
        /// Gets or sets the total pieces for the parent order.
        /// </summary>
        /// <value>The total pieces.</value>
        public int TotalPieces { get; set; }

        /// <summary>
        /// Gets or sets the available pieces for the parent order.
        /// </summary>
        /// <value>The available pieces.</value>
        public int AvailablePieces { get; set; }

        /// <summary>
        /// Gets or sets the QFB identifier who performed the split.
        /// </summary>
        /// <value>The QFB who split.</value>
        public string QfbWhoSplit { get; set; }

        /// <summary>
        /// Gets or sets the number of detail orders created from the split.
        /// </summary>
        /// <value>The detail orders count.</value>
        public int DetailOrdersCount { get; set; }

        /// <summary>
        /// Gets or sets the list of detail orders.
        /// </summary>
        /// <value>The detail orders.</value>
        public List<OpenOrderProductionDetailModel> OrderProductionDetail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI should auto expand the detail orders section.
        /// </summary>
        /// <value>True to auto expand, otherwise false.</value>
        public bool AutoExpandOrderDetail { get; set; }
    }
}
