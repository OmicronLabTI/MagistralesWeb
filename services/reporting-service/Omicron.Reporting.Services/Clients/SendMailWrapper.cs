// <summary>
// <copyright file="SendMailWrapper.cs" company="Axity">
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
    /// Implementation of sed mail wrapper.
    /// </summary>
    public class SendMailWrapper : ISendMailWrapper
    {
        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="client">Email client.</param>
        /// <param name="message">Message to submit.</param>
        /// <returns>Nothing.</returns>
        public async Task SendMailAsync(SmtpClient client, MailMessage message)
        {
            client.SendCompleted += (s, e) =>
            {
                client.Dispose();
                message.Dispose();
            };

            await client.SendMailAsync(message);
        }
    }
}
