// <summary>
// <copyright file="ProductivityModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// the productivity model.
    /// </summary>
    public class ProductivityModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<List<string>> Matrix { get; set; }
    }
}
