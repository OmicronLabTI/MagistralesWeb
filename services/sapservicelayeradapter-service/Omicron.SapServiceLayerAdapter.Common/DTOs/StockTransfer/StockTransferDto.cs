// <summary>
// <copyright file="StockTransferDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.StockTransfer
{
    /// <summary>
    /// The class for the StockTransferDto.
    /// </summary>
    public class StockTransferDto
    {
        /// <summary>
        /// Gets or sets the DocumentDate.
        /// </summary>
        /// <value>Document Date.</value>
        [JsonProperty("DocDate")]
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Gets or sets the DocumentSubType.
        /// </summary>
        /// <value>Document SubType.</value>
        [JsonProperty("FromWarehouse")]
        public string FromWarehouse { get; set; }

        /// <summary>
        /// Gets or sets the DocumentSubType.
        /// </summary>
        /// <value>Document SubType.</value>
        [JsonProperty("ToWarehouse")]
        public string ToWarehouse { get; set; }

        /// <summary>
        /// Gets or sets the DocumentSubType.
        /// </summary>
        /// <value>Document SubType.</value>
        [JsonProperty("JournalMemo")]
        public string JournalMemo { get; set; }

        /// <summary>
        /// Gets or sets the DocumentSubType.
        /// </summary>
        /// <value>Document SubType.</value>
        [JsonProperty("StockTransferLines")]
        public List<StockTransferLineDto> StockTransferLines { get; set; }
    }
}
