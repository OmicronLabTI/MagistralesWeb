// <summary>
// <copyright file="ProductionOrderQr.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model.Db
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Gets the production order qr.
    /// </summary>
    [Table("productionorderqr")]
    public class ProductionOrderQr
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        [Column("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// Int Description.
        /// </value>
        [Column("userorderid")]
        public int UserOrderId { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// Int Description.
        /// </value>
        [Column("magistralqrroute")]
        public string MagistralQrRoute { get; set; }
    }
}
