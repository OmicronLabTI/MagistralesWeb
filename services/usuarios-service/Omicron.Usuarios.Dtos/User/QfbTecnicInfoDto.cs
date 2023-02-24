// <summary>
// <copyright file="QfbTecnicInfoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Dtos.User
{
    /// <summary>
    /// Class Qfb Tecnic Info Dto.
    /// </summary>
    public class QfbTecnicInfoDto
    {
        /// <summary>
        /// Gets or sets TecnicId.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public string QfbId { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string QfbFirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        /// <value>
        /// String LastName.
        /// </value>
        public string QfbLastName { get; set; }

        /// <summary>
        /// Gets or sets TecnicId.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public string TecnicId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets is tecnic required.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public bool IsTecnicRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets is tecnic required.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public bool IsValidTecnic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets is tecnic required.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public bool IsValidQfbConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets is tecnic required.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public bool IsValidTecnicConfiguration { get; set; }
    }
}
