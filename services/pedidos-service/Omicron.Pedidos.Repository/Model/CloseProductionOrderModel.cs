// <summary>
// <copyright file="CloseProductionOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Close production order model.
    /// </summary>
    public class CloseProductionOrderModel
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The code.</value>
        public string UserId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets order identifier.
        /// </summary>
        /// <value>The order id.</value>
        public int OrderId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets batches.
        /// </summary>
        /// <value>The order batches.</value>
        public List<BatchesConfigurationModel> Batches
        {
            get; set;
        }
    }
}
