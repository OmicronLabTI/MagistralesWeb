// <summary>
// <copyright file="ProductTypeColorsModel.cs" company="Axity">
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
    /// Model ProductTypeColors.
    /// </summary>
    ///
    [Table("productypecolors")]
    public class ProductTypeColorsModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value> Id. </value>
        [Key]
        [Column("id")]
        public string TemaId { get; set; }

        /// <summary>
        /// Gets or sets BackgroundColor.
        /// </summary>
        /// <value> Name. </value>
        [Column("backgroundcolor")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets LabelText.
        /// </summary>
        /// <value> Pieces. </value>
        [Column("labeltext")]
        public string LabelText { get; set; }

        /// <summary>
        /// Gets or sets TextColor.
        /// </summary>
        /// <value> Pieces. </value>
        [Column("textcolor")]
        public string TextColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Is active.
        /// </summary>
        /// <value> Is active. </value>
        [Column("isactive")]
        public bool IsActive { get; set; }
    }
}
