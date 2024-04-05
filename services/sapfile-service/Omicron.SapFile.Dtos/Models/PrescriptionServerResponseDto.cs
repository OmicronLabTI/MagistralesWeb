// <summary>
// <copyright file="PrescriptionServerResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Dtos.Models
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
        /// Gets or sets Azure Prescription Url.
        /// </summary>
        /// <value>The code.</value>
        public string ServerPrescriptionUrl { get; set; }
    }
}
