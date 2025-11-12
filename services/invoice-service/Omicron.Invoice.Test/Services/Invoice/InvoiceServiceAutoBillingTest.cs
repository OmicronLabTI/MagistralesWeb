// <summary>
// <copyright file="InvoiceServiceAutoBillingTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.Invoice
{
    /// <summary>
    /// Class InvoiceServiceAutoBillingTest.
    /// </summary>
    [TestFixture]
    public class InvoiceServiceAutoBillingTest : BaseTest
    {
        private Mock<IInvoiceDao> mockDao;
        private Mock<IUsersService> mockUsers;
        private Mock<IBackgroundTaskQueue> mockQueue;
        private Mock<IServiceScopeFactory> mockScopeFactory;
        private Mock<Serilog.ILogger> mockLogger;
        private Mock<ISapAdapter> mockSapAdapter;
        private Mock<ISapServiceLayerAdapterService> mockServiceLayer;
        private Mock<ICatalogsService> mockCatalogs;
        private Mock<IRedisService> mockRedis;
        private InvoiceService invoiceService;

        /// <summary>
        /// Initializes the mocks and test environment before each test.
        /// </summary>
        [SetUp]
        public void Init()
        {
            this.mockDao = new Mock<IInvoiceDao>();
            this.mockUsers = new Mock<IUsersService>();
            this.mockQueue = new Mock<IBackgroundTaskQueue>();
            this.mockScopeFactory = new Mock<IServiceScopeFactory>();
            this.mockLogger = new Mock<Serilog.ILogger>();
            this.mockSapAdapter = new Mock<ISapAdapter>();
            this.mockServiceLayer = new Mock<ISapServiceLayerAdapterService>();
            this.mockCatalogs = new Mock<ICatalogsService>();
            this.mockRedis = new Mock<IRedisService>();

            this.invoiceService = new InvoiceService(
                this.mockDao.Object,
                this.mockQueue.Object,
                this.mockScopeFactory.Object,
                this.mockLogger.Object,
                this.mockSapAdapter.Object,
                this.mockServiceLayer.Object,
                this.mockCatalogs.Object,
                this.mockRedis.Object,
                this.mockUsers.Object);
        }

        /// <summary>
        /// Test to verify successful execution of GetAutoBillingAsync.
        /// Ensures all dependent DAO calls and user mapping are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetAutoBillingAsync_Success()
        {
            // arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "status", "SUCCESS" }
            };

            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel { Id = "INV-001", AlmacenUser = "USR-001", BillingType = "AUTO", TypeInvoice = "A", RetryNumber = 0 },
                new InvoiceModel { Id = "INV-002", AlmacenUser = "USR-002", BillingType = "AUTO", TypeInvoice = "B", RetryNumber = 1 }
            };

            var sapOrders = new Dictionary<string, List<InvoiceSapOrderModel>>
            {
                { "INV-001", new List<InvoiceSapOrderModel> { new InvoiceSapOrderModel { SapOrderId = 123 } } },
                { "INV-002", new List<InvoiceSapOrderModel> { new InvoiceSapOrderModel { SapOrderId = 456 } } }
            };

            var remissions = new Dictionary<string, List<InvoiceRemissionModel>>
            {
                { "INV-001", new List<InvoiceRemissionModel> { new InvoiceRemissionModel { RemissionId = 11 } } },
                { "INV-002", new List<InvoiceRemissionModel> { new InvoiceRemissionModel { RemissionId = 22 } } }
            };

            var errorCatalog = new List<InvoiceErrorModel>
            {
                new InvoiceErrorModel { Code = "E001", ErrorMessage = "Error de prueba" }
            };

            var users = new List<UserModel>
            {
                new UserModel { Id = "USR-001", FirstName = "Ana", LastName = "López" },
                new UserModel { Id = "USR-002", FirstName = "Carlos", LastName = "Ramírez" }
            };

            this.mockDao.Setup(x => x.GetAutoBillingBaseAsync(It.IsAny<List<string>>(), 0, 10))
                .ReturnsAsync(invoices);
            this.mockDao.Setup(x => x.GetAutoBillingCountAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(2);
            this.mockDao.Setup(x => x.GetSapOrdersByInvoiceIdsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(sapOrders);
            this.mockDao.Setup(x => x.GetRemissionsByInvoiceIdsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(remissions);
            this.mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(errorCatalog);

            this.mockUsers.Setup(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceUtils.CreateResult(
                    true,
                    200,
                    null,
                    JsonConvert.SerializeObject(users),
                    null,
                    null));

            // act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // assert
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.IsNotNull(result.Response);
            ClassicAssert.AreEqual(2, ((List<AutoBillingRowDto>)result.Response).Count);
            this.mockDao.Verify(x => x.GetAutoBillingBaseAsync(It.IsAny<List<string>>(), 0, 10), Times.Once);
            this.mockUsers.Verify(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test to validate empty response when no invoices are returned.
        /// Ensures method completes successfully with an empty dataset.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task GetAutoBillingAsync_Empty()
        {
            // arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" }
            };

            this.mockDao.Setup(x => x.GetAutoBillingBaseAsync(It.IsAny<List<string>>(), 0, 10))
                .ReturnsAsync(new List<InvoiceModel>());
            this.mockDao.Setup(x => x.GetAutoBillingCountAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(0);
            this.mockUsers.Setup(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(new List<UserModel>()), null, null));

            // act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // assert
            ClassicAssert.IsTrue(result.Success);
            ClassicAssert.AreEqual(0, ((List<AutoBillingRowDto>)result.Response).Count);
        }

        /// <summary>
        /// Test to validate exception handling within GetAutoBillingAsync.
        /// Ensures method logs error and rethrows exceptions when DAO fails.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public void GetAutoBillingAsync_ThrowsException()
        {
            // arrange
            var parameters = new Dictionary<string, string> { { "offset", "0" }, { "limit", "10" } };
            this.mockDao.Setup(x => x.GetAutoBillingBaseAsync(It.IsAny<List<string>>(), 0, 10))
                .ThrowsAsync(new Exception("Database error"));

            // act & assert
            Assert.ThrowsAsync<Exception>(async () => await this.invoiceService.GetAutoBillingAsync(parameters));
        }
    }
}
