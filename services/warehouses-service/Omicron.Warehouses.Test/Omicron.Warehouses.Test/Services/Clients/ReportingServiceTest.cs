// <summary>
// <copyright file="ReportingServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Test.Services.SapAdapter
{
    using System.Net.Http;
    using System.Threading;
    using Moq;
    using Moq.Protected;
    using NUnit.Framework;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Clients;

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
            Assert.IsTrue(result.Success);
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
            Assert.IsTrue(result.Success);
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
            Assert.IsTrue(result);
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
            Assert.IsFalse(result);
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
            Assert.IsFalse(result);
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
