// <summary>
// <copyright file="ReportingServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Services.Request
{
    using NUnit.Framework;
    using Omicron.Reporting.Services;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ReportingServiceTest : BaseTest
    {
        private IReportingService reportingService;

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.reportingService = new ReportingService();
        }

        /// <summary>
        /// Create raw material request as PDF file.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequestPdf_CreatePreview_ReturnFileStream()
        {
            // arrange
            var request = this.GetMockRawMaterialRequest();

            // act
            var response = this.reportingService.CreateRawMaterialRequestPdf(request, true);

            // assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.FileName);
            Assert.IsNotEmpty(response.FileName);
            Assert.IsTrue(response.FileName.Contains($"_PREVIEW.pdf"));
            Assert.IsNotNull(response.FileStream);
            Assert.Greater(response.FileStream.Length, 0);
        }

        /// <summary>
        /// Create raw material request as PDF file.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequestPdf_CreateNonPreview_ReturnFileStream()
        {
            // arrange
            var request = this.GetMockRawMaterialRequest();

            // act
            var response = this.reportingService.CreateRawMaterialRequestPdf(request, false);

            // assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.FileName);
            Assert.IsNotEmpty(response.FileName);
            Assert.IsTrue(response.FileName.Contains($"_{request.Id}.pdf"));
            Assert.IsNotNull(response.FileStream);
            Assert.Greater(response.FileStream.Length, 0);
        }

        /// <summary>
        /// Create raw material request as PDF file.
        /// </summary>
        [Test]
        public void CreateRawMaterialRequestPdf_CreatePreviewWithEmptySignature_ReturnFileStream()
        {
            // arrange
            var request = this.GetMockRawMaterialRequest();
            request.Signature = string.Empty;

            // act
            var response = this.reportingService.CreateRawMaterialRequestPdf(request, true);

            // assert
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.FileName);
            Assert.IsNotEmpty(response.FileName);
            Assert.IsTrue(response.FileName.Contains($"_PREVIEW.pdf"));
            Assert.IsNotNull(response.FileStream);
            Assert.Greater(response.FileStream.Length, 0);
        }
    }
}
