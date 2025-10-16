// <summary>
// <copyright file="InvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

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

            var responseInvoice = GetGenericResponseModel(responseCode, isResponseInvoiceSuccess, invoiceResult, userError);
            var shippingTypesObject = new List<ShippingTypesResponseDto>
            {
                new () { TransportCode = 1, TransportName = "DHL" },
            };

            var resultShippingTypes = GetGenericResponseModel(
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ResultModel>());
            if (isResponseInvoiceSuccess)
            {
                Assert.That(result.Code == 200, Is.True);
                Assert.That(result.Success, Is.True);
                Assert.That(result.UserError, Is.Null);
            }
            else
            {
                Assert.That(result.Code == 200, Is.True);
                Assert.That(result.Success, Is.True);
                Assert.That(result.UserError, Is.Not.Null);
                Assert.That(result.UserError, Is.EqualTo("No existen registros coincidentes (ODBC -2028)"));
            }

            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Comments, Is.Null);
        }

        /// <summary>
        /// InvoiceTrackingInfoTest.
        /// </summary>
        /// <param name="sapTrackingNumber">Sap Tracking Number.</param>
        /// <param name="sapExtendedTrackingNumbers">sapExtendedTrackingNumbers.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(null, null)]
        [TestCase("1001-9001", null)]
        [TestCase("1001-9001,1002-9002", null)]
        [TestCase("1001-9001,1002-9002,1003-9003", null)]
        [TestCase("1001-9001,1002-9002,1003-9003", "1004-9004")]
        public async Task InvoiceTrackingInfoTest(string sapTrackingNumber, string sapExtendedTrackingNumbers)
        {
            int invoiceId = 149812;
            var trackingInfo = new TrackingInformationDto
            {
                TrackingNumber = "9005",
                TransportMode = "DHL",
                PackageId = 1005,
            };

            // arrange
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var invoiceResult = new InvoiceDto
            {
                DocumentEntry = 3,
                TrackingNumber = sapTrackingNumber,
                ExtendedTrackingNumbers = sapExtendedTrackingNumbers,
            };

            var responseInvoice = GetGenericResponseModel(200, true, invoiceResult, null);
            var shippingTypesObject = new List<ShippingTypesResponseDto>
            {
                new () { TransportCode = 1, TransportName = "DHL" },
            };

            var resultShippingTypes = GetGenericResponseModel(
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ResultModel>());
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Comments, Is.Null);
        }

        /// <summary>
        /// InvoiceTrackingInfoTest.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateInvoiceByRemissions()
        {
            // arrange
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var deliveryNote = new DeliveryNoteCreatedDto() { CustomerCode = "C0001", DocumentLines = new List<DeliveryNoteLineCreatedDto>() { new DeliveryNoteLineCreatedDto() { LineNum = 1 } } };
            var response = new ServiceLayerGenericMultipleResultDto<DeliveryNoteCreatedDto>() { Value = new List<DeliveryNoteCreatedDto>() { deliveryNote } };
            var responseInvoice = GetGenericResponseModel(200, true, response, null);
            mockServiceLayerClient
                .Setup(sl => sl.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(responseInvoice));
            mockServiceLayerClient
                .Setup(sl => sl.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetGenericResponseModel(200, true, null, null)));

            var invoiceServiceMock = new InvoiceService(mockServiceLayerClient.Object, mockLogger.Object);
            var result = await invoiceServiceMock.CreateInvoiceByRemissions(new List<int>());

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ResultModel>());
            Assert.That(result.Comments, Is.Null);
        }
    }
}
