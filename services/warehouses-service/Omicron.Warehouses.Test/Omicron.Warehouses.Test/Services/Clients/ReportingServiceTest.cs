// <summary>
// <copyright file="ReportingServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test.Services.SapAdapter
{
    /// <summary>
    /// Test class for catalogs service.
    /// </summary>
    [TestFixture]
    public class ReportingServiceTest : BaseHttpClientTest<ReportingService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetAsync()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetAsync("endpoint").Result;

            // Assert
            Assert.That(result.Success, Is.True);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostAsync()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostAsync(new { }, "endpoint").Result;

            // Assert
            Assert.That(result.Success, Is.True);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void SubmitRequest()
        {
            // Arrange
            var client = this.CreateClient(this.GetBooleanResult());

            // Act
            var result = client.SubmitRequest(new RawMaterialRequestModel()).Result;

            // Assert
            Assert.That(result.Item1, Is.True);
            this.GetHttpMock().Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void SubmitRequest_WithError()
        {
            // Arrange
            var client = this.CreateClient(this.GetBooleanResult(false));

            // Act
            var result = client.SubmitRequest(new RawMaterialRequestModel()).Result;

            // Assert
            Assert.That(result.Item1, Is.False);
            this.GetHttpMock().Protected().Verify("SendAsync", Times.Exactly(3), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void SubmitRequest_Throw()
        {
            // Arrange
            var client = this.CreateClient(this.GetStringResult());

            // Act
            var result = client.SubmitRequest(new RawMaterialRequestModel()).Result;

            // Assert
            Assert.That(result.Item1, Is.False);
            this.GetHttpMock().Protected().Verify("SendAsync", Times.Exactly(3), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        private ResultModel GetBooleanResult(bool resultResponse = true)
        {
            var result = AutoFixtureProvider.Create<ResultModel>();
            result.Success = true;
            result.Response = resultResponse;
            return result;
        }

        private ResultModel GetStringResult()
        {
            var result = AutoFixtureProvider.Create<ResultModel>();
            result.Success = true;
            result.Response = "Error";
            return result;
        }
    }
}
