// <summary>
// <copyright file="OmicronMailClientTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Test.Services.SapAdapter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Clients;

    /// <summary>
    /// Test class for OmicronMailClient.
    /// </summary>
    [TestFixture]
    public class OmicronMailClientTest
    {
        private OmicronMailClient omicronMailClient;

        /// <summary>
        /// Set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var sendMailWrapperMock = new Mock<ISendMailWrapper>();
            sendMailWrapperMock.Setup(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>())).Returns(Task.CompletedTask);

            this.omicronMailClient = new OmicronMailClient(sendMailWrapperMock.Object);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetSmtpClient()
        {
            // Arrange
            var smtpConfig = this.GetMockSmtpConfigModel();

            // Act
            var result = this.omicronMailClient.GetSmtpClient(smtpConfig);

            // Assert
            Assert.AreEqual(smtpConfig.SmtpServer, result.Host);
            Assert.AreEqual(smtpConfig.SmtpPort, result.Port);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SendMail()
        {
            // Arrange
            var smtpConfig = this.GetMockSmtpConfigModel();
            var files = new Dictionary<string, MemoryStream>();

            // Act
            var result = await this.omicronMailClient.SendMail(smtpConfig, "to@mail.com", "subject", "body", "cc1@mail.com;cc2@mail.com", files);

            // Assert
            Assert.IsTrue(result);
        }

        private SmtpConfigModel GetMockSmtpConfigModel()
        {
            return new SmtpConfigModel
            {
                SmtpDefaultPassword = "12345",
                SmtpDefaultUser = "mail@gmail.com",
                SmtpServer = "smtp.domain.com",
                SmtpPort = 123,
            };
        }
    }
}
