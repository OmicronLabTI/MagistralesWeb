// <summary>
// <copyright file="UserOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Entities.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class OrderLog Model.
    /// </summary>
    [Table("usersorders")]
    public class UserOrderModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
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
        /// </value>
        public string Productionorderid { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        /// </value>
        public string Status { get; set; }
    }
}
