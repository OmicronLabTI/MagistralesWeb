// <summary>
// <copyright file="ProductionFacturaQrModel.cs" company="Axity">
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
    /// Class for the invoice qr model.
    /// </summary>
    [Table("productionfacturaqr")]
    public class ProductionFacturaQrModel
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
        [Column("facturaid")]
        public int FacturaId { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// Int Description.
        /// </value>
        [Column("facturaqrroute")]
        public string FacturaQrRoute { get; set; }
    }
}
