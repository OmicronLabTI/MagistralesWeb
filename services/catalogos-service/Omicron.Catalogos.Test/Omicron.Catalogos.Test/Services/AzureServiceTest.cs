// <summary>
// <copyright file="AzureServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Services
{
    /// <summary>
    /// Test azure service.
    /// </summary>
    public class AzureServiceTest
    {
        /// <summary>
        /// Method to verify Get All ProcessPayments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetExistElementsFromAzure()
        {
            // arrange
            var mockLog = new Mock<ILogger>();
            using var stream = new MemoryStream();
            var service = new AzureService(mockLog.Object);

            // act
            await service.GetElementsFromAzure(string.Empty, string.Empty, "https://omicronblobpruebas.blob.core.windows.net/resources/Archivo.xlsx", stream);

            // assert
            Assert.That(stream, Is.Not.Null);
            Assert.That(stream.Position, Is.EqualTo(0));
        }
    }
}
