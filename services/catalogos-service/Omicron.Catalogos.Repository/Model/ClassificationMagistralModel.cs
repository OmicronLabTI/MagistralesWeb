// <summary>
// <copyright file="ClassificationMagistralModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Model
{
    /// <summary>
    /// Table for params.
    /// </summary>
    public class ClassificationMagistralModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>
        /// String Value code.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// String Description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Color.
        /// </summary>
        /// <value>
        /// Color.
        /// </value>
        public string Color { get; set; }
    }
}
