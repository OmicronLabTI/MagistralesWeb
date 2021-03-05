// <summary>
// <copyright file="UserDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Dtos.User
{
    /// <summary>
    /// Class User Dto.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets UserName.
        /// </summary>
        /// <value>
        /// String UserName.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        /// <value>
        /// String LastName.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets Role.
        /// </summary>
        /// <value>
        /// String Role.
        /// </value>
        public int Role { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        /// <value>
        /// String Password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Activo.
        /// </summary>
        /// <value>
        /// String Activo.
        /// </value>
        public int Activo { get; set; }

        /// <summary>
        /// Gets or sets Activo.
        /// </summary>
        /// <value>
        /// String Activo.
        /// </value>
        public int Piezas { get; set; }

        /// <summary>
        /// Gets or sets Activo.
        /// </summary>
        /// <value>
        /// String Activo.
        /// </value>
        public int Asignable { get; set; }

        /// <summary>
        /// Gets or sets the QFB classification.
        /// </summary>
        /// <value>
        /// String Activo.
        /// </value>
        public string Classification { get; set; }
    }
}
