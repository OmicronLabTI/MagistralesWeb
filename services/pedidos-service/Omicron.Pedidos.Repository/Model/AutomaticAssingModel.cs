// <summary>
// <copyright file="AutomaticAssingModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// the automatic assign Dto.
    /// </summary>
    public class AutomaticAssingModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The user that is assigning.</value>
        public string UserLogistic { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>the pedidoId or orderFabId.</value>
        public List<int> DocEntry { get; set; }
    }
}
