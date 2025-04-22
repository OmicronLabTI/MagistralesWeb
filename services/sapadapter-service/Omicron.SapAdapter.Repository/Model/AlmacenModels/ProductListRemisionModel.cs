// <summary>
// <copyright file="ProductListRemisionModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    /// <summary>
    /// ProductList model.
    /// </summary>
    public class ProductListRemisionModel : ProductListBaseModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DeliveryId { get; set; }
    }
}
