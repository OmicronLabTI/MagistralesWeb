// <summary>
// <copyright file="ReportingServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Services.Request
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Omicron.Reporting.Dtos.Model;
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
                new ParametersModel { Field = "EmailLogoUrl", Value = "string" },
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
                SalesOrders = "100",
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Field = "SmtpServer", Value = string.Empty },
                new ParametersModel { Field = "SmtpPort", Value = "0" },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = string.Empty },
                new ParametersModel { Field = "EmailMiddleware", Value = string.Empty },
                new ParametersModel { Field = "EmailCCDelivery", Value = string.Empty },
                new ParametersModel { Field = "EmailLogoUrl", Value = "string" },
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
                new ParametersModel { Field = "EmailCCRejected", Value = string.Empty },
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
                    SalesOrders = new List<int> { 1478 },
                    DeliveryType = "BE",
                    DeliveryOrderType = string.Empty,
                },
                new SendCancelDeliveryModel
                {
                    DeliveryId = 2,
                    SalesOrders = new List<int> { 1479 },
                    DeliveryType = "MN",
                    DeliveryOrderType = "Mixto",
                },
                new SendCancelDeliveryModel
                {
                    DeliveryId = 3,
                    SalesOrders = new List<int> { 1479 },
                    DeliveryType = "MX",
                    DeliveryOrderType = string.Empty,
                },
                new SendCancelDeliveryModel
                {
                    DeliveryId = 4,
                    SalesOrders = new List<int> { 1479 },
                    DeliveryType = "MG",
                    DeliveryOrderType = string.Empty,
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
                new ParametersModel { Field = "EmailLogisticaCc3", Value = string.Empty },
                new ParametersModel { Field = "EmailBioEqual", Value = string.Empty },
                new ParametersModel { Field = "EmailBioElite", Value = string.Empty },
                new ParametersModel { Field = "EmailLogoUrl", Value = "string" },
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

        /// <summary>
        /// gets the delivery cancel test.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequestPdfWithSignature()
        {
            // arrange
            var rawMaterial = new List<RawMaterialRequestDetailModel>()
            {
                new RawMaterialRequestDetailModel()
                {
                    Id = 1,
                    Description = "description",
                    ProductId = "reve 14",
                    RequestId = 1,
                    Unit = "ambar",
                    RequestQuantity = 5,
                },
            };
            var request = new RawMaterialRequestModel()
            {
                Id = 1,
                CreationDate = "01/04/2020",
                CreationUserId = "123",
                Observations = "ninguno",
                SigningUserId = "123",
                SigningUserName = "username",
                ProductionOrderIds = new List<int>() { 1, 2, 3 },
                OrderedProducts = rawMaterial,
                Signature = this.GetSignatureExample(),
            };

            var mockCatalog = new Mock<ICatalogsService>();
            var mockEmail = new Mock<IOmicronMailClient>();

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = service.CreateRawMaterialRequestPdf(request, true);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the delivery cancel test.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequestPdfWithoutSignature()
        {
            // arrange
            var rawMaterial = new List<RawMaterialRequestDetailModel>()
            {
                new RawMaterialRequestDetailModel()
                {
                    Id = 1,
                    Description = "description",
                    ProductId = "reve 14",
                    RequestId = 1,
                    Unit = "ambar",
                    RequestQuantity = 5,
                },
            };
            var request = new RawMaterialRequestModel()
            {
                Id = 1,
                CreationDate = "01/04/2020",
                CreationUserId = "123",
                Observations = "ninguno",
                SigningUserId = "123",
                SigningUserName = "username",
                ProductionOrderIds = new List<int>() { 1, 2, 3 },
                OrderedProducts = rawMaterial,
            };

            var mockCatalog = new Mock<ICatalogsService>();
            var mockEmail = new Mock<IOmicronMailClient>();

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = service.CreateRawMaterialRequestPdf(request, false);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the delivery cancel test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task SubmitRawMaterialRequestPdf()
        {
            // arrange
            var rawMaterial = new List<RawMaterialRequestDetailModel>()
            {
                new RawMaterialRequestDetailModel()
                {
                    Id = 1,
                    Description = "description",
                    ProductId = "reve 14",
                    RequestId = 1,
                    Unit = "ambar",
                    RequestQuantity = 5,
                },
            };
            var request = new RawMaterialRequestModel()
            {
                Id = 1,
                CreationDate = "01/04/2020",
                CreationUserId = "123",
                Observations = "ninguno",
                SigningUserId = "123",
                SigningUserName = "username",
                ProductionOrderIds = new List<int>() { 1, 2, 3 },
                OrderedProducts = rawMaterial,
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetSmtpConfig())
                .Returns(Task.FromResult(this.GetSMTPConfig()));
            mockCatalog
                .Setup(m => m.GetRawMaterialEmailConfig())
                .Returns(Task.FromResult(this.GetRawMaterialEmailConfigModel()));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = await service.SubmitRawMaterialRequestPdf(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the delivery cancel test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task SubmitIncidentsExel()
        {
            // arrange
            var request = new List<IncidentDataModel>()
            {
                new IncidentDataModel
                {
                    CreateDate = new DateTime(2021, 03, 24),
                    SaleOrder = 8990,
                    Delivery = 7450,
                    Invoice = 12234,
                    ItemCode = "Reve 14",
                    Type = "linea",
                    Incident = "roto",
                    Batches = "baches",
                    Stage = "reception",
                    Comments = "comments",
                    Status = "abierto",
                    CreateDateString = "26/04/2021 10:50:20",
                },
                new IncidentDataModel
                {
                    CreateDate = new DateTime(2021, 03, 24),
                    SaleOrder = 8991,
                    Delivery = 7452,
                    Invoice = 12232,
                    ItemCode = "Reve 15",
                    Type = "linea",
                    Incident = "perdido",
                    Batches = "baches",
                    Stage = "reception",
                    Comments = "comments",
                    Status = "atendiendo",
                    CreateDateString = "26/04/2021 10:50:20",
                },
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Field = "SmtpServer", Value = "192.168.0.1" },
                new ParametersModel { Field = "SmtpPort", Value = "5434" },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = "correo@axity.com" },
                new ParametersModel { Field = "EmailMiddleware", Value = "correo@axity.com" },
                new ParametersModel { Field = "EmailCCDelivery", Value = "correo@axity.com" },
                new ParametersModel { Field = "EmailIncidentReport", Value = "correo@axity.com" },
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
            var result = await service.SubmitIncidentsExel(request);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Method to get and send emails of products abandoned.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SendEmails()
        {
            var listDoctor = new List<EmailGenericDto>()
            {
                new EmailGenericDto { BodyEmail = "hello", CopyEmails = "copy@email.com", DestinityEmail = "destinity@email.com", Subject = "Email prueba 1" },
                new EmailGenericDto { BodyEmail = "hello", CopyEmails = "copy2@email.com", DestinityEmail = "destinity2@email.com", Subject = "Email prueba 2" },
                new EmailGenericDto { BodyEmail = "hello", CopyEmails = "copy3@email.com", DestinityEmail = "destinity3@email.com", Subject = "Email prueba 3" },
                new EmailGenericDto { BodyEmail = "hello", CopyEmails = "copy4@email.com", DestinityEmail = "destinity4@email.com", Subject = "Email prueba 4" },
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Field = "SmtpServer", Value = "192.168.0.1" },
                new ParametersModel { Field = "SmtpPort", Value = "5434" },
                new ParametersModel { Field = "EmailMiddlewarePassword", Value = "correo@axity.com" },
                new ParametersModel { Field = "EmailMiddleware", Value = "correo@axity.com" },
                new ParametersModel { Field = "EmailCCDelivery", Value = "correo@axity.com" },
                new ParametersModel { Field = "EmailIncidentReport", Value = "correo@axity.com" },
            };

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object);

            // act
            var result = await service.SendEmails(listDoctor);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }
    }
}
