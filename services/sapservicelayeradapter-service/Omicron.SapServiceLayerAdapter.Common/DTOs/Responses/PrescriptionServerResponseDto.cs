// <summary>
// <copyright file="PrescriptionServerResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Responses
{
    /// <summary>
    /// Class Prescription Server Response Dto.
    /// </summary>
    public class PrescriptionServerResponseDto
    {
        /// <summary>
        /// Gets or sets Azure Prescription Url.
        /// </summary>
        /// <value>The code.</value>
        public string AzurePrescriptionUrl { get; set; }

        /// <summary>
        /// Gets or sets Server Source Path.
        /// </summary>
        /// <value>The code.</value>
        public string ServerSourcePath { get; set; }

        /// <summary>
        /// Gets or sets Prescription File Name.
        /// </summary>
        /// <value>The code.</value>
        public string PrescriptionFileName { get; set; }

        /// <summary>
        /// Gets or sets Prescription File Extension.
        /// </summary>
        /// <value>The code.</value>
        public string PrescriptionFileExtension { get; set; }
    }
}
