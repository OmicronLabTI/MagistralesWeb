// <summary>
// <copyright file="OrderFabModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class for the order operation.
    /// </summary>
    public class OrderFabModel
    {
        /// <summary>
        /// Gets or sets ParentId.
        /// </summary>
        /// <value> ParentId.</value>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets AvailablePieces.
        /// </summary>
        /// <value> AvailablePieces.</value>
        public int AvailablePieces { get; set; }

        /// <summary>
        /// Gets or sets assigned pieces for this child order
        /// </summary>
        /// <value> AvailablePieces.</value>
        public int AssignedPieces { get; set; }
    }
}
