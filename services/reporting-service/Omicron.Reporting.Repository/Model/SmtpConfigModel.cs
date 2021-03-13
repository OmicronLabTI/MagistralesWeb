// <summary>
// <copyright file="SmtpConfigModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Entities.Model
{
    /// <summary>
    /// The smtp config model.
    /// </summary>
    public class SmtpConfigModel
    {
        /// <summary>
        /// Gets or sets smtp server.
        /// </summary>
        /// <value>The smtp server.</value>
        public string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets smtp port.
        /// </summary>
        /// <value>The smtp port.</value>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Gets or sets default user.
        /// </summary>
        /// <value>The default user.</value>
        public string SmtpDefaultUser { get; set; }

        /// <summary>
        /// Gets or sets default password.
        /// </summary>
        /// <value>The default password.</value>
        public string SmtpDefaultPassword { get; set; }

        /// <summary>
        /// Gets or sets default password.
        /// </summary>
        /// <value>The default password.</value>
        public string EmailCCDelivery { get; set; }

        /// <summary>
        /// Gets or sets default EmailAtencionCCDelivery.
        /// </summary>
        /// <value>The default EmailAtencionCCDelivery.</value>
        public string EmailAtencionCCDelivery { get; set; }
    }
}