// <summary>
// <copyright file="ProductionOrderSeparationModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model.Db
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class ProductionOrderSeparation Model.
    /// </summary>
    [Table("productionorderseparation")]
    public class ProductionOrderSeparationModel
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
        /// Gets or sets productionorderid.
        /// </summary>
        /// <value>
        /// Int productionorderid.
        /// </value>
        [Column("productionorderid")]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets ProductionDetailCount.
        /// </summary>
        /// <value>
        /// Int ProductionDetailCount.
        /// </value>
        [Column("productiondetailcount")]
        public int ProductionDetailCount { get; set; }

        /// <summary>
        /// Gets or sets totalpieces.
        /// </summary>
        /// <value>
        /// Int totalpieces.
        /// </value>
        [Column("totalpieces")]
        public int TotalPieces { get; set; }

        /// <summary>
        /// Gets or sets availablepieces.
        /// </summary>
        /// <value>
        /// Int availablepieces.
        /// </value>
        [Column("availablepieces")]
        public int AvailablePieces { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        /// <value>
        /// string Status.
        /// </value>
        [Column("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets completedat .
        /// </summary>
        /// <value>
        /// DateTime completedat .
        /// </value>
        [Column("completedat")]
        public DateTime? CompletedAt { get; set; }
    }
}
