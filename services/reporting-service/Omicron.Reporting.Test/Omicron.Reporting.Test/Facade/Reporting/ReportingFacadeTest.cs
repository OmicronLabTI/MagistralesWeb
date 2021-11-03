// <summary>
// <copyright file="ReportingFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Facade.Request
{
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.Reporting.Dtos.Model;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Facade.Request;
    using Omicron.Reporting.Services;
    using Omicron.Reporting.Services.Mapping;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class ReportingFacadeTest : BaseTest
    {
        private ReportingFacade reportingFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var fileResultModel = AutoFixtureProvider.Create<FileResultModel>();
            fileResultModel.Success = true;

            var resultModel = AutoFixtureProvider.Create<ResultModel>();
            resultModel.Success = true;
            resultModel.Code = 200;

            var mockReportingService = new Mock<IReportingService>();
            mockReportingService.SetReturnsDefault(fileResultModel);
            mockReportingService.SetReturnsDefault(Task.FromResult(resultModel));
            mockReportingService
                .Setup(m => m.SendEmailRejectedOrder(It.IsAny<SendRejectedEmailModel>()))
                .Returns(Task.FromResult(resultModel));

            mockReportingService
                .Setup(m => m.SendEmailForeignPackage(It.IsAny<SendPackageModel>()))
                .Returns(Task.FromResult(resultModel));

            mockReportingService
                .Setup(m => m.SendEmailLocalPackage(It.IsAny<SendLocalPackageModel>()))
                .Returns(Task.FromResult(resultModel));

            mockReportingService
                .Setup(m => m.SendEmailCancelDeliveryOrders(It.IsAny<List<SendCancelDeliveryModel>>()))
                .Returns(Task.FromResult(resultModel));

            mockReportingService
                .Setup(m => m.SubmitIncidentsExel(It.IsAny<List<IncidentDataModel>>()))
                .Returns(Task.FromResult(resultModel));

            mockReportingService
                .Setup(m => m.SendEmails(It.IsAny<List<EmailGenericDto>>()))
                .Returns(Task.FromResult(resultModel));

            this.reportingFacade = new ReportingFacade(mockReportingService.Object, mapper);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequestPdf()
        {
            // arrange
            var requests = AutoFixtureProvider.Create<RawMaterialRequestDto>();
            requests.Signature = File.ReadAllText("SignatureBase64.txt");

            // act
            var response = this.reportingFacade.CreateRawMaterialRequestPdf(requests, false);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotEmpty(response.FileName);
            Assert.IsNotNull(response.FileName);
            Assert.IsNotNull(response.FileStream);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SubmitRawMaterialRequest()
        {
            // arrange
            var requests = AutoFixtureProvider.Create<RawMaterialRequestDto>();
            requests.Signature = File.ReadAllText("SignatureBase64.txt");

            // act
            var response = await this.reportingFacade.SubmitRawMaterialRequestPdf(requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SendEmailForeignPackage()
        {
            // arrange
            var requests = new SendPackageDto();

            // act
            var response = await this.reportingFacade.SendEmailForeignPackage(requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SendEmailLocalPackage()
        {
            // arrange
            var requests = new SendLocalPackageDto();

            // act
            var response = await this.reportingFacade.SendEmailLocalPackage(requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SendEmailRejectedOrder()
        {
            // arrange
            var requests = new SendRejectedEmailDto();

            // act
            var response = await this.reportingFacade.SendEmailRejectedOrder(requests);

            // arrange
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SendEmailCancelDeliveryOrders()
        {
            // arrange
            var requests = new List<SendCancelDeliveryDto>();

            // act
            var response = await this.reportingFacade.SendEmailCancelDeliveryOrders(requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SubmitIncidentsExel()
        {
            // arrange
            var requests = new List<IncidentDataDto>();

            // act
            var response = await this.reportingFacade.SubmitIncidentsExel(requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test facade map result.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task SendEmails()
        {
            // arrange
            var requests = new List<EmailGenericDto>()
            {
                new EmailGenericDto { BodyEmail = "body", CopyEmails = "email@email.com", DestinityEmail = "email2@email.com", Subject = "subject" },
            };

            // act
            var response = await this.reportingFacade.SendEmails(requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Code == 200);
            Assert.IsNotNull(response.ExceptionMessage);
            Assert.IsNotNull(response.UserError);
        }
    }
}
