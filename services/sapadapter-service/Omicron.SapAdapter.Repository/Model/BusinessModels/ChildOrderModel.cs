// <summary>
// <copyright file="ChildOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    /// <summary>
    /// Class for the doctor prescription info.
    /// </summary>
    public class ChildOrderModel
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
        public int AssignedPieces { get; set; }

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
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string UserCreate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CreateDate { get; set; }

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
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CatalogGroup { get; set; }
    }
}
