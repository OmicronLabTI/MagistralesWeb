// <summary>
// <copyright file="RoleModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Entities.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table for roles.
    /// </summary>
    [Table("roles")]
    public class RoleModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        [Column("idrole")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>
        /// Int Description.
        /// </value>
        [Column("roledescription")]
        public string Description { get; set; }
    }
}
