// <summary>
// <copyright file="ProductColorsDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Dtos.Models
{
    /// <summary>
    /// Class ProductColorsDto.
    /// </summary>
    public class ProductColorsDto
    {
       /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Id. </value>
        public string TemaId { get; set; }

        /// <summary>
        /// Gets or sets BackgroundColor.
        /// </summary>
        /// <value> Name. </value>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets LabelText.
        /// </summary>
        /// <value> Pieces. </value>
        public string LabelText { get; set; }

        /// <summary>
        /// Gets or sets TextColor.
        /// </summary>
        /// <value> Pieces. </value>
        public string TextColor { get; set; }
    }
}
