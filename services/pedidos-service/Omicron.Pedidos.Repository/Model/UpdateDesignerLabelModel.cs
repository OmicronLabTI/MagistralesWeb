// <summary>
// <copyright file="UpdateDesignerLabelModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// class to update designer label.
    /// </summary>
    public class UpdateDesignerLabelModel
    {
        /// <summary>
        /// Gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public List<UpdateDesignerLabelDetailModel> Details { get; set; }

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
