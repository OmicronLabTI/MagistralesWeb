// <summary>
// <copyright file="InvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services
{
    /// <summary>
    /// Class ProjectServiceTest.
    /// </summary>
    [TestFixture]
    public class InvoiceServiceTest : BaseTest
    {
        private IInvoiceService userService;
        private IInvoiceDao invoiceDao;
        private IMapper mapper;

        private DatabaseContext context;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalOrderValidationDBTest")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.Invoices.AddRange(this.GetAllInvoices());
            this.context.Remissions.AddRange(this.GetAllRemissions());
            this.context.InvoiceError.AddRange(this.GetAllErrors());
            this.context.InvoiceSapOrderModel.AddRange(this.GetInvoiceSapOrderModel());
            this.context.SaveChanges();

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var userServiceMock = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            this.invoiceDao = new InvoiceDao(this.context);
            this.userService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, userServiceMock.Object, processPaymentServiceMock.Object);
        }

        /// <summary>
        /// Method to verify carry out the order process.
        /// </summary>
        /// <returns> A <see cref="Task"/> representing the asynchronous unit test. </returns>
        [Test]
        public async Task RegisterInvoice()
        {
            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var userServiceMock = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            var invoiceDaoMock = new Mock<IInvoiceDao>();

            invoiceDaoMock.Setup(x => x.InsertInvoices(It.IsAny<List<InvoiceModel>>()))
                .Returns(Task.CompletedTask);

            invoiceDaoMock.Setup(x => x.InsertRemissions(It.IsAny<List<InvoiceRemissionModel>>()))
                .Returns(Task.CompletedTask);

            invoiceDaoMock.Setup(x => x.InsertSapOrders(It.IsAny<List<InvoiceSapOrderModel>>()))
                .Returns(Task.CompletedTask);

            var invoiceService = new InvoiceService(
                invoiceDaoMock.Object,
                taskQueue.Object,
                serviceScopeFactoryMock.Object,
                logger.Object,
                sapAdapterServiceMock.Object,
                servicelayerServiceMock.Object,
                catalogServiceMock.Object,
                redisServiceMock.Object,
                userServiceMock.Object,
                processPaymentServiceMock.Object);

            var request = new CreateInvoiceDto()
            {
                CardCode = "C03865",
                ProcessId = "XXXXXXXXXXXX1",
                CfdiDriverVersion = string.Empty,
                IdDeliveries = new List<int>() { 1, 2, 3 },
                IdSapOrders = new List<int>() { 1, 2, 3 },
                CreateUserId = "1",
                DxpOrderId = "XXXXXXX23",
                InvoiceType = "GENERICO",
                BillingType = "Completa",
            };

            // Act
            var response = await invoiceService.RegisterInvoice(request);

            // Assert
            Assert.That(response.Code.Equals(200));

            invoiceDaoMock.Verify(x => x.InsertInvoices(It.IsAny<List<InvoiceModel>>()), Times.Once);
            invoiceDaoMock.Verify(x => x.InsertRemissions(It.Is<List<InvoiceRemissionModel>>(r => r.Count == 3)), Times.Once);
            invoiceDaoMock.Verify(x => x.InsertSapOrders(It.Is<List<InvoiceSapOrderModel>>(s => s.Count == 3)), Times.Once);
        }

        /// <summary>
        /// Method to verify carry out the order process.
        /// </summary>
        /// <param name="generateCatchException">GenerateCatchException.</param>
        /// <returns> A <see cref="Task"/> representing the asynchronous unit test. </returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task PublishProcessToMediatR(bool generateCatchException)
        {
            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var userServiceMock = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            this.userService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, userServiceMock.Object, processPaymentServiceMock.Object);
            var request = new CreateInvoiceDto()
            {
                CardCode = "C03865",
                ProcessId = "XXXXXXXXXXXX1",
                CfdiDriverVersion = string.Empty,
                IdDeliveries = new List<int>() { 1, 2, 3 },
                IdSapOrders = new List<int>() { 1, 2, 3 },
                CreateUserId = "1",
                DxpOrderId = "XXXXXXX23",
                InvoiceType = "GENERICO",
                BillingType = "Completa",
            };

            if (generateCatchException)
            {
                taskQueue
                .Setup(x => x.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()))
                .Throws(new Exception("Invoice Service - Error sending to queue"));
            }

            var response = this.userService.PublishProcessToMediatR(request);

            // Assert
            Assert.That(response.Equals(!generateCatchException));
        }

        /// <summary>
        /// Method to verify carry out the order process.
        /// </summary>
        /// <param name="sucess">GenerateCatchException.</param>
        /// <param name="errorMessage">error message.</param>
        /// <param name="processId">the process id.</param>
        /// <param name="hasInvoice">has invoice.</param>
        /// <param name="cfdiVersion">Cfdi version.</param>
        /// <param name="redisValue">redis value.</param>
        /// <returns> A <see cref="Task"/> representing the asynchronous unit test. </returns>
        [Test]
        [TestCase(false, "No se encontró la factura con el id 100", "100")]
        [TestCase(false, "Ya se encuentra procesandose la factura 1", "1")]
        [TestCase(true, null, "2", true)]
        [TestCase(true, null, "2", false)]
        [TestCase(true, null, "2", false, "XXX", "")]
        [TestCase(true, null, "2", false, "", "")]
        [TestCase(false, "C01", "2", false, "", "")]
        public async Task CreateInvoice(bool sucess, string errorMessage, string processId, bool hasInvoice = false, string cfdiVersion = "XXXXXX1", string redisValue = "XXXXXX1")
        {
            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var userServiceMock = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            var catalogResult = catalogServiceMock
                .Setup(x => x.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResultModel(new List<ParametersDto> { new ParametersDto() { Value = cfdiVersion } })));
            var redisServiceMock = new Mock<IRedisService>();
            redisServiceMock
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(redisValue));

            sapAdapterServiceMock
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResultModel(new InvoicesDataDto() { HasInvoice = hasInvoice, InvoiceId = 1 })));

            servicelayerServiceMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResultModel(1)));

            if (!sucess)
            {
                servicelayerServiceMock
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new System.Exception("C01"));
            }

            this.userService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, userServiceMock.Object, processPaymentServiceMock.Object);
            var request = new CreateInvoiceDto()
            {
                CardCode = "C03865",
                ProcessId = processId,
                CfdiDriverVersion = string.Empty,
                IdDeliveries = new List<int>() { 1, 2, 3 },
                IdSapOrders = new List<int>() { 1, 2, 3 },
                CreateUserId = "1",
                DxpOrderId = "XXXXXXX23",
                InvoiceType = "GENERICO",
                BillingType = "Completa",
            };

            var response = await this.userService.CreateInvoice(request);

            // Assert
            Assert.That(response.Success.Equals(sucess));
            if (!sucess)
            {
                Assert.That(response.UserError.Equals(errorMessage));
            }
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoices()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Status, "Enviada a crear,Creando factura,Error al crear" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.GetInvoices(dic);

            // assert
            var result = response.Response as List<InvoiceErrorDto>;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(10));
            Assert.That(result, Is.All.InstanceOf<InvoiceErrorDto>());
            Assert.That(response.Comments, Is.EqualTo(16));
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoicesSearchByInvoiceId()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.IdType, "invoice" },
                { ServiceConstants.Id, "8a8c6e" },
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.GetInvoices(dic);

            // assert
            var result = response.Response as List<InvoiceErrorDto>;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(response.Response, Is.All.InstanceOf<InvoiceErrorDto>());
            Assert.That(response.Comments, Is.EqualTo(1));
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoicesSearchByPedidoDxp()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.IdType, "pedidodxp" },
                { ServiceConstants.Id, "3f4dc9" },
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.GetInvoices(dic);

            // assert
            var result = response.Response as List<InvoiceErrorDto>;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(response.Response, Is.All.InstanceOf<InvoiceErrorDto>());
            Assert.That(response.Comments, Is.EqualTo(1));
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoicesSearchByPedidoSap()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.IdType, "pedidosap" },
                { ServiceConstants.Id, "177299, 177304" },
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.GetInvoices(dic);

            // assert
            var result = response.Response as List<InvoiceErrorDto>;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(response.Response, Is.All.InstanceOf<InvoiceErrorDto>());
            Assert.That(response.Comments, Is.EqualTo(2));
            Assert.That(result.Any(x => x.Id == "37d61da2-0c1d-4df0-b6a0-f740274e3893"), Is.True);
            Assert.That(result.Any(x => x.Id == "1ba6a7b1-f92a-4d6b-90b0-e3b83ee7ca40"), Is.True);
        }

        /// <summary>
        /// Filtro por Tipo de Factura - Genérica.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoices_WithTypeInvoiceFilter()
        {
            // Arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Status, "Error al crear,Creando factura" },
                { ServiceConstants.TypeInvoice, "Genérica" },
                { ServiceConstants.BillingType, "Parcial,Completa" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(
                this.invoiceDao,
                taskQueue.Object,
                serviceScopeFactoryMock.Object,
                logger.Object,
                sapAdapterServiceMock.Object,
                servicelayerServiceMock.Object,
                catalogServiceMock.Object,
                redisServiceMock.Object,
                mockUserService.Object,
                processPaymentServiceMock.Object);

            // Act
            var response = await localService.GetInvoices(dic);

            // Assert
            var result = response.Response as List<InvoiceErrorDto>;
            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.All.Matches<InvoiceErrorDto>(x => x.TypeInvoice == "Genérica"));
        }

        /// <summary>
        /// Filtro por Forma de Facturación.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoices_WithBillingTypeFilter()
        {
            // Arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Status, "Error al crear,Creando factura" },
                { ServiceConstants.TypeInvoice, "Genérica,No genérica" },
                { ServiceConstants.BillingType, "Parcial" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(
                this.invoiceDao,
                taskQueue.Object,
                serviceScopeFactoryMock.Object,
                logger.Object,
                sapAdapterServiceMock.Object,
                servicelayerServiceMock.Object,
                catalogServiceMock.Object,
                redisServiceMock.Object,
                mockUserService.Object,
                processPaymentServiceMock.Object);

            // Act
            var response = await localService.GetInvoices(dic);

            // Assert
            var result = response.Response as List<InvoiceErrorDto>;
            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.All.Matches<InvoiceErrorDto>(x => x.BillingType == "Parcial"));
        }

        /// <summary>
        /// Sin filtros nuevos - valores default.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetInvoices_WithoutFilters()
        {
            // Arrange
            var dic = new Dictionary<string, string>
    {
        { ServiceConstants.Offset, "0" },
        { ServiceConstants.Limit, "10" },
        { ServiceConstants.Status, "Error al crear,Creando factura" },
    };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(
                this.invoiceDao,
                taskQueue.Object,
                serviceScopeFactoryMock.Object,
                logger.Object,
                sapAdapterServiceMock.Object,
                servicelayerServiceMock.Object,
                catalogServiceMock.Object,
                redisServiceMock.Object,
                mockUserService.Object,
                processPaymentServiceMock.Object);

            // Act
            var response = await localService.GetInvoices(dic);

            // Assert
            var result = response.Response as List<InvoiceErrorDto>;
            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.All.InstanceOf<InvoiceErrorDto>());
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task UpdateManualChangeWithError()
        {
            // arrange
            var dic = new UpdateManualChangeDto() { Id = "INV-001" };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.UpdateManualChange(dic);

            // assert
            Assert.That(response.Code, Is.EqualTo(400));
            Assert.That(response.UserError, Is.Not.Null);
            Assert.That(response.UserError, Is.EqualTo(ServiceConstants.ErrorUpdateInvoice));
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task UpdateManualChange()
        {
            // arrange
            var dic = new UpdateManualChangeDto() { Id = "INV-003" };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.UpdateManualChange(dic);

            // assert
            Assert.That(response.Code, Is.EqualTo(200));
            Assert.That(response.UserError, Is.Null);
        }

        /// <summary>
        /// Method to verify carry out the order process.
        /// </summary>
        /// <returns> A <see cref="Task"/> representing the asynchronous unit test. </returns>
        [Test]
        public async Task GetInvoicesByRemissionId()
        {
            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var userServiceMock = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            this.userService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, userServiceMock.Object, processPaymentServiceMock.Object);

            var response = await this.userService.GetInvoicesByRemissionId(new List<int> { 1 });

            // Assert
            Assert.That(response.Success.Equals(true));
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAutoBillingByFacturaSap()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.IdType, "invoice" },
                { ServiceConstants.Id, "150833" },
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersResponse()));

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.GetAutoBillingAsync(dic);

            // assert
            var result = response.Response as List<AutoBillingRowDto>;
            var total = response.Comments.GetType().GetProperty("total").GetValue(response.Comments);

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(response.Response, Is.All.InstanceOf<AutoBillingRowDto>());
            Assert.That(response.Comments.GetType().GetProperty("total"), Is.Not.Null);
            Assert.That(total, Is.EqualTo(1));
        }

        /// <summary>
        /// Method Validate GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetUninvoicedSapOrders()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.IdPedidoDxpType, "4c0c4d72-6540-43c2-8511-baadeb1afcf4" },
            };

            var data = new List<OrderDetailModel> 
            {
                new OrderDetailModel { Id = 1, OrderId = 5001, ItemCode = "REVE 14", TransactionId = "4c0c4d72-6540-43c2-8511-baadeb1afcf4" },
                new OrderDetailModel { Id = 2, OrderId = 5002, ItemCode = "REVE 14", TransactionId = "4c0c4d72-6540-43c2-8511-baadeb1afcf4" },
                new OrderDetailModel { Id = 3, OrderId = 5010, ItemCode = "REVE 14", TransactionId = "4c0c4d72-6540-43c2-8511-baadeb1afcf4" },
                new OrderDetailModel { Id = 4, OrderId = 5011, ItemCode = "REVE 14", TransactionId = "4c0c4d72-6540-43c2-8511-baadeb1afcf4" },
            };

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();
            var mockUserService = new Mock<IUsersService>();
            var processPaymentServiceMock = new Mock<IProcessPaymentService>();

            processPaymentServiceMock
                .Setup(m => m.GetProcessPayments(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResultModel(data)));

            var localService = new InvoiceService(this.invoiceDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object, mockUserService.Object, processPaymentServiceMock.Object);

            // act
            var response = await localService.GetUninvoicedSapOrders(dic);

            // assert
            var result = response.Response as List<int>;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Does.Contain(5010));
            Assert.That(result, Does.Contain(5011));
        }
    }
}
