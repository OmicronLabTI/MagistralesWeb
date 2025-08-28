// <summary>
// <copyright file="ProductionOrderSeparationDetailLogsModel.cs" company="Axity">
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
    /// Class productionorderseparationdetail Model.
    /// </summary>
    [Table("productionorderseparationdetaillogs")]
    public class ProductionOrderSeparationDetailLogsModel
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
        [Column("productionorderparent")]
        public int ParentProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Step.
        /// </summary>
        /// <value>
        /// String Step.
        /// </value>
        [Column("laststep")]
        public string LastStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Status.
        /// </summary>
        /// <value>
        /// Bool Status.
        /// </value>
        [Column("issuccessful")]
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        /// <value>
        /// String ErrorMessage.
        /// </value>
        [Column("errormessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// Int ProductionOrderId.
        /// </value>
        [Column("productionorderchild")]
        public int? ChildProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Payload.
        /// </summary>
        /// <value>
        /// String Payload.
        /// </value>
        [Column("payload", TypeName = "json")]
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets CreatedAt.
        /// </summary>
        /// <value>
        /// DateTime CreatedAt.
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
