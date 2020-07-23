// <summary>
// <copyright file="LoginModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Entities.Model
{
    /// <summary>
    /// The login model.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        /// <value>
        /// Password.
        /// </value>
        public string Password { get; set; }
    }
}
