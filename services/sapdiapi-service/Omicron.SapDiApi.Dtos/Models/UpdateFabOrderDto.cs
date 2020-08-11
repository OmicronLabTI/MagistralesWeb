// <summary>
// <copyright file="UpdateFabOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    /// <summary>
    /// the update fab order dto.
    /// </summary>
    public class UpdateFabOrderDto
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
        public string Status { get; set;  }
    }
}
