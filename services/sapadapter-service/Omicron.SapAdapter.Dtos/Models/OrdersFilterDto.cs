// <summary>
// <copyright file="OrdersFilterDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Dtos.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// class to get the recption.
    /// </summary>
    public class OrdersFilterDto
    {
        /// <summary>
        /// Gets or sets MagistralIds.
        /// </summary>
        /// <value>
        /// String MagistralIds.
        /// </value>
        public List<int> MagistralIds { get; set; }

        /// <summary>
        /// Gets or sets StartDate.
        /// </summary>
        /// <value>
        /// String StartDate.
        /// </value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets StartDate.
        /// </summary>
        /// <value>
        /// String StartDate.
        /// </value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets OrdersIds.
        /// </summary>
        /// <value>
        /// OrdersIds.
        /// </value>
        public List<int> OrdersIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets HasChips.
        /// </summary>
        /// <value>
        /// HasChips.
        /// </value>
        public bool HasChips { get; set; }
    }
}
