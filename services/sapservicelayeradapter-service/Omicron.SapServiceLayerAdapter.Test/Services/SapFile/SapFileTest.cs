// <summary>
// <copyright file="SapFileTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services.SapFile
{
    /// <summary>
    /// The test.
    /// </summary>
    [TestFixture]
    public class SapFileTest : BaseHttpClientTest<SapFileService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostToSapFile()
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
        public void PostToSapFileFailure()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.PostAsync(new { }, "endpoint"));
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostToSapFileWithErrorResponse()
        {
            // Arrange
            var client = this.CreateClientWithErrorResponse();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.PostAsync(new { }, "endpoint"));
        }
    }
}
