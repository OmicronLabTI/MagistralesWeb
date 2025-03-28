// <summary>
// <copyright file="WarehouseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Dtos.User
{
    using System.Collections.Generic;

    /// <summary>
    /// Class Warehouse Dto.
    /// </summary>
    public class WarehouseDto
    {
        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// String comments.
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// String comments.
        public List<string> WarehouseCodes { get; set; }
    }
}
