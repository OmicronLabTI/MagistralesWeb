// <summary>
// <copyright file="InvoicePaymentInfoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Invoices
{
    /// <summary>
    /// The class for the InvoiceLineDto.
    /// </summary>
    public class InvoicePaymentInfoDto
    {

        /// <summary>
        /// Gets or sets the  DocumentNumber.
        /// </summary>
        /// <value> DocumentNumber.</value>
        [JsonProperty("DocNum")]
        public int DocumentNumber { get; set; }

        /// <summary>
        /// Gets or sets the Document FormaPago33.
        /// </summary>
        /// <value>Document FormaPago33.</value>
        [JsonProperty("U_BXP_FORMAPAGO33")]
        public string FormaPago33 { get; set; }
    }
}
