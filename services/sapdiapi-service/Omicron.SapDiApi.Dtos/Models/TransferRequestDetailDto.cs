// <summary>
// <copyright file="TransferRequestDetailDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models
{
    /// <summary>
    /// Class for Transfer Request Detail Dto.
    /// </summary>
    public class TransferRequestDetailDto
    {
        /// <summary>
        /// Gets or sets ItemCode.
        /// </summary>
        /// <value>
        /// String ItemCode.
        /// </value>
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        /// <value>
        /// Double Quantity.
        /// </value>
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets SourceWarehosue.
        /// </summary>
        /// <value>
        /// String SourceWarehosue.
        /// </value>
        public string SourceWarehosue { get; set; }

        /// <summary>
        /// Gets or sets TargetWarehosue.
        /// </summary>
        /// <value>
        /// String TargetWarehosue.
        /// </value>
        public string TargetWarehosue { get; set; }
    }
}
