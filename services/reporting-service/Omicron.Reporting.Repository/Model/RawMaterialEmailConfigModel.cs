// <summary>
// <copyright file="RawMaterialEmailConfigModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Entities.Model
{
    /// <summary>
    /// The raw material email config model.
    /// </summary>
    public class RawMaterialEmailConfigModel
    {
        /// <summary>
        /// Gets or sets addressee.
        /// </summary>
        /// <value>The smtp server.</value>
        public string Addressee { get; set; }

        /// <summary>
        /// Gets or sets copy to emails.
        /// </summary>
        /// <value>The copy to emails.</value>
        public string CopyTo { get; set; }
    }
}