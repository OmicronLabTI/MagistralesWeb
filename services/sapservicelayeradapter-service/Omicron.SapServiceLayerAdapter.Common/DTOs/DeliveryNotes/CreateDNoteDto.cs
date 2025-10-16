// <summary>
// <copyright file="CreateDNoteDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes
{
    /// <summary>
    /// Class for Cancel Delivery Dto.
    /// </summary>
    public class CreateDNoteDto : BaseDeliveryNoteDto
    {
        /// <summary>
        /// Gets or sets the Delivery Note Lines.
        /// </summary>
        /// <value>Delivery Note Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<BaseDeliveryNoteLineDto> DeliveryNoteLines { get; set; }
    }
}
