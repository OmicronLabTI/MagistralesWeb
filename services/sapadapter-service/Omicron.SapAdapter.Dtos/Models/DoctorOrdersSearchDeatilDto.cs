// <summary>
// <copyright file="DoctorOrdersSearchDeatilDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// class for searhing the details of header and order.
    /// </summary>
    public class DoctorOrdersSearchDeatilDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<int> SaleOrders { get; set; }
    }
}
