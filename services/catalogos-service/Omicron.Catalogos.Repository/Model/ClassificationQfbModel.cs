// <summary>
// <copyright file="ClassificationQfbModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table for params.
    /// </summary>
    [Table("classificationqfb")]
    public class ClassificationQfbModel
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
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// Int Description.
        /// </value>
        [Column("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// Int Description.
        /// </value>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether logic deleted flag.
        /// </summary>
        /// <value>
        /// Deleted flag.
        /// </value>
        [Column("active")]
        public bool Active { get; set; }
    }
}
