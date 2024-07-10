// <summary>
// <copyright file="InventoryTransferRequestResult.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryTransferRequest
{
    /// <summary>
    /// Class for Inventory Transfer Request Result.
    /// </summary>
    public class InventoryTransferRequestResult
    {
        /// <summary>
        /// Gets or sets UserInfo.
        /// </summary>
        /// <value>
        /// UserInfo.
        /// </value>
        public string UserInfo { get; set; }

        /// <summary>
        /// Gets or sets Transfer Request Id.
        /// </summary>
        /// <value>
        /// Transfer Request Id.
        /// </value>
        public int TransferRequestId { get; set; }

        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        /// <value>
        /// Error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request is for labels.
        /// </summary>
        /// <value>Is label.</value>
        public bool IsLabel { get; set; }
    }
}
