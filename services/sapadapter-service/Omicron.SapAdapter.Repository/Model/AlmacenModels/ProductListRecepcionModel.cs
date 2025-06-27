// <summary>
// <copyright file="ProductListRecepcionModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System.Collections.Generic;

    /// <summary>
    /// ProductList model.
    /// </summary>
    public class ProductListRecepcionModel : ProductListBaseModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<int> DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Has Pending To Store.
        /// </summary>
        /// <value>The code.</value>
        public bool HasPendingToStore { get; set; }
    }
}
