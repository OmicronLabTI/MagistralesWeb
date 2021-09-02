// <summary>
// <copyright file="CreateSaleOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models.Experience
{
    using System.Collections.Generic;

    /// <summary>
    /// class for sale order.
    /// </summary>
    public class CreateSaleOrderDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string PrescriptionUrl { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<ShoppingCartItemDto> Items { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ShippinAddress { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the transaction record in the payment table (id field).
        /// </summary>
        /// <value>The code.</value>
        public int TransactionId { get; set; }
    }
}
