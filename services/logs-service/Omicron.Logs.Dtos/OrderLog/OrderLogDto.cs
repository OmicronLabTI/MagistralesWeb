// <summary>
// <copyright file="OrderLogDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Dtos.OrderLog
{
    using System;
    using System.Collections.Generic;
    using Omicron.Logs.Dtos.UserOrder;

    /// <summary>
    /// Class OrderLog Dto.
    /// </summary>
    public class OrderLogDto
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        /// <value>
        /// Int id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets logdatetime.
        /// </summary>
        /// <value>
        /// datetime logdatetime.
        /// </value>
        public DateTime Logdatetime { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// String userid.
        /// </value>
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        /// <value>
        /// String type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets noid.
        /// </summary>
        /// <value>
        /// String noid.
        /// </value>
        public string Noid { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        /// <value>
        /// String description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets detalle.
        /// </summary>
        /// <value>
        /// String detalle.
        /// </value>
        public List<UserOrderDto> Detalle { get; set; }
    }
}
