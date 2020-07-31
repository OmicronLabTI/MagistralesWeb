// <summary>
// <copyright file="UserModel.cs" company="Axity">
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
    [Table("orderslogs")]
    public class OrderLogModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// Gets or sets logdatetime.
        /// </summary>
        /// <value>
        /// Datetime logdatetime.
        /// </value>
        public Datetime logdatetime { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// String userid.
        /// </value>
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        /// <value>
        /// String type.
        /// </value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets noid.
        /// </summary>
        /// <value>
        /// String noid.
        /// </value>
        public string noid { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        /// <value>
        /// String description.
        /// </value>
        public string description { get; set; }
    }
}
