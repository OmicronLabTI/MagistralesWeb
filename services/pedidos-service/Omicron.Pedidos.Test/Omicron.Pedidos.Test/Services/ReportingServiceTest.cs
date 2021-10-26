// <summary>
// <copyright file="ReportingServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.Services.Reporting;

    /// <summary>
    /// Test class for Sap Adapter.
    /// </summary>
    [TestFixture]
    public class ReportingServiceTest : BaseHttpClientTest<ReportingService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetReportingService()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetReportingService("endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetReportingServiceFailure()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.GetReportingService("endpoint"));
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostReportingService()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostReportingService(new { }, "endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostReportingServiceFailure()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.PostReportingService(new { }, "endpoint"));
        }
    }
}
