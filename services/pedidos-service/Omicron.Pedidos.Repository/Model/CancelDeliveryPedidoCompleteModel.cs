// <summary>
// <copyright file="CancelDeliveryPedidoCompleteModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// class model.
    /// </summary>
    public class CancelDeliveryPedidoCompleteModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<CancelDeliveryPedidoModel> CancelDelivery { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<DetallePedidoModel> DetallePedido { get; set; }
    }
}
