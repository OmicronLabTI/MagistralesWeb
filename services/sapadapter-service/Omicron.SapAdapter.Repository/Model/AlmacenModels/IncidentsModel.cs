// <summary>
// <copyright file="IncidentsModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;

    /// <summary>
    /// class for incidents.
    /// </summary>
    public class IncidentsModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public int SaleOrderId { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Incidence { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Batches { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Stage { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string User { get; set; }
    }
}
