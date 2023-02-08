// <summary>
// <copyright file="PaymentsDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Dtos.DxpModels
{
    /// <summary>
    /// The payment table.
    /// </summary>
    public class PaymentsDto
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the logged in user to made the transaction.
        /// </summary>
        /// <value>
        /// User transaction Name.
        /// </value>
        public int ShippingCostAccepted { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string DeliveryComments { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string DeliverySuggestedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Is Doctor Direction.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public int IsDoctorDirection { get; set; }
    }
}
