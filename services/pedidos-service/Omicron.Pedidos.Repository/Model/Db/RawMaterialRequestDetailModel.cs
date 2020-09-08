// <summary>
// <copyright file="RawMaterialRequestDetailModel.cs" company="Axity">
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
    /// Class for request raw materia.
    /// </summary>
    [Table("rawmaterialrequestdetail")]
    public class RawMaterialRequestDetailModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        /// <value>
        /// Request id.
        /// </value>
        [Column("rawmaterialrequestid")]
        public int RequestId { get; set; }

        /// <summary>
        /// Gets or sets product id.
        /// </summary>
        /// <value>The code.</value>
        [Column("productid")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets product description.
        /// </summary>
        /// <value>The code.</value>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets request quantity.
        /// </summary>
        /// <value>The base quantity.</value>
        [Column("requestquantity")]
        public decimal RequestQuantity { get; set; }
    }
}
