// <summary>
// <copyright file="DeliveryNoteServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services.DeliveryNotes
{
    /// <summary>
    /// Class DeliveryNoteServiceTest.
    /// </summary>
    public class DeliveryNoteServiceTest : BaseTest
    {
        private Mock<ILogger> mockLogger;
        private IDeliveryNoteService deliveryNoteService;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            this.mockLogger = new Mock<ILogger>();
            this.deliveryNoteService = new DeliveryNoteService(mockServiceLayerClient.Object, this.mockLogger.Object);
        }

        /// <summary>
        /// Method to create delivery with invalid order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryWithNotFoundOrder()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);

            var serviceLayerClientResult = new ResultModel()
            {
                Success = false,
                UserError = "Error 401",
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = string.Empty,
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);

            var result = await service.CreateDelivery(createDelivery);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
        }

        /// <summary>
        /// Method to verify Get All Almacens.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryWithShipping()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);
            var orderLines = new List<OrderLineDto>();

            var orderLine = new OrderLineDto()
            {
                ItemCode = "REVE 14",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 150,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
            };
            var orderLineShipping = new OrderLineDto()
            {
                ItemCode = "FL 1",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 300,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
                OwnerCode = 30,
            };
            orderLines.Add(orderLine);
            orderLines.Add(orderLineShipping);

            var saleOrder = new OrderDto()
            {
                IsOmigenomics = "N",
                CardCode = "C03580",
                SalesPersonCode = 30,
                DocumentsOwner = 0,
                BillingAddress = string.Empty,
                ShippingAddress = string.Empty,
                ShippingCode = string.Empty,
                JournalMemo = string.Empty,
                Comments = "Comentarios",
                OrderLines = orderLines,
            };

            var serviceLayerClientResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = JsonConvert.SerializeObject(saleOrder),
                Code = 200,
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var patchResul = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(patchResul));

            var createResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(createResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = "linea",
                Batches = new List<AlmacenBatchDto>()
                {
                    new AlmacenBatchDto()
                    {
                        BatchNumber = "Axity.10",
                        BatchQty = 1,
                    },
                },
                IsPackage = "N",
                IsOmigenomics = false,
            };

            var shippingItem = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "FL 1",
                OrderType = "300",
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);
            createDelivery.Add(shippingItem);

            var result = await service.CreateDelivery(createDelivery);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
        }

        /// <summary>
        /// Method to verify Get All Almacens.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns></summary>
        [Test]
        public async Task CreateDeliveryPartialWithNotFoundOrder()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);

            var serviceLayerClientResult = new ResultModel()
            {
                Success = false,
                UserError = "Error 401",
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = string.Empty,
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);

            var result = await service.CreateDeliveryPartial(createDelivery);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
        }

        /// <summary>
        /// Method to verify CreateDeliveryPartialWithShipping.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns></summary>
        [Test]
        public async Task CreateDeliveryPartialWithShipping()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);
            var orderLines = new List<OrderLineDto>();

            var orderLine = new OrderLineDto()
            {
                ItemCode = "REVE 14",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 150,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
            };
            var orderLineShipping = new OrderLineDto()
            {
                ItemCode = "FL 1",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 300,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
                OwnerCode = 30,
            };
            orderLines.Add(orderLine);
            orderLines.Add(orderLineShipping);

            var saleOrder = new OrderDto()
            {
                IsOmigenomics = "N",
                CardCode = "C03580",
                SalesPersonCode = 30,
                DocumentsOwner = 0,
                BillingAddress = string.Empty,
                ShippingAddress = string.Empty,
                ShippingCode = string.Empty,
                JournalMemo = string.Empty,
                Comments = "Comentarios",
                OrderLines = orderLines,
            };

            var serviceLayerClientResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = JsonConvert.SerializeObject(saleOrder),
                Code = 200,
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var patchResul = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(patchResul));

            var createResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(createResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = "linea",
                Batches = new List<AlmacenBatchDto>()
                {
                    new AlmacenBatchDto()
                    {
                        BatchNumber = "Axity.10",
                        BatchQty = 1,
                    },
                },
                IsPackage = "N",
                IsOmigenomics = false,
            };

            var shippingItem = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "FL 1",
                OrderType = "300",
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);
            createDelivery.Add(shippingItem);

            var result = await service.CreateDeliveryPartial(createDelivery);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
        }

        /// <summary>
        /// Test for cancel delivery.
        /// </summary>
        /// <param name="isResponseDeliveryNoteSuccess">isResponseDeliveryNoteSuccess.</param>
        /// <param name="isResponseCancellationDocumentSuccess">isResponseCancellationDocumentSuccess.</param>
        /// <param name="isStockTransferSuccess">isStockTransferSuccess.</param>
        /// <param name="isCancelOrderSuccess">Is Cancel Order Success.</param>
        /// <param name="userError">userError.</param>
        /// <param name="type">type.</param>
        /// <returns>Result.</returns>
        [Test]
        [TestCase(false, true, true, true, "No se encontró el documento delivery notes", "total")]
        [TestCase(true, false, true, true, "Error al cancelar el documento", "total")]
        [TestCase(true, true, false, true, "Error al generar la tranferencia de stock", "total")]
        [TestCase(true, true, true, false, "Error al cancelar orden", "total")]
        [TestCase(true, true, true, true, null, "total")]
        [TestCase(true, true, true, true, null, "partial")]
        public async Task CancelDelivery(
            bool isResponseDeliveryNoteSuccess,
            bool isResponseCancellationDocumentSuccess,
            bool isStockTransferSuccess,
            bool isCancelOrderSuccess,
            string userError,
            string type)
        {
            // Arrange
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockDeliveryNoteService = new DeliveryNoteService(mockServiceLayerClient.Object, this.mockLogger.Object);

            var mockDeliveryNoteResponse = new DeliveryNoteDto
            {
                DeliveryOrderType = "MX",
            };

            var responseDeliveryNote = GetGenericResponseModel(400, isResponseDeliveryNoteSuccess, mockDeliveryNoteResponse, userError, null, null);
            mockServiceLayerClient
                .Setup(sl => sl.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(responseDeliveryNote));

            var responseCancellationDocument = GetGenericResponseModel(400, isResponseCancellationDocumentSuccess, null, userError, null, null);
            var responseStockTransfer = GetGenericResponseModel(400, isStockTransferSuccess, null, userError, null, null);
            var responseOrderCancel = GetGenericResponseModel(400, isCancelOrderSuccess, null, userError, null, null);

            mockServiceLayerClient
                .SetupSequence(sl => sl.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(responseCancellationDocument))
                .Returns(Task.FromResult(responseOrderCancel))
                .Returns(Task.FromResult(responseStockTransfer));

            var deliveryNotesToCancel = new List<CancelDeliveryDto>
            {
                new ()
                {
                    Delivery = 123,
                    SaleOrderId = new List<int> { 1, 2 },
                    MagistralProducts = new List<ProductDeliveryDto>
                    {
                        new ()
                        {
                            ItemCode = "ItemCode1",
                            Pieces = 1,
                        },
                        new ()
                        {
                            ItemCode = "ItemCode2",
                            Pieces = 2,
                        },
                    },
                },
            };

            // Act
            var result = await mockDeliveryNoteService.CancelDelivery(type, deliveryNotesToCancel);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf<ResultModel>(result);
            if (!isResponseDeliveryNoteSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                ClassicAssert.AreEqual("Error-No se encontró el documento delivery notes", resultDict.Value);
            }
            else if (!isResponseCancellationDocumentSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                ClassicAssert.AreEqual("Error-Error al cancelar el documento", resultDict.Value);
            }
            else if (!isStockTransferSuccess)
            {
                ClassicAssert.AreEqual(3, ((Dictionary<string, string>)result.Response).Count);
            }
            else if (!isResponseCancellationDocumentSuccess)
            {
                ClassicAssert.AreEqual(4, ((Dictionary<string, string>)result.Response).Count);
            }
            else
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                ClassicAssert.AreEqual("Ok", resultDict.Value);
            }

            ClassicAssert.IsNull(result.UserError);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsNotNull(result.Response);
            ClassicAssert.IsNull(result.Comments);
        }

        /// <summary>
        /// Method to create delivery partial with invalid order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryBacthWithNotFoundOrder()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);

            var serviceLayerClientResult = new ResultModel()
            {
                Success = false,
                UserError = "Error 401",
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = string.Empty,
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);

            var result = await service.CreateDeliveryBatch(createDelivery);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
        }

        /// <summary>
        /// Method to verify Get All Almacens.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryBatchWithShipping()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);
            var orderLines = new List<OrderLineDto>();

            var orderLine = new OrderLineDto()
            {
                ItemCode = "REVE 14",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 150,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
            };
            var orderLineShipping = new OrderLineDto()
            {
                ItemCode = "FL 1",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 300,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
                OwnerCode = 30,
            };
            orderLines.Add(orderLine);
            orderLines.Add(orderLineShipping);

            var saleOrder = new OrderDto()
            {
                IsOmigenomics = "N",
                CardCode = "C03580",
                SalesPersonCode = 30,
                DocumentsOwner = 0,
                BillingAddress = string.Empty,
                ShippingAddress = string.Empty,
                ShippingCode = string.Empty,
                JournalMemo = string.Empty,
                Comments = "Comentarios",
                OrderLines = orderLines,
            };

            var serviceLayerClientResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = JsonConvert.SerializeObject(saleOrder),
                Code = 200,
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var patchResul = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(patchResul));

            var createResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(createResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = "linea",
                Batches = new List<AlmacenBatchDto>()
                {
                    new AlmacenBatchDto()
                    {
                        BatchNumber = "Axity.10",
                        BatchQty = 1,
                    },
                },
                IsPackage = "N",
                IsOmigenomics = false,
            };

            var shippingItem = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 15,
                ItemCode = "FL 1",
                OrderType = "300",
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);
            createDelivery.Add(shippingItem);

            var result = await service.CreateDeliveryBatch(createDelivery);
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(200, result.Code);
        }
    }
}