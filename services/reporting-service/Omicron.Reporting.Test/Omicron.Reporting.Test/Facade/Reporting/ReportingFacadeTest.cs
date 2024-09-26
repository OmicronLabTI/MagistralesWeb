// <summary>
// <copyright file="ReportingFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Facade.Request
{
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

            mockReportingService
                .Setup(m => m.CreateRawMaterialRequestPdf(It.IsAny<RawMaterialRequestModel>(), It.IsAny<bool>()))
                .Returns(new List<string> { "http://urlpdf.com", "http://urlpdf2.com" });

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

            Assert.That(response, Is.Not.Null);
            response.ForEach(report =>
            {
                Assert.That(report, Is.Not.Null);
            });
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
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
            Assert.That(response, Is.Not.Null);
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
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
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Code == 200, Is.True);
            Assert.That(response.ExceptionMessage, Is.Not.Null);
            Assert.That(response.UserError, Is.Not.Null);
        }
    }
}
