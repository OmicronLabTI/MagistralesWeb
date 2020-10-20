// <summary>
// <copyright file="UpdateDesignerLabelDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// class to update designer label.
    /// </summary>
    public class UpdateDesignerLabelDto
    {
        /// <summary>
        /// Gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public bool Checked { get; set; }

        /// <summary>
        /// Gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public string DesignerSignature { get; set; }
    }
}
