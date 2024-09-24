// <summary>
// <copyright file="SapServiceLayerAdapterServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Services.ServiceLayerAdapter
{
    /// <summary>
    /// The test.
    /// </summary>
    [TestFixture]
    public class SapServiceLayerAdapterServiceTest : BaseHttpClientTest<SapServiceLayerAdapterService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostAsyncSapServiceLayerAdapter()
        {
            // Arrange
            var client = this.CreateClientResultModel();

            // Act
            var result = client.PostAsync("endpoint", "{\"key\": \"value\"}").Result;

            // Assert
            ClassicAssert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostAsyncSapServiceLayerAdapterFailure()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            ClassicAssert.ThrowsAsync<CustomServiceException>(async () => await client.PostAsync("endpoint", "{\"key\": \"value\"}"));
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostAsyncSapServiceLayerAdapterControlledError()
        {
            // Arrange
            var client = this.CreateClientWithErrorResponse();

            // Act
            ClassicAssert.ThrowsAsync<CustomServiceException>(async () => await client.PostAsync("endpoint", "{\"key\": \"value\"}"));
        }
    }
}
