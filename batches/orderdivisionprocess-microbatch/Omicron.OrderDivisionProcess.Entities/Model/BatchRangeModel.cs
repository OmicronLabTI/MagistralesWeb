// <summary>
// <copyright file="BatchRangeModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Entities.Model
{
    /// <summary>
    /// BatchRangeModel.
    /// </summary>
    public class BatchRangeModel
    {
        /// <summary>
        /// Gets or sets Offset.
        /// </summary>
        /// <value>The code.</value>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets Limit.
        /// </summary>
        /// <value>The Limit.</value>
        public int Limit { get; set; }
    }
}
