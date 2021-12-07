// <summary>
// <copyright file="SendCancelDeliveryModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for the delivery model.
    /// </summary>
    public class SendCancelDeliveryModel
    {
        /// <summary>
        /// Gets or sets DeliveryId.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets SalesOrders.
        /// </summary>
        /// <value>The code.</value>
        public List<int> SalesOrders { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DeliveryType { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string DeliveryOrderType { get; set; }
    }
}
