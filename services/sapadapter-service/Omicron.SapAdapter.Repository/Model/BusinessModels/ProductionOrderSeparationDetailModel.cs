// <summary>
// <copyright file="ProductionOrderSeparationDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    using System;

    /// <summary>
    /// Class for the doctor prescription info.
    /// </summary>
    public class ProductionOrderSeparationDetailModel
    {
        /// <summary>
        /// Gets or sets detailproductionorderid.
        /// </summary>
        /// <value>
        /// Int detailproductionorderid.
        /// </value>
        public int DetailOrderId { get; set; }

        /// <summary>
        /// Gets or sets OrderId.
        /// </summary>
        /// <value>
        /// Int OrderId.
        /// </value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// String userid.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets createdat.
        /// </summary>
        /// <value>
        /// DateTime createdat.
        /// </value>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets assignedpieces.
        /// </summary>
        /// <value>
        /// int assignedpieces.
        /// </value>
        public int AssignedPieces { get; set; }
    }
}
