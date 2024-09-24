// <summary>
// <copyright file="SapServiceLayerAdapterServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services.SapServiceLayer
{
    using Omicron.Pedidos.Services.SapServiceLayerAdapter;

    /// <summary>
    /// Test class for Sap Adapter.
    /// </summary>
    [TestFixture]
    public class SapServiceLayerAdapterServiceTest : BaseHttpClientTest<SapServiceLayerAdapterService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostToSapDiApi()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostAsync(new { }, "endpoint").Result;

            // Assert
            ClassicAssert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PatchAsyncSapServiceLayerAdapter()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PatchAsync("endpoint", "{\"key\": \"value\"}").Result;

            // Assert
            ClassicAssert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PatchAsyncSapServiceLayerAdapterFailure()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            ClassicAssert.ThrowsAsync<CustomServiceException>(async () => await client.PatchAsync("endpoint", "{\"key\": \"value\"}"));
        }
    }
}