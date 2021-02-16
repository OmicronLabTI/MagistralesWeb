// <summary>
// <copyright file="InvoiceProductModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System.Collections.Generic;

    /// <summary>
    /// class for the product.
    /// </summary>
    public class InvoiceProductModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<string> Batches { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool NeedsCooling { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value.
        /// </summary>
        /// <value>The code.</value>
        public bool IsMagistral { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value.
        /// </summary>
        /// <value>The code.</value>
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value.
        /// </summary>
        /// <value>The code.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value.
        /// </summary>
        /// <value>The code.</value>
        public int SaleOrderId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value.
        /// </summary>
        /// <value>The code.</value>
        public IncidentInfoModel Incident { get; set; }
    }
}
