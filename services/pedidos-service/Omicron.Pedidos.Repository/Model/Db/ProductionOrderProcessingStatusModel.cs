// <summary>
// <copyright file="ProductionOrderProcessingStatusModel.cs" company="Axity">
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
    /// Gets the production order qr.
    /// </summary>
    [Table("productionorderprocessingstatus")]
    public class ProductionOrderProcessingStatusModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// string Id.
        /// </value>
        [Key]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// Int ProductionOrderId.
        /// </value>
        [Column("productionorderid")]
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Step.
        /// </summary>
        /// <value>
        /// String Step.
        /// </value>
        [Column("step")]
        public string Step { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        /// <value>
        /// String Status.
        /// </value>
        [Column("status")]
        public string Status { get; set; } // "In Progress", "Success", "Failed"

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        /// <value>
        /// String ErrorMessage.
        /// </value>
        [Column("errormessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets Payload.
        /// </summary>
        /// <value>
        /// String Payload.
        /// </value>
        [Column("payload", TypeName = "json")]
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets LastUpdated.
        /// </summary>
        /// <value>
        /// DateTime LastUpdated.
        /// </value>
        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets LastUpdated.
        /// </summary>
        /// <value>
        /// DateTime LastUpdated.
        /// </value>
        [Column("lastupdated")]
        public DateTime LastUpdated { get; set; }
    }
}
