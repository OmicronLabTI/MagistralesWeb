// <summary>
// <copyright file="SapAdapterTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Test.Services.SapAdapter
{
    using NUnit.Framework;
    using Omicron.Pedidos.Services.SapAdapter;

    /// <summary>
    /// Test class for Sap Adapter.
    /// </summary>
    [TestFixture]
    public class SapAdapterTest : BaseHttpClientTest<SapAdapter>
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
            var result = client.GetSapAdapter("endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
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
            var result = client.PostSapAdapter(new { }, "endpoint").Result;

            // Assert
            Assert.IsTrue(result.Success);
        }
    }
}
