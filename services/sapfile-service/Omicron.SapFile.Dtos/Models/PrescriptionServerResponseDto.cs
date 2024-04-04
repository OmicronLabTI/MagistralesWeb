// <summary>
// <copyright file="PrescriptionServerResponseDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using System.Diagnostics;

namespace Omicron.SapFile.Dtos.Models
{
    /// <summary>
    /// Class Prescription Server Response Dto.
    /// </summary>
    public class PrescriptionServerResponseDto
    {
        /// <summary>
        /// Gets or sets Azure Recipe Url.
        /// </summary>
        /// <value>The code.</value>
        public string AzureRecipeUrl { get; set; }

        /// <summary>
        /// Gets or sets Azure Recipe Url.
        /// </summary>
        /// <value>The code.</value>
        public string ServerRecipeUrl { get; set; }

        /// <summary>
        /// Gets or sets Error.
        /// </summary>
        /// <value>The code.</value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets It Download Correctly.
        /// </summary>
        /// <value>The code.</value>
        public bool ItDownloadCorrectly { get; set; }
    }
}
