// <summary>
// <copyright file="RawMaterialRequestDetailDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Dtos.Model
{
    /// <summary>
    /// Class for request raw materia.
    /// </summary>
    public class RawMaterialRequestDetailDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets product id.
        /// </summary>
        /// <value>The code.</value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets product description.
        /// </summary>
        /// <value>The code.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets request quantity.
        /// </summary>
        /// <value>The base quantity.</value>
        public decimal RequestQuantity { get; set; }

        /// <summary>
        /// Gets or sets measurement of unit.
        /// </summary>
        /// <value>The measurement unit.</value>
        public string Unit { get; set; }
    }
}
