// <summary>
// <copyright file="DeliveryDetailByOrdersModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.JoinsModels
{
    /// <summary>
    /// Modelo que representa la relación entre un Delivery Note (ODLN/DLN1)
    /// y un Pedido de Venta (ORDR/RDR1) en SAP Business One.
    /// </summary>
    public class DeliveryDetailByOrdersModel
    {
        /// <summary>
        /// Gets or sets DeliveryDocEntry.
        /// </summary>
        /// <value>The DeliveryDocEntry.</value>
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets DeliveryDocEntry.
        /// </summary>
        /// <value>The DeliveryDocEntry.</value>
        public int DeliveryLine { get; set; }

        /// <summary>
        /// Gets or sets DeliveryDocEntry.
        /// </summary>
        /// <value>The DeliveryDocEntry.</value>
        public int OrderLine { get; set; }

        /// <summary>
        /// Gets or sets DeliveryDocEntry.
        /// </summary>
        /// <value>The DeliveryDocEntry.</value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets DeliveryDocEntry.
        /// </summary>
        /// <value>The DeliveryDocEntry.</value>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets DeliveryDocEntry.
        /// </summary>
        /// <value>The DeliveryDocEntry.</value>
        public int OrderId { get; set; }
    }
}
