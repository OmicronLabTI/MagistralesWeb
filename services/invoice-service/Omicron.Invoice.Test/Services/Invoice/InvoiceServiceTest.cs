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
    /// Class InvoiceServiceTest.
    /// </summary>
    [TestFixture]
    public class InvoiceServiceTest : BaseTest
    {
        /// <summary>
        /// Test method that validates successful retrieval of AutoBilling data.
        /// Ensures that the service correctly returns data when the DAO contains results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
        [Test]
        public async Task GetAutoBillingAsync_Success()
        {
            // arrange
            var mockDao = new Mock<IInvoiceDao>();
            var mockUsers = new Mock<IUsersService>();
            var mockQueue = new Mock<IBackgroundTaskQueue>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            var mockLogger = new Mock<Serilog.ILogger>();
            var mockSapAdapter = new Mock<ISapAdapter>();
            var mockServiceLayer = new Mock<ISapServiceLayerAdapterService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockRedis = new Mock<IRedisService>();

            mockDao.Setup(x => x.GetAutoBillingBaseAsync(It.IsAny<List<string>>(), 0, 10))
                .ReturnsAsync(new List<InvoiceModel> { new InvoiceModel { Id = "INV-001", AlmacenUser = "U001" } });
            mockDao.Setup(x => x.GetAutoBillingCountAsync(It.IsAny<List<string>>())).ReturnsAsync(1);

            mockDao.Setup(x => x.GetSapOrdersByInvoiceIdsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new Dictionary<string, List<InvoiceSapOrderModel>>());

            mockDao.Setup(x => x.GetRemissionsByInvoiceIdsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new Dictionary<string, List<InvoiceRemissionModel>>());

            mockDao.Setup(x => x.GetAllErrors()).ReturnsAsync(new List<InvoiceErrorModel>());

            // ✅ Mock UsersService con JSON serializado para evitar errores de deserialización
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
                mockUsers.Object);

            var parameters = new Dictionary<string, string> { { "offset", "0" }, { "limit", "10" } };

            // act
            var result = await service.GetAutoBillingAsync(parameters);

            // assert
            ClassicAssert.IsTrue(result.Success);
        }

        /// <summary>
        /// Test method that validates correct handling when no AutoBilling records are found.
        /// Ensures that the service still returns a successful response, even when the data set is empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
        [Test]
        public async Task GetAutoBillingAsync_EmptyResult()
        {
            // arrange
            var mockDao = new Mock<IInvoiceDao>();
            var mockUsers = new Mock<IUsersService>();
            var mockQueue = new Mock<IBackgroundTaskQueue>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            var mockLogger = new Mock<Serilog.ILogger>();
            var mockSapAdapter = new Mock<ISapAdapter>();
            var mockServiceLayer = new Mock<ISapServiceLayerAdapterService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockRedis = new Mock<IRedisService>();

            mockDao.Setup(x => x.GetAutoBillingBaseAsync(It.IsAny<List<string>>(), 0, 10))
                .ReturnsAsync(new List<InvoiceModel>());
            mockDao.Setup(x => x.GetAutoBillingCountAsync(It.IsAny<List<string>>())).ReturnsAsync(0);

            // ✅ Mock UsersService con JSON serializado para mantener compatibilidad con la lógica de deserialización interna
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
                mockUsers.Object);

            var parameters = new Dictionary<string, string> { { "offset", "0" }, { "limit", "10" } };

            // act
            var result = await service.GetAutoBillingAsync(parameters);

            // assert
            ClassicAssert.IsTrue(result.Success);
        }
    }
}
