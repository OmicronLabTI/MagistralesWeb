// <summary>
// <copyright file="CreateInvoiceDocumentDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices
{
    /// <summary>
    /// The class for the CreateInvoiceDocumentDto.
    /// </summary>
    public class CreateInvoiceDocumentDto
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
    }
}
