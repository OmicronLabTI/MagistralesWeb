// <summary>
// <copyright file="WarehouseModel.cs" company="Axity">
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
    /// Model warehouse.
    /// </summary>
    [Table("warehouses")]
    public class WarehouseModel
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
        /// <value> Name. </value>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> Pieces. </value>
        [Column("applytoproducts")]
        public string AppliesToProducts { get; set; }

        /// <summary>
        /// Gets or sets a value Pieces.
        /// </summary>
        /// <value> Pieces. </value>
        [Column("applytomanufacturers")]
        public string AppliesToManufacturers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Is active.
        /// </summary>
        /// <value> Is active. </value>
        [Column("isactive")]
        public bool IsActive { get; set; }
    }
}
