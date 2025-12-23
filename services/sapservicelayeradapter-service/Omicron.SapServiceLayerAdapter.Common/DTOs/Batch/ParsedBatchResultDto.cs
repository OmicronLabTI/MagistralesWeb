// <summary>
// <copyright file="ParsedBatchResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Batch
{
    /// <summary>
    /// The class for almacen batch.
    /// </summary>
    public class ParsedBatchResultDto
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public int ContentId { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets WarehouseCode.
        /// </summary>
        /// <value>
        /// String WarehouseCode.
        /// </value>
        public string ContentBody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets.
        /// </summary>
        /// <value>
        /// bool IsError.
        /// </value>
        public bool IsError { get; set; }
    }
}
