// <summary>
// <copyright file="PrescriptionServerRequestDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Requests
{
    /// <summary>
    /// Class Prescription Server Dto.
    /// </summary>
    public class PrescriptionServerRequestDto
    {
        /// <summary>
        /// Gets or sets Azure Recipe Url.
        /// </summary>
        /// <value>The code.</value>
        public string AzurePrescriptionUrl { get; set; }
    }
}
