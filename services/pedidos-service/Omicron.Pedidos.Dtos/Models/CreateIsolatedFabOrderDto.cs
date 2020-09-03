// <summary>
// <copyright file="CreateIsolatedFabOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// the update fab order model.
    /// </summary>
    public class CreateIsolatedFabOrderDto
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets product code.
        /// </summary>
        /// <value>The product code.</value>
        public string ProductCode { get; set; }
    }
}
