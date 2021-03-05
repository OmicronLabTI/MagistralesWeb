// <summary>
// <copyright file="FinishOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// the finishOrder Dto.
    /// </summary>
    public class FinishOrderDto
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The code.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets fabrication order id.
        /// </summary>
        /// <value>The fabrication order id.</value>
        public List<int> FabricationOrderId { get; set; }

        /// <summary>
        /// Gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public string QfbSignature { get; set; }

        /// <summary>
        /// Gets or sets the signature image on base 64.
        /// </summary>
        /// <value>The signature.</value>
        public string TechnicalSignature { get; set; }
    }
}
