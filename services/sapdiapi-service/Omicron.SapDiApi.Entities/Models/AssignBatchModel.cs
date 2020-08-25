// <summary>
// <copyright file="AssignBatchModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Entities.Models
{
    /// <summary>
    /// the assgin batch model.
    /// </summary>
    public class AssignBatchModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public double AssignedQty { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string Action { get; set; }
    }
}
