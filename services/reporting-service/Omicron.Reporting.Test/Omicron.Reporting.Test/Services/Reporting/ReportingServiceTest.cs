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
        /// <param name="party">The party.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("DHL")]
        public async Task SendEmailForeignPackage(string party)
        {
            // arrange
            var request = new SendPackageModel
            {
                DestinyEmail = "email",
                PackageId = 1,
                TrackingNumber = "asdf",
                TransportMode = party,
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Field = "SmtpServer", Value = string.Empty },
                new ParametersModel { Field = "SmtpPort", Value = "0" },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = string.Empty },
                new ParametersModel { Field = "EmailMiddleware", Value = string.Empty },
                new ParametersModel { Field = "EmailCCDelivery", Value = string.Empty },
                new ParametersModel { Field = $"{party}Email", Value = "email@email.com" },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = await service.SendEmailForeignPackage(request);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="reasonNotDelivered"> reason why not was delivered.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("En Camino", null)]
        [TestCase("No Entregado", "domicilio no encontrado")]
        [TestCase("Entregado", null)]
        public async Task SendEmailLocalPackage(string status, string reasonNotDelivered)
        {
            // arrange
            var request = new SendLocalPackageModel
            {
                DestinyEmail = "email",
                PackageId = 1,
                Status = status,
                ReasonNotDelivered = reasonNotDelivered,
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

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <param name="party">The party.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase("DHL")]
        public async Task SendEmailRejectedOrder(string party)
        {
            // arrange
            var request = new SendRejectedEmailModel
            {
                RejectedOrder = new List<RejectedOrdersModel>
                {
                    new RejectedOrdersModel
                    {
                        CustomerName = "Name Customer",
                        DestinyEmail = "erika.rosas@axity.com",
                        SalesOrders = "99983",
                        Comments = "este es un comentario",
                    },
                    new RejectedOrdersModel
                    {
                        CustomerName = "Name Customer",
                        DestinyEmail = "erika.rosas@axity.com",
                        SalesOrders = "99983",
                        Comments = "este es un comentario",
                    },
                },
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Field = "SmtpServer", Value = string.Empty },
                new ParametersModel { Field = "SmtpPort", Value = "0" },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = string.Empty },
                new ParametersModel { Field = "EmailMiddleware", Value = string.Empty },
                new ParametersModel { Field = "EmailCCDelivery", Value = string.Empty },
                new ParametersModel { Field = "EmailAtencionAClientes", Value = string.Empty },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = await service.SendEmailRejectedOrder(request);
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the delivery cancel test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task SendEmailCancelDeliveryOrders()
        {
            // arrange
            var request = new List<SendCancelDeliveryModel>()
            {
                new SendCancelDeliveryModel
                {
                    DeliveryId = 1,
                    SalesOrders = "1478",
                    AsesorEmail = "email@email.com",
                },
                new SendCancelDeliveryModel
                {
                    DeliveryId = 2,
                    SalesOrders = "1479",
                    AsesorEmail = string.Empty,
                },
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Field = "SmtpServer", Value = string.Empty },
                new ParametersModel { Field = "SmtpPort", Value = "0" },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = string.Empty },
                new ParametersModel { Field = "EmailMiddleware", Value = string.Empty },
                new ParametersModel { Field = "EmailCCDelivery", Value = string.Empty },
                new ParametersModel { Field = "EmailAtencionAClientes", Value = string.Empty },
                new ParametersModel { Field = "EmailLogisticaCc2", Value = string.Empty },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            mockCatalog
                .Setup(m => m.GetSmtpConfig())
                .Returns(Task.FromResult(new SmtpConfigModel()));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = await service.SendEmailCancelDeliveryOrders(request);

            Assert.IsNotNull(result);
        }
    }
}
