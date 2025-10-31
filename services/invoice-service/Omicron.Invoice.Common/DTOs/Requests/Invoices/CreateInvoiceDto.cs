// <summary>
// <copyright file="CreateInvoiceDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Common.DTOs.Requests.Invoices
{
    /// <summary>
    /// Class request orders.
    /// </summary>
    public class CreateInvoiceDto
    {
        /// <summary>
        /// Gets or sets the Card Code.
        /// </summary>
        /// <value>CardCode.</value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the ProcessId.
        /// </summary>
        /// <value>ProcessId.</value>
        public string ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the CfdiDriverVersion.
        /// </summary>
        /// <value>CfdiDriverVersion.</value>
        public string CfdiDriverVersion { get; set; }

        /// <summary>
        /// Gets or sets the IdDeliveries.
        /// </summary>
        /// <value>IdDeliveries.</value>
        public List<int> IdDeliveries { get; set; }

        /// <summary>
        /// Gets or sets the IdDeliveries.
        /// </summary>
        /// <value>IdDeliveries.</value>
        public List<int> IdSapOrders { get; set; }

        /// <summary>
        /// Gets or sets the CreateUserId.
        /// </summary>
        /// <value>CreateUserId.</value>
        public string CreateUserId { get; set; }

        /// <summary>
        /// Gets or sets the DxpOrderId.
        /// </summary>
        /// <value>DxpOrderId.</value>
        public string DxpOrderId { get; set; }

        /// <summary>
        /// Gets or sets the InvoiceType.
        /// </summary>
        /// <value>InvoiceType.</value>
        public string InvoiceType { get; set; }

        /// <summary>
        /// Gets or sets the billing type (complete or partial).
        /// </summary>
        /// <value>
        /// Billing type string.
        /// </value>
        public string BillingType { get; set; }
    }
}
