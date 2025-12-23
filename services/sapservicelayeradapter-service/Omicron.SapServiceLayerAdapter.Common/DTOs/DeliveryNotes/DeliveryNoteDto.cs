// <summary>
// <copyright file="DeliveryNoteDto.cs" company="Axity">
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
    public class DeliveryNoteDto : BaseDeliveryNoteDto
    {
        /// <summary>
        /// Gets or sets the Delivery Note Lines.
        /// </summary>
        /// <value>Delivery Note Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<DeliveryNoteLineDto> DeliveryNoteLines { get; set; }

        /// <summary>
        /// Gets or sets the Delivery Note Lines.
        /// </summary>
        /// <value>Delivery Note Lines.</value>
        [JsonProperty("U_ENVASE")]
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the Delivery Note Lines.
        /// </summary>
        /// <value>Delivery Note Lines.</value>
        [JsonProperty("U_ETIQUETA")]
        public string Label { get; set; }
    }
}
