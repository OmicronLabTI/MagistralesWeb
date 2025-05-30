// <summary>
// <copyright file="RemittedPiecesModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    /// <summary>
    /// The remitted pieces model.
    /// </summary>
    public class RemittedPiecesModel
    {
        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>
        /// String ItemCode.
        /// </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Total Pieces.
        /// </summary>
        /// <value>
        /// Int Total Pieces.
        /// </value>
        public int TotalPieces { get; set; }

        /// <summary>
        /// Gets or sets Available Pieces.
        /// </summary>
        /// <value>
        /// Int Available Pieces.
        /// </value>
        public int AvailablePieces { get; set; }

        /// <summary>
        /// Gets or sets Remission Pieces.
        /// </summary>
        /// <value>
        /// Int Remission Pieces.
        /// </value>
        public int RemissionPieces { get; set; }
    }
}
