// <summary>
// <copyright file="ProductionOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Gets the join between DetalleFormula and OrdenFabricacionmodel.
    /// </summary>
    public class ProductionOrderModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int ProductionOrderId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<ProductionOrderComponentModel> Details { get; set; }
    }
}
