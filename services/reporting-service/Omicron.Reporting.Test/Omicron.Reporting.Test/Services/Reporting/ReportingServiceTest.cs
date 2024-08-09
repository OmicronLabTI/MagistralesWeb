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
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services;
    using Omicron.Reporting.Services.AzureServices;
    using Omicron.Reporting.Services.Clients;
    using Omicron.Reporting.Services.ReportBuilder.SuppliesWarehouse;
    using Omicron.Reporting.Services.ServiceLayerAdapter;

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
                DestinyEmail = "email@email.com;test@email.com",
                PackageId = 1,
                TrackingNumber = "asdf",
                TransportMode = party,
                ClientName = "Gustavo ramirez",
                SalesOrders = "AASSs",
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
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

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
                new ParametersModel { Field = "DeliveryNotDeliveryCopy", Value = "string" },
                new ParametersModel { Field = "EmailAtencionAClientes", Value = "string" },
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .SetReturnsDefault(Task.FromResult(true));

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(x => x[It.Is<string>(s => s == "InvoicePdfAzureroute")]).Returns("test");
            mockConfig.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountName")]).Returns("test3");
            mockConfig.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountKey")]).Returns("test4");
            var mockAzure = new Mock<IAzureService>();

            if (status == "Entregado")
            {
                mockAzure
                    .Setup(x => x.GetlementFromAzure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(It.IsAny<BlobDownloadInfo>()));
            }

            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

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
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

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
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
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

            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

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
                    Warehouse = "MG",
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
            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

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
                    Warehouse = "MG",
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
            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

            // act
            var result = service.CreateRawMaterialRequestPdf(request, false);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// gets the delivery cancel test.
        /// </summary>
        /// <param name="hasDiApiError">Has di api errror.</param>
        /// <returns>the orders.</returns>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task SubmitRawMaterialRequestPdf(bool hasDiApiError)
        {
            // arrange
            var userId = "123";
            var userName = "username";

            var rawMaterial = new List<RawMaterialRequestDetailModel>()
            {
                new RawMaterialRequestDetailModel()
                {
                    Id = 1,
                    Description = "SULFATO DE COBRE 5 GR, SOBRES",
                    ProductId = "1001   12 SB",
                    RequestId = 1,
                    Unit = "Paquete",
                    RequestQuantity = 5,
                    Warehouse = "MG",
                    IsLabel = true,
                },
                new RawMaterialRequestDetailModel()
                {
                    Id = 1,
                    Description = "SULFATO DE COBRE 5 GR, SOBRES",
                    ProductId = "1001   200 SB",
                    RequestId = 1,
                    Unit = null,
                    RequestQuantity = 5,
                    Warehouse = "MG",
                    IsLabel = false,
                },
            };
            var request = new RawMaterialRequestModel()
            {
                Id = 1,
                CreationDate = "01/04/2020",
                CreationUserId = "123",
                Observations = "ninguno",
                SigningUserId = userId,
                SigningUserName = userName,
                ProductionOrderIds = new List<int>() { 1, 2, 3 },
                OrderedProducts = rawMaterial,
            };

            var resultSapServiceLayerAdapter = new List<TransferRequestResult>
            {
                new TransferRequestResult
                {
                    UserInfo = $"{userName}-{userId}",
                    Error = hasDiApiError ? "Error enviado pro Di Api" : string.Empty,
                    TransferRequestId = hasDiApiError ? 0 : 756778,
                },
            };

            var listParams = new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Field = "SmtpServer", Value = "SmtpServer" },
                new ParametersModel { Id = 2, Field = "SmtpPort", Value = "1234" },
                new ParametersModel { Id = 3, Field = "EmailMiddlewarePassword", Value = "EmailMiddlewarePassword" },
                new ParametersModel { Id = 4, Field = "EmailMiddleware", Value = "EmailMiddleware" },
                new ParametersModel { Id = 5, Field = "EmailCCDelivery", Value = "EmailCCDelivery" },
                new ParametersModel { Id = 6, Field = "EmailMiddlewareUser", Value = "EmailMiddlewareUser" },
                new ParametersModel { Id = 7, Field = "EmailAlmacen2", Value = "almacen_etiqueta@mail.com" },
                new ParametersModel { Id = 8, Field = "EmailAlmacen", Value = "almacen_noetiqueta@mail.com" },
                new ParametersModel { Id = 9, Field = "EmailLogisticaCc1", Value = "logisitca1@mail.com" },
                new ParametersModel { Id = 10, Field = "EmailLogisticaCc2", Value = "logisitca2@mail.com" },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            mockSapServiceLayerAdapter
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultSapServiceLayerAdapter(resultSapServiceLayerAdapter)));

            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();

            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

            // act
            var result = await service.SubmitRawMaterialRequestPdf(request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);

            if (hasDiApiError)
            {
                Assert.IsFalse((bool)result.Response);
                Assert.IsEmpty((List<string>)result.Comments);
            }
            else
            {
                Assert.IsTrue((bool)result.Response);
                Assert.IsTrue(((List<string>)result.Comments).Count > 0);
            }
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
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
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

            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

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
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
            };

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .Setup(m => m.SendMail(It.IsAny<SmtpConfigModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, MemoryStream>>()))
                .Returns(Task.FromResult(true));

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockConfig = new Mock<IConfiguration>();
            var mockAzure = new Mock<IAzureService>();
            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

            // act
            var result = await service.SendEmails(listDoctor);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
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
        public async Task SendEmailLocalPackageMailNull(string status, string reasonNotDelivered)
        {
            // arrange
            var request = new SendLocalPackageModel
            {
                DestinyEmail = null,
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
                new ParametersModel { Field = "DeliveryNotDeliveryCopy", Value = "string" },
                new ParametersModel { Field = "EmailAtencionAClientes", Value = "string" },
                new ParametersModel { Field = "EmailMiddlewareUser", Value = "string" },
            };

            var mockCatalog = new Mock<ICatalogsService>();
            mockCatalog
                .Setup(m => m.GetParams(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(listParams));

            var mockEmail = new Mock<IOmicronMailClient>();
            mockEmail
                .SetReturnsDefault(Task.FromResult(true));

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(x => x[It.Is<string>(s => s == "InvoicePdfAzureroute")]).Returns("test");
            mockConfig.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountName")]).Returns("test3");
            mockConfig.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountKey")]).Returns("test4");
            var mockAzure = new Mock<IAzureService>();

            if (status == "Entregado")
            {
                mockAzure
                    .Setup(x => x.GetlementFromAzure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(It.IsAny<BlobDownloadInfo>()));
            }

            var mockSapServiceLayerAdapter = new Mock<ISapServiceLayerAdapterService>();
            var service = new ReportingService(mockCatalog.Object, mockEmail.Object, mockConfig.Object, mockAzure.Object, mockSapServiceLayerAdapter.Object);

            // act
            var result = await service.SendEmailLocalPackage(request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Response.Equals(false));
            Assert.IsTrue(result.Code.Equals(200));
        }
    }
}
