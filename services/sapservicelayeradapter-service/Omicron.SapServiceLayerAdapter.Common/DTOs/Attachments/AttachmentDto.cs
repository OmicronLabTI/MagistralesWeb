// <summary>
// <copyright file="AttachmentDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Attachments
{
    /// <summary>
    /// class for sale orde model.
    /// </summary>
    public class AttachmentDto
    {
        /// <summary>
        /// Gets or sets AbsoluteEntry.
        /// </summary>
        /// <value>The AbsoluteEntry.</value>
        [JsonProperty("AbsoluteEntry")]
        public int? AbsoluteEntry { get; set; }

        /// <summary>
        /// Gets or sets LineNum.
        /// </summary>
        /// <value>The LineNum.</value>
        [JsonProperty("LineNum")]
        public int LineNum { get; set; }

        /// <summary>
        /// Gets or sets SourcePath.
        /// </summary>
        /// <value>The SourcePath.</value>
        [JsonProperty("SourcePath")]
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets FileName.
        /// </summary>
        /// <value>The FileName.</value>
        [JsonProperty("FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets FileExtension.
        /// </summary>
        /// <value>The FileExtension.</value>
        [JsonProperty("FileExtension")]
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets AttachmentDate.
        /// </summary>
        /// <value>The AttachmentDate.</value>
        [JsonProperty("AttachmentDate")]
        public string AttachmentDate { get; set; }

        /// <summary>
        /// Gets or sets Override.
        /// </summary>
        /// <value>The Override.</value>
        [JsonProperty("Override")]
        public string Override { get; set; }

        /// <summary>
        /// Gets or sets FreeText.
        /// </summary>
        /// <value>The FreeText.</value>
        [JsonProperty("FreeText")]
        public string? FreeText { get; set; }

        /// <summary>
        /// Gets or sets CopyToTargetDoc.
        /// </summary>
        /// <value>The CopyToTargetDoc.</value>
        [JsonProperty("CopyToTargetDoc")]
        public string CopyToTargetDoc { get; set; }

        /// <summary>
        /// Gets or sets CopyToProductionOrder.
        /// </summary>
        /// <value>The CopyToProductionOrder.</value>
        [JsonProperty("CopyToProductionOrder")]
        public string CopyToProductionOrder { get; set; }
    }
}