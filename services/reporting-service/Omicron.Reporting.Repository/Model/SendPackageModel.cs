// <summary>
// <copyright file="SendPackageModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Entities.Model
{
    /// <summary>
    /// class for the packageDto.
    /// </summary>
    public class SendPackageModel
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string TransportMode { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string DestinyEmail { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int PackageId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string SalesOrders { get; set; }

        /// <summary>
        /// Gets or sets a value sales person email.
        /// </summary>
        /// <value>
        /// Bool is production order.
        public string SalesPrsonEmail { get; set; }

        /// <summary>
        /// Gets or sets Sales Person Name.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsPatient.
        /// </summary>
        /// <value>
        /// String IsPatient.
        /// </value>
        public bool IsPatient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Address.
        /// </summary>
        /// <value>
        /// String Address.
        /// </value>
        public string Address { get; set; }
    }
}
