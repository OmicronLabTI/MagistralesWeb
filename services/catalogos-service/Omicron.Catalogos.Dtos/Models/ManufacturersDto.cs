// <summary>
// <copyright file="ManufacturersDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Dtos.Models
{
    /// <summary>
    /// Class manufacturers.
    /// </summary>
    public class ManufacturersDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Id. </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value> Name. </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Is active.
        /// </summary>
        /// <value> Is active. </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> Pieces. </value>
        public int? Pieces { get; set; }

        /// <summary>
        /// Gets or sets Clasification.
        /// </summary>
        /// <value> Clasification.</value>
        public string Classification { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets ApplySpecialContainerRule.
        /// </summary>
        /// <value> The ApplySpecialContainerRule. </value>
        public bool ApplySpecialContainerRule { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets ApplySpecialContainerRule.
        /// </summary>
        /// <value> The ApplySpecialContainerRule. </value>
        public int? MaxProductsByOrder { get; set; }

        /// <summary>
        /// Gets or sets Clasification.
        /// </summary>
        /// <value> Clasification.</value>
        public string ClassificationCode { get; set; }
    }
}
