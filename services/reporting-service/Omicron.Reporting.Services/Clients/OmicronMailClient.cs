// <summary>
// <copyright file="OmicronMailClient.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Clients
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Interface mail client.
    /// </summary>
    public class OmicronMailClient : IOmicronMailClient
    {
        private readonly ISendMailWrapper sendMailWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OmicronMailClient"/> class.
        /// </summary>
        /// <param name="sendMailWrapper">The catalogs service.</param>
        public OmicronMailClient(ISendMailWrapper sendMailWrapper)
        {
            this.sendMailWrapper = sendMailWrapper;
        }

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
        public async Task<bool> SendMail(SmtpConfigModel smtpConfig, string to, string subject, string body, string copyTo, Dictionary<string, MemoryStream> files = null)
        {
            try
            {
                var client = this.GetSmtpClient(smtpConfig);
                var message = new MailMessage(smtpConfig.SmtpDefaultUser, to, subject, body);
                message.IsBodyHtml = body.Contains("html");

                this.CreateMailAddressList(copyTo).ForEach(x => message.CC.Add(x.Address));

                if (files != null)
                {
                    files.Keys.ToList().ForEach(x => message.Attachments.Add(new Attachment(files[x], x)));
                }

                await this.sendMailWrapper.SendMailAsync(client, message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send email. Error : " + ex);
                return false;
            }
        }

        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="smtpConfig">Smtp config.</param>
        /// <returns>Smtp client.</returns>
        public SmtpClient GetSmtpClient(SmtpConfigModel smtpConfig)
        {
            return new SmtpClient(smtpConfig.SmtpServer, smtpConfig.SmtpPort)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 20000,
                Credentials = new System.Net.NetworkCredential(smtpConfig.SmtpDefaultUser, smtpConfig.SmtpDefaultPassword),
            };
        }

        /// <summary>
        /// Convert string to mail collection.
        /// </summary>
        /// <param name="mails">string with mails.</param>
        /// <returns>mail lists.</returns>
        private List<MailAddress> CreateMailAddressList(string mails)
        {
            return mails.Split(";").Where(x => !string.IsNullOrEmpty(x)).Select(x => new MailAddress(x)).ToList();
        }
    }
}
