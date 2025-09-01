// <summary>
// <copyright file="ProductionOrderSeparationDetailLogModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Entities.Model
{
    /// <summary>
    /// ProductionOrderSeparationDetailLogModel.
    /// </summary>
    public class ProductionOrderSeparationDetailLogModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// string Id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets ParentProductionOrderId.
        /// </summary>
        /// <value>
        /// Int ParentProductionOrderId.
        /// </value>
        public int ParentProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Step.
        /// </summary>
        /// <value>
        /// String Step.
        /// </value>
        public string LastStep { get; set; } // Last Succesfully Step.

        /// <summary>
        /// Gets or sets IsSuccessful.
        /// </summary>
        /// <value>
        /// IsSuccessful.
        /// </value>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        /// <value>
        /// String ErrorMessage.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets ChildProductionOrderId.
        /// </summary>
        /// <value>
        /// Int ChildProductionOrderId.
        /// </value>
        public int? ChildProductionOrderId { get; set; }

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
