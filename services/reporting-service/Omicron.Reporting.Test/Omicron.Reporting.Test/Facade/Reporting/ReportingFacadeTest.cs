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

            var resultDto = AutoFixtureProvider.Create<FileResultModel>();
            resultDto.Success = true;

            var mockReportingService = new Mock<IReportingService>();
            mockReportingService.SetReturnsDefault(resultDto);

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
    }
}
