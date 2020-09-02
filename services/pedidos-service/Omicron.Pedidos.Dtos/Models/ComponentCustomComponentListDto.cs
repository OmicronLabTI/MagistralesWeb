// <summary>
// <copyright file="ComponentCustomComponentListDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// Custom formula component.
    /// </summary>
    public class ComponentCustomComponentListDto
    {
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
        /// Gets or sets base quantity.
        /// </summary>
        /// <value>The base quantity.</value>
        public decimal BaseQuantity { get; set; }
    }
}
