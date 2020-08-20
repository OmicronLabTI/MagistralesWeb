// <summary>
// <copyright file="AsesorModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The asesor table.
    /// </summary>
    [Table("OSLP")]
    public class AsesorModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Key]
        [Column("SlpCode")]
        public int AsesorId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        [Column("SlpName")]
        public string AsesorName { get; set; }
    }
}
