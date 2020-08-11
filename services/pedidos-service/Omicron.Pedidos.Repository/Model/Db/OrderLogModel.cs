// <summary>
// <copyright file="OrderLogModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
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
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets logdatetime.
        /// </summary>
        /// <value>
        /// Datetime logdatetime.
        /// </value>
        [Column("logdatetime")]
        public DateTime Logdatetime { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// String userid.
        /// </value>
        [Column("userid")]
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        /// <value>
        /// String type.
        /// </value>
        [Column("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets noid.
        /// </summary>
        /// <value>
        /// String noid.
        /// </value>
        [Column("noid")]
        public string Noid { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        /// <value>
        /// String description.
        /// </value>
        [Column("description")]
        public string Description { get; set; }
    }
}
