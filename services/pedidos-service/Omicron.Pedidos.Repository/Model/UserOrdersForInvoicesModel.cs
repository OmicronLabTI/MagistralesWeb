// <summary>
// <copyright file="UserOrdersForInvoicesModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class forUserOrdersForInvoicesModel.
    /// </summary>
    public class UserOrdersForInvoicesModel
    {
        /// <summary>
        /// Gets or sets Salesorderid.
        /// </summary>
        /// <value>
        /// String Salesorderid.
        /// </value>
        public string Salesorderid { get; set; }

        /// <summary>
        /// Gets or sets productionorderid.
        /// </summary>
        /// <value>
        /// String productionorderid.
        public string Productionorderid { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets StatusAlmacen.
        /// </summary>
        /// <value>
        /// StatusAlmacen.
        public string StatusAlmacen { get; set; }

        /// <summary>
        /// Gets or sets DeliveryId.
        /// </summary>
        /// <value>
        /// DeliveryId.
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets StatusInvoice.
        /// </summary>
        /// <value>
        /// StatusInvoice.
        /// </value>
        public string StatusInvoice { get; set; }
    }
}
