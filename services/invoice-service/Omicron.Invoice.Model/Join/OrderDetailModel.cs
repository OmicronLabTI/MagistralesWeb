// <summary>
// <copyright file="OrderDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Model.Join
{
    /// <summary>
    /// The Order detail model.
    /// </summary>
    public class OrderDetailModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public int IsSampleOrder { get; set; }
    }
}
