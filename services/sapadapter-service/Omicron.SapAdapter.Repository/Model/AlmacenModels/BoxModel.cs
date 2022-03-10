// <summary>
// <copyright file="BoxModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;

    /// <summary>
    /// class for the boxes of the packages.
    /// </summary>
    public class BoxModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets dimension.
        /// </summary>
        /// <value>
        /// dimension.
        /// </value>
        public string Dimensions { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// weight.
        /// </value>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets register.
        /// </summary>
        /// <value>
        /// register Datetime.
        /// </value>
        public DateTime RegisterDate { get; set; }
    }
}
