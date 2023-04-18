// <summary>
// <copyright file="AlmacenServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services.Almacen
{
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.Services.AlmacenService;

    /// <summary>
    /// Test class for Sap Adapter.
    /// </summary>
    [TestFixture]
    public class AlmacenServiceTest : BaseHttpClientTest<AlmacenService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetSapAdapter()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetAlmacenData("endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetAlmacenerror()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.GetAlmacenData("endpoint"));
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostSapAdapter()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostAlmacenData("endpoint", new { }).Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostAlmacenError()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.PostAlmacenData("endpoint", new { }));
        }
    }
}
