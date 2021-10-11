// <summary>
// <copyright file="InsertLogModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.KafkaConsumer.Entities.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// class insert log.
    /// </summary>
    [Table("SalesLogs")]
    public class InsertLogModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets SalesOrderId.
        /// </summary>
        /// <value>
        /// String SalesOrderId.
        /// </value>
        [Column("SalesOrderId")]
        public int SalesOrderId { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// String ProductionOrderId.
        /// </value>
        [Column("ProductionOrderId")]
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets StatusSalesOrder.
        /// </summary>
        /// <value>
        /// String StatusSalesOrder.
        /// </value>
        [Column("StatusSalesOrder")]
        public string StatusSalesOrder { get; set; }

        /// <summary>
        /// Gets or sets StatusProductionOrder.
        /// </summary>
        /// <value>
        /// String StatusProductionOrder.
        /// </value>
        [Column("StatusProductionOrder")]
        public string StatusProductionOrder { get; set; }

        /// <summary>
        /// Gets or sets DataCheckin.
        /// </summary>
        /// <value>
        /// dataTime DataCheckin.
        /// </value>
        [Column("DataCheckin")]
        public DateTime DataCheckin { get; set; }

        /// <summary>
        /// Gets or sets DataCheckin.
        /// </summary>
        /// <value>
        /// dataTime DataCheckin.
        /// </value>
        [Column("UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets DataCheckin.
        /// </summary>
        /// <value>
        /// dataTime DataCheckin.
        /// </value>
        [Column("IsProductionOrder")]
        public bool IsProductionOrder { get; set; }
    }
}
