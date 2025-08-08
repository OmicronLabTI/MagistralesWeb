// <summary>
// <copyright file="ProductionOrderSeparationModel.cs" company="Axity">
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
    public class ProductionOrderSeparationModel
    {
        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public int ProductionDetailCount { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public int TotalPieces { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public int AvailablePieces { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public DateTime CompletedAt { get; set; }
    }
}
