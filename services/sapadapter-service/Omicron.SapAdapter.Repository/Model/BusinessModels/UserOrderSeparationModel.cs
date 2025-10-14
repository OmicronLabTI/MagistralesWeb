// <summary>
// <copyright file="UserOrderSeparationModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    using System.Collections.Generic;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// Class for the doctor prescription info.
    /// </summary>
    public class UserOrderSeparationModel
    {
        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public List<UserOrderModel> UserOrders { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public List<ProductionOrderSeparationModel> ProductionOrderSeparations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets OnSplitProcess.
        /// </summary>
        /// <value>
        /// bool OnSplitProcess.
        /// </value>
        public bool OnSplitProcess { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public List<ProductionOrderSeparationDetailModel> ProductionOrderSeparationsDetail { get; set; }
    }
}
