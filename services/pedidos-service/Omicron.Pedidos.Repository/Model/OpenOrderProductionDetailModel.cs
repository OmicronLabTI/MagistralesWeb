// <summary>
// <copyright file="OpenOrderProductionDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// the open orders productions detail.
    /// </summary>
    public class OpenOrderProductionDetailModel
    {
        /// <summary>
        /// Gets or sets the detail order identifier.
        /// </summary>
        /// <value>The detail order id.</value>
        public string OrderProductionDetailId { get; set; }

        /// <summary>
        /// Gets or sets the assigned pieces to the detail order.
        /// </summary>
        /// <value>The assigned pieces.</value>
        public int AssignedPieces { get; set; }

        /// <summary>
        /// Gets or sets the QFB assigned to this detail order.
        /// </summary>
        /// <value>The assigned QFB.</value>
        public string AssignedQfb { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the detail order.
        /// Keep as string to match existing conventions (e.g., "7/09/2025 17:53:00").
        /// </summary>
        /// <value>The creation date as string.</value>
        public string DateCreated { get; set; }
    }
}
