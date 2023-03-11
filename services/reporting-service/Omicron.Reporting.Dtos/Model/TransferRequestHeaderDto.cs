// <summary>
// <copyright file="TransferRequestHeaderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Dtos.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for Transfer Request Header Dto.
    /// </summary>
    public class TransferRequestHeaderDto
    {
        /// <summary>
        /// Gets or sets UserInfo.
        /// </summary>
        /// <value>
        /// String UserInfo.
        /// </value>
        public string UserInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request is for labels.
        /// </summary>
        /// <value>Is label.</value>
        public bool IsLabel { get; set; }

        /// <summary>
        /// Gets or sets TransferRequestDetail.
        /// </summary>
        /// <value>
        /// String TransferRequestDetail.
        /// </value>
        public List<TransferRequestDetailDto> TransferRequestDetail { get; set; }
    }
}
