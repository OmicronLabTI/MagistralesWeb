// <summary>
// <copyright file="AlmacenBatchDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    /// <summary>
    /// class for the almacen dto.
    /// </summary>
    public class AlmacenBatchDto
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string BatchNumber { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public decimal BatchQty { get; set; }
    }
}
