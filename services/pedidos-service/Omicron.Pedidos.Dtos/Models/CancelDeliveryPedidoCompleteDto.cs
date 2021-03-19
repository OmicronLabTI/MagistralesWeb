// <summary>
// <copyright file="CancelDeliveryPedidoCompleteDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Gets the model.
    /// </summary>
    public class CancelDeliveryPedidoCompleteDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<CancelDeliveryPedidoDto> CancelDelivery { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<DetallePedidoDto> DetallePedido { get; set; }
    }
}
