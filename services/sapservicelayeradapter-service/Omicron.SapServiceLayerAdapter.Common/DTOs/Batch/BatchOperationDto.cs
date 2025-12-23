// <summary>
// <copyright file="BatchOperationDto.cs" company="Axity">
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
    public class BatchOperationDto
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets WarehouseCode.
        /// </summary>
        /// <value>
        /// String WarehouseCode.
        /// </value>
        public object Body { get; set; }

        /// <summary>
        /// Gets or sets WarehouseCode.
        /// </summary>
        /// <value>
        /// String WarehouseCode.
        /// </value>
        public string ContentId { get; set; }
    }
}
