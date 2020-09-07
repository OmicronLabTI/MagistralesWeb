// <summary>
// <copyright file="CustomComponentListDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Custom formula.
    /// </summary>
    public class CustomComponentListDto
    {
        /// <summary>
        /// Gets or sets name.
        /// </summary>
        /// <value>
        /// The formula name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        public List<ComponentCustomComponentListDto> Components { get; set; }
    }
}
