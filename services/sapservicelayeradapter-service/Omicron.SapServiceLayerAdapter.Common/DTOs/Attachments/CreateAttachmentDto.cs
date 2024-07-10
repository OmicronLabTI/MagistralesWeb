// <summary>
// <copyright file="CreateAttachmentDto.cs" company="Axity">
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
    public class CreateAttachmentDto
    {
        /// <summary>
        /// Gets or sets AttachmentLines.
        /// </summary>
        /// <value>The AttachmentLines.</value>
        [JsonProperty("Attachments2_Lines")]
        public List<AttachmentDto> AttachmentLines { get; set; }
    }
}