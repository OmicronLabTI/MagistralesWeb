// <summary>
// <copyright file="CompleteDetalleFormulaModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class to join wor1 with oitm and oitw.
    /// </summary>
    public class CompleteDetalleFormulaModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int OrderFabId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal BaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal RequiredQuantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Consumed { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal Available { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Warehouse { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal PendingQuantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal Stock { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal WarehouseQuantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Action { get; set; }
    }
}
