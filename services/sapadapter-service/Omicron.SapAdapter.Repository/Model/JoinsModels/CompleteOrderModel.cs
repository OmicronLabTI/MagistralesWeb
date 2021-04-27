// <summary>
// <copyright file="CompleteOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model
{
    /// <summary>
    /// class for the complete order.
    /// </summary>
    public class CompleteOrderModel
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
        public string Codigo { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Medico { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string AsesorName { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string FechaInicio { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string FechaFin { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string PedidoStatus { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Qfb { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public DetallePedidoModel Detalles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string LabelType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public int FinishedLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public int? AtcEntry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string HasRecipte { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string OrderType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsChecked.
        /// </summary>
        /// <value>The IsChecked.</value>
        public string Canceled { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string PedidoMuestra { get; set; }
    }
}
