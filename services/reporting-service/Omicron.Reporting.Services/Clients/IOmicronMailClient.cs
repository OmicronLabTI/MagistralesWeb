// <summary>
// <copyright file="IOmicronMailClient.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Clients
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Interface mail client.
    /// </summary>
    public interface IOmicronMailClient
    {
        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="smtpConfig">Smtp config.</param>
        /// <param name="to">To mail.</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <param name="copyTo">Copy to.</param>
        /// <param name="files">Attached files.</param>
        /// <returns>Submit status flag.</returns>
        Task<bool> SendMail(SmtpConfigModel smtpConfig, string to, string subject, string body, string copyTo, Dictionary<string, MemoryStream> files = null);
    }
}
