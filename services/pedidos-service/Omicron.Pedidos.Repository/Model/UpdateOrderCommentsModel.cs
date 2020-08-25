// <summary>
// <copyright file="UpdateOrderCommentsModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class for the updateStatus.
    /// </summary>
    public class UpdateOrderCommentsModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets comments.
        /// </summary>
        /// <value>The comments.</value>
        public string Comments { get; set; }
    }
}
