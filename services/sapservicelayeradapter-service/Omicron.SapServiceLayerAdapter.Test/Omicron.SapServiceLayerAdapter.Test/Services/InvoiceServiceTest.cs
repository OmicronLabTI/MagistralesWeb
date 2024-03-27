// <summary>
// <copyright file="OrderServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Microsoft.AspNetCore.Mvc;
using Omicron.SapServiceLayerAdapter.Services.Invoices.Impl;
using Serilog;

namespace Omicron.SapServiceLayerAdapter.Test.Services
{
    /// <summary>
    /// Class OrdersServiceTest.
    /// </summary>
    [TestFixture]
    public class InvoiceServiceTest : BaseTest
    {
        /// <summary>
        /// Test the send package.
        /// </summary>
        /// <param name="isResponseInvoiceSuccess">Is Response Invoice Success.</param>
        /// <param name="responseCode">Response code.</param>
        /// <param name="userError">User error.</param>
        /// <param name="transportMode">Transport Mode.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(false, 400, "No existen registros coincidentes (ODBC -2028)", "")]
        [TestCase(true, 200, null, "DHL")]
        [TestCase(true, 200, null, "NO EXISTE TRANSPORT MODE")]
        public async Task UpdateInvoiceTrackingInfoTest(bool isResponseInvoiceSuccess, int responseCode, string userError, string transportMode)
        {
            int invoiceId = 123456;
            var trackingInfo = new TrackingInformationDto
            {
                TrackingNumber = "123456789",
                TransportMode = transportMode,
            };

            // arrange
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var invoiceResult = new InvoiceDto();
            if (isResponseInvoiceSuccess)
            {
                invoiceResult = new InvoiceDto
                {
                    DocumentEntry = 3,
                };
            }

            var responseInvoice = this.GetGenericResponseModel(responseCode, isResponseInvoiceSuccess, invoiceResult, userError);
            var shippingTypesObject = new List<ShippingTypesResponseDto>
            {
                new () { TransportCode = 1, TransportName = "DHL" },
            };

            var resultShippingTypes = this.GetGenericResponseModel(
                200,
                true,
                new ServiceLayerResponseDto
                {
                    Metadata = "metadata",
                    Value = JsonConvert.SerializeObject(shippingTypesObject),
                });

            mockServiceLayerClient
                .SetupSequence(sl => sl.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(responseInvoice))
                .Returns(Task.FromResult(resultShippingTypes));

            var invoiceServiceMock = new InvoiceService(mockServiceLayerClient.Object, mockLogger.Object);
            var result = await invoiceServiceMock.UpdateInvoiceTrackingInfo(invoiceId, trackingInfo);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResultModel>(result);
            if (isResponseInvoiceSuccess)
            {
                Assert.IsTrue(result.Code == 200);
                Assert.IsTrue(result.Success);
                Assert.IsNull(result.UserError);
            }
            else
            {
                Assert.IsTrue(result.Code == 200);
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.UserError);
                Assert.AreEqual("No existen registros coincidentes (ODBC -2028)", result.UserError);
            }

            Assert.IsNotNull(result.Response);
            Assert.IsNull(result.Comments);
        }
    }
}
