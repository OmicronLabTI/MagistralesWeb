// <summary>
// <copyright file="SapDiApiTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test.Services.SapDiApi
{
    using NUnit.Framework;
    using Omicron.Pedidos.Services.SapDiApi;

    /// <summary>
    /// Test class for Sap Adapter.
    /// </summary>
    [TestFixture]
    public class SapDiApiTest : BaseHttpClientTest<SapDiApi>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetSapDiApi()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetSapDiApi("endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void PostToSapDiApi()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.PostToSapDiApi(new { }, "endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}
