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
    /// Unit tests for <see cref="InvoiceService.GetAutoBillingAsync"/>.
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
        /// Verifies successful execution of GetAutoBillingAsync, ensuring correct DAO calls and data mapping.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingAsync_Success()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "status", "SUCCESS" },
            };

            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    Id = "INV-001",
                    AlmacenUser = "USR-001",
                    BillingType = "AUTO",
                    TypeInvoice = "A",
                    RetryNumber = 0,
                    InvoiceCreateDate = DateTime.Now.AddDays(-1),
                    SapOrders = new List<InvoiceSapOrderModel>
                    {
                        new InvoiceSapOrderModel { SapOrderId = 123 },
                    },
                    Remissions = new List<InvoiceRemissionModel>
                    {
                        new InvoiceRemissionModel { RemissionId = 11 },
                    },
                },
                new InvoiceModel
                {
                    Id = "INV-002",
                    AlmacenUser = "USR-002",
                    BillingType = "AUTO",
                    TypeInvoice = "B",
                    RetryNumber = 1,
                    InvoiceCreateDate = DateTime.Now.AddDays(-2),
                    SapOrders = new List<InvoiceSapOrderModel>
                    {
                        new InvoiceSapOrderModel { SapOrderId = 456 },
                    },
                    Remissions = new List<InvoiceRemissionModel>
                    {
                        new InvoiceRemissionModel { RemissionId = 22 },
                    },
                },
            };

            var errorCatalog = new List<InvoiceErrorModel>
            {
                new InvoiceErrorModel { Code = "E001", ErrorMessage = "Error de prueba" },
            };

            var users = new List<UserModel>
            {
                new UserModel { Id = "USR-001", FirstName = "Ana", LastName = "López" },
                new UserModel { Id = "USR-002", FirstName = "Carlos", LastName = "Ramírez" },
            };

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(invoices);

            this.mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(2);

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

            // Act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(((List<AutoBillingRowDto>)result.Response).Count, Is.EqualTo(2));
            });

            this.mockDao.Verify(
                x => x.GetAutoBillingByFilters(
                It.IsAny<List<string>>(),
                It.IsAny<List<string>>(),
                It.IsAny<List<string>>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                0,
                10), Times.Once);

            this.mockUsers.Verify(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Verifies GetAutoBillingAsync returns an empty response when no invoices exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingAsync_Empty()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
            };

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(new List<InvoiceModel>());

            this.mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            this.mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(new List<InvoiceErrorModel>());

            this.mockUsers.Setup(x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .ReturnsAsync(ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(new List<UserModel>()), null, null));

            // Act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(((List<AutoBillingRowDto>)result.Response).Count, Is.EqualTo(0));
            });
        }

        /// <summary>
        /// Verifies GetAutoBillingAsync properly throws when DAO fails.
        /// </summary>
        [Test]
        public void GetAutoBillingAsync_ThrowsException()
        {
            // Arrange
            var parameters = new Dictionary<string, string> { { "offset", "0" }, { "limit", "10" } };

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await this.invoiceService.GetAutoBillingAsync(parameters));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }

        /// <summary>
        /// Verifies GetAutoBillingAsync correctly applies TypeInvoice filter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBilling_InvoiceFilterSuccess()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "status", "SUCCESS" },
                { "typeInvoice", "Genérica,No genérica" },
            };

            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    Id = "INV-001",
                    AlmacenUser = "USR-001",
                    TypeInvoice = "Genérica",
                    BillingType = "Parcial",
                    InvoiceCreateDate = DateTime.Now,
                    RetryNumber = 0,
                    ErrorMessage = null,
                    UpdateDate = DateTime.Now,
                    IdFacturaSap = 123,
                    DxpOrderId = "DXP-001",
                    SapOrders = new List<InvoiceSapOrderModel>
                    {
                        new InvoiceSapOrderModel { Id = 1, SapOrderId = 1001, IdInvoice = "INV-001" },
                    },
                    Remissions = new List<InvoiceRemissionModel>
                    {
                        new InvoiceRemissionModel { Id = 1, RemissionId = 2001, IdInvoice = "INV-001" },
                    },
                },
            };

            var errorCatalog = new List<InvoiceErrorModel>
                {
                    new InvoiceErrorModel { Id = 1, Code = "E001", ErrorMessage = "Test error" },
                };

            var users = new List<UserModel>
                {
                    new UserModel { Id = "USR-001", FirstName = "Test", LastName = "User" },
                };

            var usersJson = JsonConvert.SerializeObject(users);

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(invoices);

            this.mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(1);

            this.mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(errorCatalog);

            this.mockUsers.Setup(x => x.GetUsersById(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new ResultDto
                {
                    Success = true,
                    Code = 200,
                    Response = usersJson,
                    UserError = null,
                    ExceptionMessage = null,
                    Comments = null,
                });

            // Act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);

                var responseList = (List<AutoBillingRowDto>)result.Response;
                Assert.That(responseList.Count, Is.EqualTo(1));
                Assert.That(responseList[0].TypeInvoice, Is.EqualTo("Genérica"));
                Assert.That(responseList[0].AlmacenUser, Is.EqualTo("Test User"));
            });
        }

        /// <summary>
        /// Verifies GetAutoBillingAsync correctly applies BillingType filter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetBillingTypeFilter_Success()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
        {
            { "offset", "0" },
            { "limit", "10" },
            { "status", "SUCCESS" },
            { "billingType", "Parcial,Completa" },
        };

            var invoices = new List<InvoiceModel>
        {
            new InvoiceModel
            {
                Id = "INV-001",
                AlmacenUser = "USR-001",
                TypeInvoice = "Genérica",
                BillingType = "Parcial",
                InvoiceCreateDate = DateTime.Now,
                RetryNumber = 0,
                ErrorMessage = null,
                UpdateDate = DateTime.Now,
                IdFacturaSap = 123,
                DxpOrderId = "DXP-001",
                SapOrders = new List<InvoiceSapOrderModel>(),
                Remissions = new List<InvoiceRemissionModel>(),
            },
        };

            var errorCatalog = new List<InvoiceErrorModel>
                    {
                        new InvoiceErrorModel { Code = "E001", ErrorMessage = "Test error" },
                    };

            var users = new List<UserModel>
                    {
                        new UserModel { Id = "USR-001", FirstName = "Test", LastName = "User" },
                    };

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.Is<List<string>>(list => list.Contains("Parcial") && list.Contains("Completa")),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(invoices);

            this.mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(1);

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

            // Act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(((List<AutoBillingRowDto>)result.Response).Count, Is.EqualTo(1));
            });
        }

        /// <summary>
        /// Verifies filters combination works correctly (typeInvoice + billingType).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingAsync_WithCombinedFilters_Success()
        {
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "status", "SUCCESS" },
                { "typeInvoice", "Genérica,No genérica" },
                { "billingType", "Parcial,Completa" },
            };

            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    Id = "INV-001",
                    AlmacenUser = "USR-001",
                    TypeInvoice = "Genérica",
                    BillingType = "Parcial",
                    InvoiceCreateDate = DateTime.Now,
                    RetryNumber = 0,
                    SapOrders = new List<InvoiceSapOrderModel>(),
                    Remissions = new List<InvoiceRemissionModel>(),
                },
                new InvoiceModel
                {
                    Id = "INV-002",
                    AlmacenUser = "USR-001",
                    TypeInvoice = "No genérica",
                    BillingType = "Completa",
                    InvoiceCreateDate = DateTime.Now,
                    RetryNumber = 0,
                    SapOrders = new List<InvoiceSapOrderModel>(),
                    Remissions = new List<InvoiceRemissionModel>(),
                },
            };

            var errorCatalog = new List<InvoiceErrorModel>();
            var users = new List<UserModel>
            {
                new UserModel { Id = "USR-001", FirstName = "Test", LastName = "User" },
            };
            var usersJson = JsonConvert.SerializeObject(users);

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(invoices);

            this.mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(2);

            this.mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(errorCatalog);

            this.mockUsers.Setup(x => x.GetUsersById(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new ResultDto
                {
                    Success = true,
                    Code = 200,
                    Response = usersJson,
                });

            // Act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // Assert
            var responseList = (List<AutoBillingRowDto>)result.Response;
            Assert.That(responseList.Count, Is.EqualTo(2));

            Assert.That(responseList.Any(x => x.TypeInvoice == "Genérica"), Is.True);
            Assert.That(responseList.Any(x => x.TypeInvoice == "No genérica"), Is.True);

            Assert.That(responseList.Any(x => x.BillingType == "Parcial"), Is.True);
            Assert.That(responseList.Any(x => x.BillingType == "Completa"), Is.True);
        }

        /// <summary>
        /// Verifies default behavior when no type/billing filters are provided.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingAsync_WithoutTypeAndBillingFilters_ReturnsAllTypes()
        {
            var parameters = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "status", "SUCCESS" },
            };

            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    Id = "INV-001",
                    AlmacenUser = "USR-001",
                    TypeInvoice = "Genérica",
                    BillingType = "Parcial",
                    InvoiceCreateDate = DateTime.Now,
                    RetryNumber = 0,
                    SapOrders = new List<InvoiceSapOrderModel>(),
                    Remissions = new List<InvoiceRemissionModel>(),
                },
                new InvoiceModel
                {
                    Id = "INV-002",
                    AlmacenUser = "USR-001",
                    TypeInvoice = "No genérica",
                    BillingType = "Completa",
                    InvoiceCreateDate = DateTime.Now,
                    RetryNumber = 0,
                    SapOrders = new List<InvoiceSapOrderModel>(),
                    Remissions = new List<InvoiceRemissionModel>(),
                },
            };

            var errorCatalog = new List<InvoiceErrorModel>();
            var users = new List<UserModel>
            {
                new UserModel { Id = "USR-001", FirstName = "Test", LastName = "User" },
            };
            var usersJson = JsonConvert.SerializeObject(users);

            this.mockDao.Setup(x => x.GetAutoBillingByFilters(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    0,
                    10))
                .ReturnsAsync(invoices);

            this.mockDao.Setup(x => x.GetAutoBillingCount(
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(2);

            this.mockDao.Setup(x => x.GetAllErrors())
                .ReturnsAsync(errorCatalog);

            this.mockUsers.Setup(x => x.GetUsersById(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new ResultDto
                {
                    Success = true,
                    Code = 200,
                    Response = usersJson,
                });

            // Act
            var result = await this.invoiceService.GetAutoBillingAsync(parameters);

            // Assert
            var responseList = (List<AutoBillingRowDto>)result.Response;
            Assert.That(responseList.Count, Is.EqualTo(2));

            Assert.That(responseList.Any(x => x.TypeInvoice == "Genérica"), Is.True);
            Assert.That(responseList.Any(x => x.TypeInvoice == "No genérica"), Is.True);
            Assert.That(responseList.Any(x => x.BillingType == "Parcial"), Is.True);
            Assert.That(responseList.Any(x => x.BillingType == "Completa"), Is.True);
        }
    }
}
