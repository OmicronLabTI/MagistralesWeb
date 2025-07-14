// <summary>
// <copyright file="ConfigWarehouseModel.cs" company="Axity">
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
    /// Model configwarehouses.
    /// </summary>
    [Table("configwarehouses")]
    public class ConfigWarehouseModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Id. </value>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// <value> Mainwarehouse. </value>
        [Column("mainwarehouse")]
        public string Mainwarehouse { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> Manufacturers. </value>
        [Column("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> Products. </value>
        [Column("products")]
        public string Products { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> exceptions. </value>
        [Column("exceptions")]
        public string Exceptions { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> alternativewarehouses. </value>
        [Column("alternativewarehouses")]
        public string Alternativewarehouses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Is active.
        /// </summary>
        /// <value> Is active. </value>
        [Column("isactive")]
        public bool IsActive { get; set; }
    }
}
