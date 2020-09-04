// <summary>
// <copyright file="ComponentCustomComponentListModel.cs" company="Axity">
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
    /// Component of custom formula.
    /// </summary>
    [Table("customcomponentlistscomponents")]
    public class ComponentCustomComponentListModel
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [Column("customlistid")]
        public int CustomListId { get; set; }

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
        /// Gets or sets base quantity.
        /// </summary>
        /// <value>The base quantity.</value>
        [Column("basequantity")]
        public decimal BaseQuantity { get; set; }
    }
}
