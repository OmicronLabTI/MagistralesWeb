// <summary>
// <copyright file="SapAdapterServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.SapServiceAdapter
{
    /// <summary>
    /// The test.
    /// </summary>
    [TestFixture]
    public class SapAdapterServiceTest : BaseHttpClientTest<SapAdapter>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostSapAdapter()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostSapAdapter(new { }, "endpoint").Result;

            // Assert
            Assert.That(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostSapAdapterError()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.PostSapAdapter(new { }, "endpoint"));
        }
    }
}
