// <summary>
// <copyright file="OrderServiceTest.cs" company="Axity">
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
    public class OrderServiceTest : BaseTest
    {
        private Mock<ILogger> logger;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            this.logger = new Mock<ILogger>();
        }

        /// <summary>
        /// Method to GetOrdersHeaderStatus for dxp project.
        /// </summary>
        /// <param name="isSuccess">Is Success.</param>
        /// <param name="userError">User Error.</param>
        /// <param name="code">Code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true, null, 200)]
        [TestCase(false, "Invalid session or session already timeout.", 401)]
        public async Task GetLastGeneratedOrder(bool isSuccess, string userError, int code)
        {
            // arrange
            var resultServiceLayer = new ResultModel
            {
                Success = isSuccess,
                Response = isSuccess ?
                JsonConvert.SerializeObject(new ServiceLayerResponseDto
                {
                    Metadata = "metadata",
                    Value = JsonConvert.SerializeObject(new List<OrderDto>()),
                }) : JsonConvert.SerializeObject(new ServiceLayerErrorDetailDto()),
                UserError = userError,
                ExceptionMessage = null,
                Code = code,
                Comments = null,
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            mockServiceLayerClient
               .Setup(x => x.GetAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(resultServiceLayer));

            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object);

            // act
            var result = await orderServiceMock.GetLastGeneratedOrder();

            // assert
            if (isSuccess)
            {
                Assert.True(result.Success);
                Assert.True(result.Code == 200);
                Assert.IsNotNull(result.Response);
                Assert.IsNull(result.UserError);
                Assert.IsInstanceOf<List<OrderDto>>(result.Response);
            }
            else
            {
                Assert.IsFalse(result.Success);
                Assert.True(result.Code == 401);
                Assert.IsNotNull(result.Response);
                Assert.IsNotNull(result.UserError);
                Assert.AreEqual("Invalid session or session already timeout.", result.UserError);
                Assert.IsInstanceOf<ServiceLayerErrorResponseDto>(result.Response);
            }

            Assert.IsNotNull(result);
            Assert.IsNull(result.ExceptionMessage);
            Assert.IsNull(result.Comments);
        }

        /// <summary>
        /// Close sample orders.
        /// </summary>
        /// <param name="isResponseOrderSuccess">Is Response Order Success.</param>
        /// <param name="isResponseInventoryGenExitSuccess">Is Response Inventory Gen Exit Success.</param>
        /// <param name="isCloseOrderSuccess">Is Close Order Success.</param>
        /// <param name="userError">User Error.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(false, true, true, "No existen registros coincidentes (ODBC -2028)")]
        [TestCase(true, false, true, "Error al crear el Inventory Gen Exit")]
        [TestCase(true, true, false, "Error al cerrar la orden")]
        [TestCase(true, true, true, null)]
        public async Task CloseSampleOrders(bool isResponseOrderSuccess, bool isResponseInventoryGenExitSuccess, bool isCloseOrderSuccess, string userError)
        {
            var sampleOrders = new List<CloseSampleOrderDto>
            {
                new ()
                {
                    SaleOrderId = 1,
                    ItemsList = new List<CreateDeliveryDto>
                    {
                        new ()
                        {
                            OrderType = "linea",
                            ItemCode = "Item Code 23",
                            Batches = new List<BatchNumbersDto>
                            {
                                new () { BatchNumber = "BATCH1", Quantity = 1 },
                                new () { BatchNumber = "BATCH2", Quantity = 2.5 },
                            },
                        },
                        new ()
                        {
                            OrderType = "magistral",
                            ItemCode = "DZ 50",
                        },
                        new ()
                        {
                            OrderType = "magistral",
                            ItemCode = "FL 1",
                        },
                    },
                },
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var orderDtoMock = new OrderDto();
            if (isResponseOrderSuccess)
            {
                orderDtoMock = new OrderDto
                {
                    DocumentEntry = 3,
                    OrderLines = new List<OrderLineDto>
                    {
                        new () { ItemCode = "Item Code 23", LineNum = 0, Quantity = 1 },
                        new () { ItemCode = "DZ 50", LineNum = 1, Quantity = 1 },
                        new () { ItemCode = "FL 1", LineNum = 2, Quantity = 1 },
                    },
                };
            }

            var responseOrder = this.GetGenericResponseModel(400, isResponseOrderSuccess, orderDtoMock, userError, null, null);
            mockServiceLayerClient
                .Setup(sl => sl.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(responseOrder));

            var responseInventoryGenExit = this.GetGenericResponseModel(400, isResponseInventoryGenExitSuccess, null, userError, null, null);
            var responseCloseOrder = this.GetGenericResponseModel(400, isCloseOrderSuccess, null, userError, null, null);

            mockServiceLayerClient
                .SetupSequence(sl => sl.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(responseInventoryGenExit))
                .Returns(Task.FromResult(responseCloseOrder));

            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object);

            // act
            var result = await orderServiceMock.CloseSampleOrders(sampleOrders);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResultModel>(result);
            if (!isResponseOrderSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.AreEqual("Error-No se encontró la factura.", resultDict.Value);
            }
            else if (!isResponseInventoryGenExitSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.AreEqual("Error-Error al crear el Inventory Gen Exit", resultDict.Value);
            }
            else if (!isCloseOrderSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.AreEqual("Error-Error al cerrar la orden", resultDict.Value);
            }
            else
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.AreEqual("Ok", resultDict.Value);
            }

            Assert.IsNull(result.UserError);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Response);
            Assert.IsNull(result.Comments);
        }
    }
}
