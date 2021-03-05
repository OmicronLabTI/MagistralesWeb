// <summary>
// <copyright file="RejectedOrdersModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Entities.Model
{
    /// <summary>
    /// The file result model.
    /// </summary>
    public class RejectedOrdersModel
    {
        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string DestinyEmail { get; set; }

        /// <summary>
        /// Gets or sets Orders.
        /// </summary>
        /// <value>The code.</value>
        public string SalesOrders { get; set; }

        /// <summary>
        /// Gets or sets Coments.
        /// </summary>
        /// <value>The response.</value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Parameter id.
        /// </value>
        public int Idsmtp { get; set; }
    }
}