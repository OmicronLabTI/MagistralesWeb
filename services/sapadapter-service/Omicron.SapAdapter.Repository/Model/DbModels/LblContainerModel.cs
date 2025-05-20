// <summary>
// <copyright file="LblContainerModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.DbModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class label container.
    /// </summary>
    [Table("UFD1")]
    public class LblContainerModel
    {
        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        /// <value>The value.</value>
        [Key]
        [Column("FldValue")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>The Description.</value>
        [Column("Descr")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets TableId.
        /// </summary>
        /// <value>The TableId.</value>
        [Column("TableID")]
        public string TableId { get; set; }

        /// <summary>
        /// Gets or sets FieldId.
        /// </summary>
        /// <value>The FieldId.</value>
        [Column("FieldID")]
        public short FieldId { get; set; }
    }
}
