// <summary>
// <copyright file="OrderWithDetailDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    using System.Collections.Generic;    

    /// <summary>
    /// the object.
    /// </summary>
    public class OrderWithDetailDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public OrderDto Order { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<CompleteDetailDto> Detalle { get; set; }
    }
}
