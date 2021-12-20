// <summary>
// <copyright file="CompleteDeliveryDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.JoinsModels
{
    using System;
    using Omicron.SapAdapter.Entities.Model.DbModels;

    /// <summary>
    /// class for delivery detail.
    /// </summary>
    public class CompleteDeliveryDetailModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DocNum { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Cliente { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Medico { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public DeliveryDetailModel Detalles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public ProductoModel Producto { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Order Type.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string TypeOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Order Type.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string Comments { get; set; }
    }
}
