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
        private IInvoiceDao usersDao;
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
            this.context.SaveChanges();

            var taskQueue = new Mock<IBackgroundTaskQueue>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var sapAdapterServiceMock = new Mock<ISapAdapter>();
            var servicelayerServiceMock = new Mock<ISapServiceLayerAdapterService>();
            var logger = new Mock<Serilog.ILogger>();
            var catalogServiceMock = new Mock<ICatalogsService>();
            var redisServiceMock = new Mock<IRedisService>();

            this.usersDao = new InvoiceDao(this.context);
            this.userService = new InvoiceService(this.usersDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object);
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

            this.userService = new InvoiceService(this.usersDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object);
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

            var response = await this.userService.RegisterInvoice(request);

            // Assert
            Assert.That(response.Code.Equals(200));
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

            this.userService = new InvoiceService(this.usersDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object);
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

            this.userService = new InvoiceService(this.usersDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object);
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

            this.userService = new InvoiceService(this.usersDao, taskQueue.Object, serviceScopeFactoryMock.Object, logger.Object, sapAdapterServiceMock.Object, servicelayerServiceMock.Object, catalogServiceMock.Object, redisServiceMock.Object);

            var response = await this.userService.GetInvoicesByRemissionId(new List<int> { 1 });

            // Assert
            Assert.That(response.Success.Equals(true));
        }
    }
}
