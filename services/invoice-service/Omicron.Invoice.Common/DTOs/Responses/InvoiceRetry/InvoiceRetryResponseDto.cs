// <summary>
// <copyright file="InvoiceRetryResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Common.DTOs.Responses.InvoiceRetry
{
    /// <summary>
    /// InvoiceRetryResponseDto class.
    /// </summary>
    public class InvoiceRetryResponseDto
    {
        /// <summary>
        /// Gets or sets ProcessedIds.
        /// </summary>
        /// <value>
        /// string ProcessedIds.
        /// </value>
        public List<string> ProcessedIds { get; set; }

        /// <summary>
        /// Gets or sets SkippedIds.
        /// </summary>
        /// <value>
        /// string SkippedIds.
        /// </value>
        public List<string> SkippedIds { get; set; }
    }
}
