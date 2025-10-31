// <summary>
// <copyright file="InvoiceRetryRequestDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Common.DTOs.Requests.InvoiceRetry
{
    /// <summary>
    /// InvoiceRetryRequestDto class.
    /// </summary>
    public class InvoiceRetryRequestDto
    {
        /// <summary>
        /// Gets or sets Offset.
        /// </summary>
        /// <value>
        /// string Offset.
        /// </value>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets Limit.
        /// </summary>
        /// <value>
        /// string Limit.
        /// </value>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets InvoiceIds.
        /// </summary>
        /// <value>
        /// string InvoiceIds.
        /// </value>
        public List<string> InvoiceIds { get; set; }

        /// <summary>
        /// Gets or sets RequestingUser.
        /// </summary>
        /// <value>
        /// string RequestingUser.
        /// </value>
        public string RequestingUser { get; set; }
    }
}