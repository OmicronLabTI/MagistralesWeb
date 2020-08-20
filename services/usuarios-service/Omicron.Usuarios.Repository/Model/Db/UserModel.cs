// <summary>
// <copyright file="UserModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Entities.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Class User Model.
    /// </summary>
    [Table("user")]
    public class UserModel
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        [Key]
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets UserName.
        /// </summary>
        /// <value>
        /// String UserName.
        /// </value>
        [Column("username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        [Column("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        /// <value>
        /// String LastName.
        /// </value>
        [Column("lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets Role.
        /// </summary>
        /// <value>
        /// String Role.
        /// </value>
        [Column("role")]
        public int Role { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        /// <value>
        /// String Password.
        /// </value>
        [Column("password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Active.
        /// </summary>
        /// <value>
        /// String Active.
        /// </value>
        [Column("activo")]
        public int Activo { get; set; }
    }
}
