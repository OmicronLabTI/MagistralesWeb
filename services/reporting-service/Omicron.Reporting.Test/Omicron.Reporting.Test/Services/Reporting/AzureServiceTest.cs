// <summary>
// <copyright file="AzureServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Test.Services.Reporting
{
    /// <summary>
    /// class for payment.
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
            // act
            var service = new AzureServices();
            var result = await service.GetlementFromAzure(string.Empty, string.Empty, "https://omicronblobpruebas.blob.core.windows.net/resources/ClasificacionEnvases.xlsx");

            ClassicAssert.IsNull(result);
        }
    }
}
