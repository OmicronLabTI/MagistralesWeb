// <summary>
// <copyright file="InventoryTransferRequestTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services.InventoryTransferRequest
{
    /// <summary>
    /// Class InventoryTransferRequestTest.
    /// </summary>
    [TestFixture]
    public class InventoryTransferRequestTest : BaseTest
    {
        private Mock<ILogger> mockLogger;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            this.mockLogger = new Mock<ILogger>();
        }

        /// <summary>
        /// Test for Create Transfer Request.
        /// </summary>
        /// <param name="isSuccesfully">Is Succesfully.</param>
        /// <param name="responseCode">Response Code.</param>
        /// <param name="userError">User Error.</param>
        /// <param name="isThrowException">Is Throw Exception.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [Test]
        [TestCase(true, 200, "", false)]
        [TestCase(false, 400, "Error al crear el Transfer Request.", false)]
        [TestCase(false, 400, "Timeout Exception", true)]
        public async Task CreateTransferRequest(bool isSuccesfully, int responseCode, string userError, bool isThrowException)
        {
            // Arrange
            var transferRequestHeader = new List<TransferRequestHeaderDto>
            {
                new ()
                {
                    IsLabel = true,
                    UserId = "7a4b3b2a-39c7-4c1f-a5c2-9e279aef94d7",
                    UserInfo = "Test-Test-7a4b3b2a-39c7-4c1f-a5c2-9e279aef94d7",
                    TransferRequestDetail = new List<TransferRequestDetailDto>
                    {
                        new ()
                        {
                            ItemCode = "Product Item Code",
                            ItemDescription = "Descripcón larga del producto",
                            Quantity = 1,
                            SourceWarehosue = "BE",
                            TargetWarehosue = "MP",
                        },
                    },
                },
            };

            var responseServiceLayerObject = new InventoryTransferRequestsResponseDto
            {
                DocEntry = 1,
                DocumentDate = DateTime.Now,
                JournalMemo = "Test-Test-7a4b3b2a-39c7-4c1f-a5c2-9e279aef94d7",
                RequestedUserId = "7a4b3b2a-39c7-4c1f-a5c2-9e279aef94d7",
                StockTransferLines = new List<StockTransferLinesDto>
                {
                    new ()
                    {
                        FromWarehouseCode = "MP",
                        WarehouseCode = "BE",
                        ItemDescription = "Descripcón larga del producto",
                        Quantity = 1,
                        ItemCode = "Product Item Code",
                    },
                },
            };

            var responseTransferRequest = GetGenericResponseModel(responseCode, isSuccesfully, responseServiceLayerObject, userError);

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            if (!isThrowException)
            {
                mockServiceLayerClient
                .Setup(ts => ts.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(responseTransferRequest));
            }
            else
            {
                mockServiceLayerClient
                .Setup(ts => ts.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(userError));
            }

            // Act
            var mockTransferRequestService = new InventoryTransferRequestService(mockServiceLayerClient.Object, this.mockLogger.Object);
            var result = await mockTransferRequestService.CreateTransferRequest(transferRequestHeader);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Comments, Is.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Response, Is.InstanceOf<List<InventoryTransferRequestResult>>());
            var responseObject = (List<InventoryTransferRequestResult>)result.Response;

            if (isSuccesfully)
            {
                Assert.That(responseObject.All(ts => string.IsNullOrEmpty(ts.Error)), Is.True);
                Assert.That(responseObject.All(ts => ts.TransferRequestId > 0), Is.True);
            }
            else
            {
                Assert.That(responseObject.All(ts => !string.IsNullOrEmpty(ts.Error)), Is.True);
                Assert.That(responseObject.All(ts => ts.Error.Contains("ErrorTransferRequest-" + userError)), Is.True);
            }
        }
    }
}
