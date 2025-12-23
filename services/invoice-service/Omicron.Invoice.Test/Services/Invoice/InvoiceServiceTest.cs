// <summary>
// <copyright file="InvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.Invoice
{
    /// <summary>
    /// Unit tests for <see cref="InvoiceService.GetAutoBillingAsync"/>.
    /// </summary>
    [TestFixture]
    public class InvoiceServiceTest : BaseTest
    {
        /// <summary>
        /// Validates successful retrieval of AutoBilling data.
        /// Ensures that the service correctly returns data when the DAO contains results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingAsync_Success()
        {
            // Arrange
            var mockDao = new Mock<IInvoiceDao>();
            var mockUsers = new Mock<IUsersService>();
            var mockQueue = new Mock<IBackgroundTaskQueue>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            var mockLogger = new Mock<Serilog.ILogger>();
            var mockSapAdapter = new Mock<ISapAdapter>();
            var mockServiceLayer = new Mock<ISapServiceLayerAdapterService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockRedis = new Mock<IRedisService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(new List<InvoiceModel>
                {
                    new InvoiceModel
                    {
                        Id = "INV-001",
                        AlmacenUser = "U001",
                        InvoiceCreateDate = DateTime.Now,
                        SapOrders = new List<InvoiceSapOrderModel>(),
                        Remissions = new List<InvoiceRemissionModel>(),
                    },
                });

            mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(1);

            mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(new List<InvoiceErrorModel>());

            mockUsers
                .Setup(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceUtils.CreateResult(
                    true,
                    200,
                    null,
                    JsonConvert.SerializeObject(new List<UserModel>()),
                    null,
                    null));

            var service = new InvoiceService(
                mockDao.Object,
                mockQueue.Object,
                mockScopeFactory.Object,
                mockLogger.Object,
                mockSapAdapter.Object,
                mockServiceLayer.Object,
                mockCatalogs.Object,
                mockRedis.Object,
                mockUsers.Object,
                processPaymentServiceMock.Object);

            var parameters = new Dictionary<string, string> { { "offset", "0" }, { "limit", "10" } };

            // Act
            var result = await service.GetAutoBillingAsync(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Response, Is.Not.Null);
            });
        }

        /// <summary>
        /// Validates correct handling when no AutoBilling records are found.
        /// Ensures that the service still returns a successful response even when the dataset is empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingAsync_EmptyResult()
        {
            // Arrange
            var mockDao = new Mock<IInvoiceDao>();
            var mockUsers = new Mock<IUsersService>();
            var mockQueue = new Mock<IBackgroundTaskQueue>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            var mockLogger = new Mock<Serilog.ILogger>();
            var mockSapAdapter = new Mock<ISapAdapter>();
            var mockServiceLayer = new Mock<ISapServiceLayerAdapterService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockRedis = new Mock<IRedisService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(new List<InvoiceModel>());

            mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(new List<InvoiceErrorModel>());

            mockUsers
                .Setup(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceUtils.CreateResult(
                    true,
                    200,
                    null,
                    JsonConvert.SerializeObject(new List<UserModel>()),
                    null,
                    null));

            var service = new InvoiceService(
                mockDao.Object,
                mockQueue.Object,
                mockScopeFactory.Object,
                mockLogger.Object,
                mockSapAdapter.Object,
                mockServiceLayer.Object,
                mockCatalogs.Object,
                mockRedis.Object,
                mockUsers.Object,
                processPaymentServiceMock.Object);

            var parameters = new Dictionary<string, string> { { "offset", "0" }, { "limit", "10" } };

            // Act
            var result = await service.GetAutoBillingAsync(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(((List<AutoBillingRowDto>)result.Response).Count, Is.EqualTo(0));
            });
        }
    }
}
