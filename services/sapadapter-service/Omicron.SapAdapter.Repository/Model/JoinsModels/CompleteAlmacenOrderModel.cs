// <summary>
// <copyright file="CompleteAlmacenOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Entities.Model.JoinsModels
{
    using System;

    /// <summary>
    /// Class for the order model.
    /// </summary>
    public class CompleteAlmacenOrderModel
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
        public DetallePedidoModel Detalles { get; set; }
    }
}
