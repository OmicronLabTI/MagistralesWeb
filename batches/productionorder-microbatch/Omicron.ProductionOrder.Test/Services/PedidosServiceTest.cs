// <summary>
// <copyright file="PedidosServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Test.Services
{
    /// <summary>
    /// The test.
    /// </summary>
    [TestFixture]
    public class PedidosServiceTest : BaseHttpClientTest<PedidosService>
    {
        /// <summary>
        /// GetAsync.
        /// </summary>
        [Test]
        public void GetAsync()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetAsync("GetEndpoint", "Log Base").Result;

            // Assert
            Assert.That(result.Success);
        }

        /// <summary>
        /// GetAsyncError.
        /// </summary>
        [Test]
        public void GetAsyncError()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await client.GetAsync("GetEndpoint", "Log Base"));
        }

        /// <summary>
        /// PostAsync.
        /// </summary>
        [Test]
        public void PostAsync()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostAsync("PostEndpoint", new object(), "Log Base").Result;

            // Assert
            Assert.That(result.Success);
        }

        /// <summary>
        /// PostAsyncError.
        /// </summary>
        [Test]
        public void PostAsyncError()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await client.PostAsync("PostEndpoint", new object(), "Log Base"));
        }
    }
}
