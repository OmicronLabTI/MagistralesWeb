// <summary>
// <copyright file="SendLocalPackageDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Dtos.Model
{
    /// <summary>
    /// Class for updating the package.
    /// </summary>
    public class SendLocalPackageDto
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public int PackageId { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string DestinyEmail { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string ReasonNotDelivered { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string SalesOrders { get; set; }

        /// <summary>
        /// Gets or sets Sales Person Email.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string SalesPersonEmail { get; set; }

        /// <summary>
        /// Gets or sets Sales Person Name.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string SalesPrsonName { get; set; }
    }
}
