// <summary>
// <copyright file="RawMaterialRequestDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Dtos.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for request raw materia.
    /// </summary>
    public class RawMaterialRequestDto
    {
        /// <summary>
        /// Gets or sets the production order id.
        /// </summary>
        /// <value>
        /// Id production id.
        /// </value>
        public List<int> ProductionOrderIds { get; set; }

        /// <summary>
        /// Gets or sets signature in base 64 representation.
        /// </summary>
        /// <value>
        /// Base 64 signature.
        /// </value>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets observations.
        /// </summary>
        /// <value>
        /// String observations.
        /// </value>
        public string Observations { get; set; }

        /// <summary>
        /// Gets or sets ordered products.
        /// </summary>
        /// <value>
        /// List products.
        /// </value>
        public List<RawMaterialRequestDetailDto> OrderedProducts { get; set; }
    }
}
