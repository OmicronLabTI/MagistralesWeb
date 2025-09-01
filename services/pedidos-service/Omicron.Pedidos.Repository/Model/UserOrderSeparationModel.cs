// <summary>
// <copyright file="UserOrderSeparationModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Class for the UserOrderModel and ProductionOrderSeparationModel.
    /// </summary>
    public class UserOrderSeparationModel
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public List<UserOrderModel> UserOrders { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public List<ProductionOrderSeparationModel> ProductionOrderSeparations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets OnSplitProcess.
        /// </summary>
        /// <value>
        /// bool OnSplitProcess.
        /// </value>
        public bool OnSplitProcess { get; set; }
    }
}
