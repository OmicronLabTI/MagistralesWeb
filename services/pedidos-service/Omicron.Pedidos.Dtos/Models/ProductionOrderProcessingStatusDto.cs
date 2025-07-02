// <summary>
// <copyright file="ProductionOrderProcessingStatusDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    using System;

    /// <summary>
    /// ProductionOrderProcessingStatusDto.
    /// </summary>
    public class ProductionOrderProcessingStatusDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// string Id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets ProductionOrderId.
        /// </summary>
        /// <value>
        /// Int ProductionOrderId.
        /// </value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Step.
        /// </summary>
        /// <value>
        /// String Step.
        /// </value>
        public string Step { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        /// <value>
        /// String Status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        /// <value>
        /// String ErrorMessage.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets Payload.
        /// </summary>
        /// <value>
        /// String Payload.
        /// </value>
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets CreatedAt.
        /// </summary>
        /// <value>
        /// DateTime CreatedAt.
        /// </value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets LastUpdated.
        /// </summary>
        /// <value>
        /// DateTime LastUpdated.
        /// </value>
        public DateTime LastUpdated { get; set; }
    }
}
