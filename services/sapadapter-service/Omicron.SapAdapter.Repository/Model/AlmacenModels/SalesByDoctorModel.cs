// <summary>
// <copyright file="SalesByDoctorModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// class for sap ids relation.
    /// </summary>
    public class SalesByDoctorModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public AlmacenSalesByDoctorModel AlmacenSalesByDoctor { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public AlmacenSalesByDoctorHeaderModel AlmacenHeaderByDoctor { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<OrderListByDoctorModel> Items { get; set; }
    }
}
