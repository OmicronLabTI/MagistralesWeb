// <summary>
// <copyright file="ManualAssignDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Manual assign Dto.
    /// </summary>
    public class ManualAssignDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string UserLogistic { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>the pedidoId or orderFabId.</value>
        public List<int> DocEntry { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>If it is a Pedido or OrdenFab.</value>
        public string OrderType { get; set; }
    }
}
