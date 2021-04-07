// <summary>
// <copyright file="InvoiceHeaderAdvancedLookUp.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;

    /// <summary>
    /// Gets the almacen invoice header.
    /// </summary>
    public class InvoiceHeaderAdvancedLookUp
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Invoice { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Doctor { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime InvoiceDocDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int TotalDeliveries { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int TotalProducts { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DocEntry { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DeliverId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int SalesOrder { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string StatusDelivery { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime? DataCheckin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsLookUpInvoices { get; set; }
    }
}
