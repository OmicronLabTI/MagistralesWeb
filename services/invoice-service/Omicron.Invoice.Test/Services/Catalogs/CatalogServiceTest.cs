// <summary>
// <copyright file="CatalogServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.Catalog
{
    /// <summary>
    /// class for test.
    /// </summary>
    public class CatalogServiceTest : BaseHttpClientTest<CatalogsService>
    {
        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetParams()
        {
            // Arrange
            var client = this.CreateClient();

            // Act
            var result = client.GetParams("endpoint").Result;

            // Assert
            Assert.That(result.Success);
        }

        /// <summary>
        /// Action tests.
        /// </summary>
        [Test]
        public void GetParamsError()
        {
            // Arrange
            var client = this.CreateClientFailure();

            // Act
            Assert.ThrowsAsync<CustomServiceException>(async () => await client.GetParams("endpoint"));
        }
    }
}
