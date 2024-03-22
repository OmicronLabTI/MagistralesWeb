// <summary>
// <copyright file="OrdersServiceTest.cs" company="Axity">
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
    public class OrdersServiceTest : BaseTest
    {
        private IOrdersService ordersService;

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

            var orderServiceMock = new OrdersService(mockServiceLayerClient.Object);

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

    }
}
