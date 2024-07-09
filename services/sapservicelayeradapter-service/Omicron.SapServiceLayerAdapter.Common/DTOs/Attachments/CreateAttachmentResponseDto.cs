// <summary>
// <copyright file="CreateAttachmentResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Attachments
{
    /// <summary>
    /// class for create attachment.
    /// </summary>
    public class CreateAttachmentResponseDto : CreateAttachmentDto
    {
        /// <summary>
        /// Gets or sets AbsoluteEntry.
        /// </summary>
        /// <value>The AbsoluteEntry.</value>
        [JsonProperty("AbsoluteEntry")]
        public int AbsoluteEntry { get; set; }
    }
}