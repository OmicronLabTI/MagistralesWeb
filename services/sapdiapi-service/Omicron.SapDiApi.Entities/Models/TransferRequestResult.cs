// <summary>
// <copyright file="TransferRequestResult.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Entities.Models
{
    /// <summary>
    /// Class for Transfer Request Result.
    /// </summary>
    public class TransferRequestResult
    {
        /// <summary>
        /// Gets or sets UserInfo.
        /// </summary>
        /// <value>
        /// String UserInfo.
        /// </value>
        public string UserInfo { get; set; }

        /// <summary>
        /// Gets or sets UserInfo.
        /// </summary>
        /// <value>
        /// String UserInfo.
        /// </value>
        public int TransferRequestId { get; set; }

        /// <summary>
        /// Gets or sets UserInfo.
        /// </summary>
        /// <value>
        /// String UserInfo.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request is for labels.
        /// </summary>
        /// <value>Is label.</value>
        public bool IsLabel { get; set; }
    }
}
