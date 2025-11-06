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
        /// Test for CreateInvoice method.
        /// </summary>
        /// <param name="isResponseSuccess">Indicates if the invoice creation in Service Layer is successful.</param>
        /// <param name="throwsException">Indicates if an exception should be thrown.</param>
        /// <param name="expectedCode">Expected response code.</param>
        /// <param name="expectedSuccess">Expected success flag.</param>
        /// <param name="expectedUserError">Expected user error message.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(true, false, 200, true, null)]
        [TestCase(false, false, 500, false, "Service Layer Error: No se pudo crear la factura")]
        [TestCase(false, true, 500, false, "Unexpected error")]
        public async Task CreateInvoiceTest(bool isResponseSuccess, bool throwsException, int expectedCode, bool expectedSuccess, string expectedUserError)
        {
            // Arrange
            var createInvoiceDocumentInfo = new CreateInvoiceDocumentDto
            {
                CardCode = "C12345",
                ProcessId = "2c904df4-96db-4a56-bf56-4747c3174106",
                CfdiDriverVersion = "CFDi40",
                IdDeliveries = new List<int> { 1001, 1002, 1003 },
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();

            var deliveryMock = new List<DeliveryNoteDto>
            {
                new DeliveryNoteDto
                {
                    DocEntry = 1001,
                    DeliveryNoteLines = new List<DeliveryNoteLineDto>
                    {
                        new DeliveryNoteLineDto { LineNumber = 0 },
                        new DeliveryNoteLineDto { LineNumber = 1 },
                    },
                },
                new DeliveryNoteDto
                {
                    DocEntry = 1002,
                    DeliveryNoteLines = new List<DeliveryNoteLineDto>
                    {
                        new DeliveryNoteLineDto { LineNumber = 0 },
                        new DeliveryNoteLineDto { LineNumber = 1 },
                        new DeliveryNoteLineDto { LineNumber = 2 },
                        new DeliveryNoteLineDto { LineNumber = 3 },
                    },
                },
                new DeliveryNoteDto
                {
                    DocEntry = 1003,
                    DeliveryNoteLines = new List<DeliveryNoteLineDto>
                    {
                        new DeliveryNoteLineDto { LineNumber = 0 },
                    },
                },
            };

            var responseDeliveryMock = new ServiceLayerGenericMultipleResultDto<DeliveryNoteDto>()
            {
                Value = deliveryMock,
            };

            var deliveryResponseMock = GetGenericResponseModel(200, true, responseDeliveryMock, null);

            mockServiceLayerClient
                .Setup(sl => sl.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(deliveryResponseMock);

            if (throwsException)
            {
                mockServiceLayerClient
                    .Setup(sl => sl.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception("Unexpected error"));
            }
            else
            {
                var fakeResponse = isResponseSuccess
                    ? new InvoiceDto { DocumentEntry = 999, DocumentNumber = 999 }
                    : null;

                var invoiceResponseMock = GetGenericResponseModel(
                    200,
                    isResponseSuccess,
                    fakeResponse,
                    isResponseSuccess ? null : "Service Layer Error: No se pudo crear la factura");

                mockServiceLayerClient
                    .Setup(sl => sl.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(invoiceResponseMock);
            }

            var invoiceService = new InvoiceService(mockServiceLayerClient.Object, mockLogger.Object);

            // Act
            var result = await invoiceService.CreateInvoice(createInvoiceDocumentInfo);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ResultModel>());
            Assert.That(result.Code, Is.EqualTo(expectedCode));
            Assert.That(result.Success, Is.EqualTo(expectedSuccess));
            Assert.That(result.UserError, Is.EqualTo(expectedUserError));

            if (expectedSuccess)
            {
                Assert.That(result.Response, Is.Not.Null);
                Assert.That(Convert.ToInt32(result.Response), Is.EqualTo(999));
            }
            else
            {
                Assert.That(result.Response, Is.Null);
            }

            mockLogger.Verify(l => l.Information(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);

            if (throwsException)
            {
                mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
            }

            if (!isResponseSuccess && !throwsException)
            {
                mockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
            }
        }
    }
}