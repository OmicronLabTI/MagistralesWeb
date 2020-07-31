// <summary>
// <copyright file="UserDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Logs.Dtos.OrderLog
{
    using System;
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
        public int id { get; set; }

        /// <summary>
        /// Gets or sets logdatetime.
        /// </summary>
        /// <value>
        /// datetime logdatetime.
        /// </value>
        public DateTime logdatetime { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// String userid.
        /// </value>
        public string userid { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        /// <value>
        /// String type.
        /// </value>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets noid.
        /// </summary>
        /// <value>
        /// String noid.
        /// </value>
        public string noid { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        /// <value>
        /// String description.
        /// </value>
        public string description { get; set; }

        /// <summary>
        /// Gets or sets detalle.
        /// </summary>
        /// <value>
        /// String detalle.
        /// </value>
        public List<UsersOrder> detalle { get; set; }
    }
}
