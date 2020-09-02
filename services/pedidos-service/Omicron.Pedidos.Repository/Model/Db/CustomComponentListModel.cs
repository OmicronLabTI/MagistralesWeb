// <summary>
// <copyright file="CustomComponentListModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Entities.Model.Db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Custom formula.
    /// </summary>
    [Table("customcomponentlists")]
    public class CustomComponentListModel
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
        /// Gets or sets name.
        /// </summary>
        /// <value>
        /// The formula name.
        /// </value>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        [Column("productid")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        [NotMapped]
        public List<ComponentCustomComponentListModel> Components { get; set; }

        /// <summary>
        /// Gets or sets creation user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [Column("creationuserid")]
        public string CreationUserId { get; set; }

        /// <summary>
        /// Gets or sets creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        [Column("creationdate")]
        public string CreationDate { get; set; }
    }
}
