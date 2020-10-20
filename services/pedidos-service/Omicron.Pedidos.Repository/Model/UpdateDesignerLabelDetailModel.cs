// <summary>
// <copyright file="UpdateDesignerLabelDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    /// <summary>
    /// class for the detail.
    /// </summary>
    public class UpdateDesignerLabelDetailModel
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
    }
}
