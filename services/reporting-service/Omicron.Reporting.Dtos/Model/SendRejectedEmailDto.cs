// <summary>
// <copyright file="SendRejectedEmailDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Dtos.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for request raw materia.
    /// </summary>
    public class SendRejectedEmailDto
    {
        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        /// <value>
        /// Id production ids.
        /// </value>
        public List<RejectedOrdersDto> RejectedOrder { get; set; }
    }
}