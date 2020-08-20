// <summary>
// <copyright file="CancelOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models 
{
    /// <summary>
    /// Cancel a SAP order dto
    /// </summary>
    public class CancelOrderDto
    {
        /// <summary>
        /// Gets or sets order identifier.
        /// </summary>
        /// <value>The order id.</value>
        public int OrderId { get; set; }
    }
}
