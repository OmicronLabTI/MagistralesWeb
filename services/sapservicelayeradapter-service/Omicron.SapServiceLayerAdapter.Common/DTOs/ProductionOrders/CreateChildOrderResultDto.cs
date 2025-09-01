// <summary>
// <copyright file="CreateChildOrderResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.ProductionOrders
{
    /// <summary>
    /// the order.
    /// </summary>
    public class CreateChildOrderResultDto
    {
        /// <summary>
        /// Gets or sets the Id of the created child production order.
        /// </summary>
        public int ProductionOrderChildId { get; set; }

        /// <summary>
        /// Gets or sets the last step executed in the process.
        /// </summary>
        public string LastStep { get; set; }

        /// <summary>
        /// Gets or sets the error message if the process failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
