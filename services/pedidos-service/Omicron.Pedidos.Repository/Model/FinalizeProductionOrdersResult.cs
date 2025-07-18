// <summary>
// <copyright file="FinalizeProductionOrdersResult.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Finalize Production Orders Result.
    /// </summary>
    public class FinalizeProductionOrdersResult
    {
        /// <summary>
        /// Gets or sets list Successful.
        /// </summary>
        /// <value>List FinalizeProductionOrderModel.</value>
        public List<FinalizeProductionOrderModel> Successful { get; set; }

        /// <summary>
        /// Gets or sets list Failed.
        /// </summary>
        /// <value>List ProductionOrderFailedResultModel.</value>
        public List<ProductionOrderFailedResultModel> Failed { get; set; }
    }
}
