// <summary>
// <copyright file="DeliveryNoteCreatedDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes
{
    /// <summary>
    /// The class for the DeliveryNoteDto.
    /// </summary>
    public class DeliveryNoteCreatedDto : DeliveryNoteDto
    {
        /// <summary>
        /// Gets or sets the Due Date.
        /// </summary>
        /// <value>Due Date.</value>
        [JsonProperty("DocEntry")]
        public int DocEntry { get; set; }

        /// <summary>
        /// Gets or sets the Delivery Note Lines.
        /// </summary>
        /// <value>Delivery Note Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<DeliveryNoteLineCreatedDto> DocumentLines { get; set; }
    }
}
