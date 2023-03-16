// <summary>
// <copyright file="RawMaterialRequestDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.DbModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The item.
    /// </summary>
    [Table("WTQ1")]
    public class RawMaterialRequestDetailModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Key]
        [Column("DocEntry")]
        public int DocEntry { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("Dscription")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("Quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets TargetWarehosue.
        /// </summary>
        /// <value>
        /// String TargetWarehosue.
        /// </value>
        [Column("WhsCode")]
        public string TargetWarehosue { get; set; }

        /// <summary>
        /// Gets or sets Unit.
        /// </summary>
        /// <value>
        /// String Unit.
        /// </value>
        [Column("unitMsr")]
        public string Unit { get; set; }
    }
}
