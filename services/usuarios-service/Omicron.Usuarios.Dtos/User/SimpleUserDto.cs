// <summary>
// <copyright file="ecnicDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Dtos.User
{
    /// <summary>
    /// Class Tecnic Dto.
    /// </summary>
    public class SimpleUserDto
    {
        /// <summary>
        /// Gets or sets TecnicId.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public string Id { get; set; }

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
    }
}
