// <summary>
// <copyright file="SalesLogs.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// class for the detail.
    /// </summary>
    public class SalesLogs
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets SalesOrderId.
        /// </summary>
        /// <value>
        /// String SalesOrderId.
        /// </value>
        public int SalesOrderId { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// String ProductionOrderId.
        /// </value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets StatusSalesOrder.
        /// </summary>
        /// <value>
        /// String StatusSalesOrder.
        /// </value>
        public string StatusSalesOrder { get; set; }

        /// <summary>
        /// Gets or sets StatusProductionOrder.
        /// </summary>
        /// <value>
        /// String StatusProductionOrder.
        /// </value>
        public string StatusProductionOrder { get; set; }

        /// <summary>
        /// Gets or sets DataCheckin.
        /// </summary>
        /// <value>
        /// dataTime DataCheckin.
        /// </value>
        public DateTime DataCheckin { get; set; }

        /// <summary>
        /// Gets or sets DataCheckin.
        /// </summary>
        /// <value>
        /// dataTime DataCheckin.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets DataCheckin.
        /// </summary>
        /// <value>
        /// dataTime DataCheckin.
        /// </value>
        public bool IsProductionOrder { get; set; }
    }
}
