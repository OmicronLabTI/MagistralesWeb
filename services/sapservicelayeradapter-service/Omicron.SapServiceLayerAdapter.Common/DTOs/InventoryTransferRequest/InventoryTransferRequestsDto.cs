// <summary>
// <copyright file="InventoryTransferRequestsDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.InventoryTransferRequest
{
    /// <summary>
    /// Class for Inventory Transfer Requests Dto.
    /// </summary>
    public class InventoryTransferRequestsDto
    {
        /// <summary>
        /// Gets or sets JournalMemo.
        /// </summary>
        /// <value>The JournalMemo.</value>
        [JsonProperty("JournalMemo")]
        public string JournalMemo { get; set; }

        /// <summary>
        /// Gets or sets DocumentDate.
        /// </summary>
        /// <value>The DocumentDate.</value>
        [JsonProperty("DocDate")]
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Gets or sets RequestedUserid.
        /// </summary>
        /// <value>The ItemCode.</value>
        [JsonProperty("U_RequestedUserid")]
        public string RequestedUserId { get; set; }

        /// <summary>
        /// Gets or sets StockTransferLines.
        /// </summary>
        /// <value>The StockTransferLines.</value>
        [JsonProperty("StockTransferLines")]
        public List<StockTransferLinesDto> StockTransferLines { get; set; }
    }
}
