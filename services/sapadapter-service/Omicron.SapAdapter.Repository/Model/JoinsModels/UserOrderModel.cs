// <summary>
// <copyright file="UserOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.JoinsModels
{
    using System;

    /// <summary>
    /// Class OrderLog Model.
    /// </summary>
    public class UserOrderModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets salesorderid.
        /// </summary>
        /// <value>
        /// String salesorderid.
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
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// String comments.
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        public int FinishedLabel { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        public string StatusAlmacen { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public DateTime? InvoiceStoreDate { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public int DeliveryId { get; set; }
    }
}
