// <summary>
// <copyright file="DoctorPaymentMethodDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Class DoctorPaymentMethodDto.
    /// </summary>
    public class DoctorPaymentMethodDto
    {
        /// <summary>
        /// Gets or sets PaymentMethodCode.
        /// </summary>
        /// <value>The PaymentMethodCode.</value>
        [JsonProperty("PaymentMethodCode")]
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets RowNumber.
        /// </summary>
        /// <value>The RowNumber.</value>
        [JsonProperty("RowNumber")]
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets or sets BPCode.
        /// </summary>
        /// <value>The BPCode.</value>
        [JsonProperty("BPCode")]
        public string BPCode { get; set; }
    }
}