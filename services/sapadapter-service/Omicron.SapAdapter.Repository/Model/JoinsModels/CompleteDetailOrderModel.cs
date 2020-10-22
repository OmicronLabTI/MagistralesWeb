// <summary>
// <copyright file="CompleteDetailOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model
{
    using System;

    /// <summary>
    /// model for the detail.
    /// </summary>
    public class CompleteDetailOrderModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int OrdenFabricacionId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CodigoProducto { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DescripcionProducto { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DescripcionCorta { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal? QtyPlanned { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int? QtyPlannedDetalle { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string FechaOf { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string FechaOfFin { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Qfb { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string PedidoStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public bool HasMissingStock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public int FinishedLabel { get; set; }
    }
}
