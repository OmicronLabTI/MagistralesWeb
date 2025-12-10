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
    using System.Collections.Generic;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;

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
        public string RealLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public int FinishedLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public string NeedsCooling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public int PedidoId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the value for missing stock.
        /// </summary>
        /// <value>the value of stock.</value>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CatalogGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsOmigenomics.
        /// </summary>
        /// <value>IsOmigenomics.</value>
        public bool IsOmigenomics { get; set; }

        /// <summary>
        /// Gets or sets OrderRelationType.
        /// </summary>
        /// <value>
        /// string OrderRelationType.
        /// </value>
        public string ProductFirmName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the production order OnSplitProcess.
        /// </summary>
        /// <value>The OnSplitProcess.</value>
        public bool OnSplitProcess { get; set; }

        /// <summary>
        /// Gets or sets OrderRelationType.
        /// </summary>
        /// <value>
        /// string OrderRelationType.
        /// </value>
        public string OrderRelationType { get; set; }

        /// <summary>
        /// Gets or sets ChildrenOrderPieces.
        /// </summary>
        /// <value>The ChildrenOrderPieces.</value>
        public int AvailablePieces { get; set; }

        /// <summary>
        /// Gets or sets ChildrenOrderPieces.
        /// </summary>
        /// <value>The ChildrenOrderPieces.</value>
        public int ChildOrdersCount { get; set; }

        /// <summary>
        /// Gets or sets ChildrenOrderPieces.
        /// </summary>
        /// <value>The ChildrenOrderPieces.</value>
        public List<ChildOrderModel> ChildOrders { get; set; } = new List<ChildOrderModel>();
    }
}
