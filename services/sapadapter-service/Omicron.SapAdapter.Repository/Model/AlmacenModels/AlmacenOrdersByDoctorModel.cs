// <summary>
// <copyright file="AlmacenOrdersByDoctorModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Class For Almacen orders.
    /// </summary>
    public class AlmacenOrdersByDoctorModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int TotalSalesOrders { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<SalesByDoctorModel> SalesOrders { get; set; }
    }
}
