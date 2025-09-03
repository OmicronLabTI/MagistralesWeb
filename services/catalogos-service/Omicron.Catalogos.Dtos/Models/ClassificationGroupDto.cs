// <summary>
// <copyright file="ClassificationGroupDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Class classification group.
    /// </summary>
    public class ClassificationGroupDto
    {
        /// <summary>
        /// Gets or sets filters.
        /// </summary>
        /// <value> collection of classifications for filters.</value>
        public List<ClassificationDto> Filters { get; set; }

        /// <summary>
        /// Gets or sets colors.
        /// </summary>
        /// <value> collection of classifications for colors.</value>
        public List<ColorsDto> Colors { get; set; }
    }
}
