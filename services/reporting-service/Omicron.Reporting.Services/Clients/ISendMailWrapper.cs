// <summary>
// <copyright file="ISendMailWrapper.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Clients
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for smto client wrapper.
    /// </summary>
    public interface ISendMailWrapper
    {
        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="client">Email client.</param>
        /// <param name="message">Message to submit.</param>
        /// <returns>Nothing.</returns>
        Task SendMailAsync(SmtpClient client, MailMessage message);
    }
}
