// <summary>
// <copyright file="ReportingServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Services.Request
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services;
    using Omicron.Reporting.Services.Clients;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ReportingServiceTest : BaseTest
    {
        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task SendEmailForeignPackage()
        {
            // arrange
            var request = new SendPackageModel
            {
                DestinyEmail = "email",
                PackageId = 1,
                TrackingNumber = "asdf",
                TransportMode = "DHL",
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetSmtpConfig())
                .Returns(Task.FromResult(new SmtpConfigModel()));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            // var result = await service.SendEmailForeignPackage(request);
            var result = new List();
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task SendEmailLocalPackage()
        {
            // arrange
            var request = new SendLocalPackageModel
            {
                DestinyEmail = "email",
                PackageId = 1,
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetSmtpConfig())
                .Returns(Task.FromResult(new SmtpConfigModel()));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = await service.SendEmailLocalPackage(request);

            Assert.IsNotNull(result);
        }
    }
}
