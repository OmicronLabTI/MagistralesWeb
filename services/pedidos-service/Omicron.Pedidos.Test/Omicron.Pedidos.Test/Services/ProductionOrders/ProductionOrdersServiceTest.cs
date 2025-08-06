// <summary>
// <copyright file="ProductionOrdersServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services.ProductionOrders
{
    using Omicron.Pedidos.Services.ProductionOrders.Impl;
    using StackExchange.Redis;
    using ZXing;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ProductionOrdersServiceTest : BaseTest
    {
        private IPedidosDao pedidosDao;

        private Mock<ILogger> logger;

        private Mock<IKafkaConnector> mockKafkaConnector;

        private Mock<ISapAdapter> sapAdapter;

        private Mock<IUsersService> userService;

        private Mock<ISapFileService> sapFileService;

        private DatabaseContext context;

        private IMapper mapper;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.ProductionOrderProcessingStatusModel.AddRange(this.GetProductionOrderProcessingStatusModel());
            this.context.UserOrderModel.AddRange(this.GetUserModelsForFinalizeProductionOrdersOnPostgresqlAsync());
            this.context.SaveChanges();
            this.pedidosDao = new PedidosDao(this.context);
            this.logger = new Mock<ILogger>();
            this.sapAdapter = new Mock<ISapAdapter>();
            this.userService = new Mock<IUsersService>();
            this.sapFileService = new Mock<ISapFileService>();
            this.mockKafkaConnector = new Mock<IKafkaConnector>();
            this.mockKafkaConnector
                .Setup(m => m.PushMessage(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
        }

        /// <summary>
        /// FinalizeProductionOrdersAsync.
        /// </summary>
        /// <param name="isServiceLayerSuccess">isServiceLayerSuccess.</param>
        /// <param name="hasResponseServiceLayer">hasResponseServiceLayer.</param>
        /// <returns>Test.</returns>
        [Test]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public async Task FinalizeProductionOrdersAsync(bool isServiceLayerSuccess, bool hasResponseServiceLayer)
        {
            var productionOrdersToFinalize = new List<FinalizeProductionOrderModel>
            {
                new FinalizeProductionOrderModel { ProductionOrderId = 100001, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // No existe en redis, pero si en la base
                new FinalizeProductionOrderModel { ProductionOrderId = 100002, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // Ya existe en redis
                new FinalizeProductionOrderModel { ProductionOrderId = 100003, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // No existe la orden de fabricación en SAP
                new FinalizeProductionOrderModel { ProductionOrderId = 100004, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // Orden no liberada
                new FinalizeProductionOrderModel { ProductionOrderId = 100005, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // Orden cerrada
                new FinalizeProductionOrderModel { ProductionOrderId = 100006, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // OK
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService
                .Setup(mr => mr.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetRedisFinalizeProductionOrderString(true)));
            mockRedisService.Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            var resultMessages = new List<ValidationsToFinalizeProductionOrdersResultModel>();
            if (hasResponseServiceLayer)
            {
                resultMessages = new List<ValidationsToFinalizeProductionOrdersResultModel>
                {
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100003, ErrorMessage = "La orden de produción 100003 no existe." },
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100004, ErrorMessage = "La orden de producción 100004 no esta liberada." },
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100005, ErrorMessage = "La orden de producción 100005 está cerrada." },
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100006, ErrorMessage = string.Empty },
                };
            }

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
                .Setup(msl => msl.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompl(resultMessages, isServiceLayerSuccess)));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                this.sapAdapter.Object,
                this.userService.Object,
                this.sapFileService.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.FinalizeProductionOrdersAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Code.Equals(200));
        }

        /// <summary>
        /// FinalizeProductionOrdersAsync.
        /// </summary>
        /// <param name="isServiceLayerSuccess">isServiceLayerSuccess.</param>
        /// <param name="hasResponseServiceLayer">hasResponseServiceLayer.</param>
        /// <returns>Test.</returns>
        [Test]
        public async Task FinalizeProductionOrdersAsyncError()
        {
            var productionOrdersToFinalize = new List<FinalizeProductionOrderModel>
            {
                new FinalizeProductionOrderModel { ProductionOrderId = 100007, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SalesOrders" }, // OK
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
                .Setup(msl => msl.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Connection Refused."));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                this.sapAdapter.Object,
                this.userService.Object,
                this.sapFileService.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.FinalizeProductionOrdersAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success, Is.False);
            Assert.That(response.Response, Is.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError.Equals("An unexpected error occurred."));
            Assert.That(response.Code.Equals(500));
        }

        /// <summary>
        /// FinalizeProductionOrdersOnSapAsync.
        /// </summary>
        /// <param name="isServiceLayerSuccess">isServiceLayerSuccess.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Test.</returns>
        [Test]
        [TestCase(false, "")]
        [TestCase(true, "")]
        [TestCase(true, "No se ha podido crear recepción de producción para la orden de fabricación {0}.")]
        public async Task FinalizeProductionOrdersOnSapAsync(bool isServiceLayerSuccess, string errorMessage)
        {
            var payload = new FinalizeProductionOrderPayload
            {
                FinalizeProductionOrder = new FinalizeProductionOrderModel
                {
                    ProductionOrderId = 100001,
                    UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e",
                    SourceProcess = "SalesOrders",
                    Batches = new List<BatchesConfigurationModel>(),
                },
            };

            var productionOrdersToFinalize = new ProductionOrderProcessingStatusModel
            {
                Id = "9e7ea1ba-5950-4a94-a34e-5b7a5db112a4",
                ProductionOrderId = 100001,
                LastStep = "Primary Validations",
                CreatedAt = DateTime.Now.AddMinutes(-5),
                ErrorMessage = null,
                LastUpdated = DateTime.Now.AddMinutes(-5),
                Payload = JsonConvert.SerializeObject(payload),
                Status = "In Progress",
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var resultMessages = new List<ValidationsToFinalizeProductionOrdersResultModel>
                {
                    new ValidationsToFinalizeProductionOrdersResultModel
                    {
                        ProductionOrderId = 100001,
                        ErrorMessage = string.Format(errorMessage, 100001),
                    },
                };

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
                .Setup(msl => msl.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompl(resultMessages, isServiceLayerSuccess)));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                this.sapAdapter.Object,
                this.userService.Object,
                this.sapFileService.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.FinalizeProductionOrdersOnSapAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);

            if (isServiceLayerSuccess && string.IsNullOrEmpty(errorMessage))
            {
                Assert.That(response.Success, Is.True);
                Assert.That(response.Code.Equals((int)HttpStatusCode.OK));
            }
            else
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Code.Equals((int)HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// FinalizeProductionOrdersOnSapAsync.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task FinalizeProductionOrdersOnSapAsyncWithError()
        {
            var payload = new FinalizeProductionOrderPayload
            {
                FinalizeProductionOrder = new FinalizeProductionOrderModel
                {
                    ProductionOrderId = 100001,
                    UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e",
                    SourceProcess = "SalesOrders",
                    Batches = new List<BatchesConfigurationModel>(),
                },
            };

            var productionOrdersToFinalize = new ProductionOrderProcessingStatusModel
            {
                Id = "9e7ea1ba-5950-4a94-a34e-5b7a5db112a4",
                ProductionOrderId = 100001,
                LastStep = "Primary Validations",
                CreatedAt = DateTime.Now.AddMinutes(-10),
                ErrorMessage = null,
                LastUpdated = DateTime.Now,
                Payload = JsonConvert.SerializeObject(payload),
                Status = "In Progress",
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
                .Setup(msl => msl.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Connection Refused."));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                this.sapAdapter.Object,
                this.userService.Object,
                this.sapFileService.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.FinalizeProductionOrdersOnSapAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Success, Is.False);
            Assert.That(response.Code.Equals((int)HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// FinalizeProductionOrdersOnPostgresqlAsyncTest.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task FinalizeProductionOrdersOnPostgresqlAsyncTest()
        {
            var payload = new FinalizeProductionOrderPayload
            {
                FinalizeProductionOrder = new FinalizeProductionOrderModel
                {
                    ProductionOrderId = 223580,
                    UserId = "7ac2db83-3c31-4042-9d1b-5531753694b4",
                    SourceProcess = "SalesOrders",
                    Batches = new List<BatchesConfigurationModel>(),
                },
            };

            var productionOrdersToFinalize = new ProductionOrderProcessingStatusModel
            {
                Id = "778e6a6a-fa1d-4767-95fa-47f3c0cb2377",
                ProductionOrderId = 223580,
                LastStep = "Update UsersOrders in postgres",
                CreatedAt = DateTime.Now.AddMinutes(-10),
                ErrorMessage = null,
                LastUpdated = DateTime.Now,
                Payload = JsonConvert.SerializeObject(payload),
                Status = "In Progress",
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                mockSapAdapter.Object,
                this.userService.Object,
                this.sapFileService.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.FinalizeProductionOrdersOnPostgresqlAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response.Success, Is.True);
            Assert.That(response.Code.Equals((int)HttpStatusCode.OK));
        }

        /// <summary>
        /// FinalizeProductionOrdersOnPostgresqlAsyncWithError.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task FinalizeProductionOrdersOnPostgresqlAsyncWithError()
        {
            var payload = new FinalizeProductionOrderPayload
            {
                FinalizeProductionOrder = new FinalizeProductionOrderModel
                {
                    ProductionOrderId = 223580,
                    UserId = "7ac2db83-3c31-4042-9d1b-5531753694b4",
                    SourceProcess = "SalesOrders",
                    Batches = new List<BatchesConfigurationModel>(),
                },
            };

            var productionOrdersToFinalize = new ProductionOrderProcessingStatusModel
            {
                Id = "778e6a6a-fa1d-4767-95fa-47f3c0cb2377",
                ProductionOrderId = 223580,
                LastStep = "Update UsersOrders in postgres",
                CreatedAt = DateTime.Now.AddMinutes(-10),
                ErrorMessage = null,
                LastUpdated = DateTime.Now,
                Payload = JsonConvert.SerializeObject(payload),
                Status = "In Progress",
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            // The error is in the sapAdapter response.
            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                this.sapAdapter.Object,
                this.userService.Object,
                this.sapFileService.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.FinalizeProductionOrdersOnPostgresqlAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Success, Is.False);
            Assert.That(response.Code.Equals((int)HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// FinalizeProductionOrdersOnPostgresqlAsyncTest.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task ProductionOrderPdfGenerationAsync()
        {
            var payload = new FinalizeProductionOrderPayload
            {
                FinalizeProductionOrder = new FinalizeProductionOrderModel
                {
                    ProductionOrderId = 224896,
                    UserId = "7ac2db83-3c31-4042-9d1b-5531753694b4",
                    SourceProcess = "SalesOrders",
                    Batches = new List<BatchesConfigurationModel>(),
                },
            };

            var productionOrdersToFinalize = new ProductionOrderProcessingStatusModel
            {
                Id = "741baf58-b87a-4235-b724-35f966229fd8",
                ProductionOrderId = 224896,
                LastStep = "Update UsersOrders in postgres",
                CreatedAt = DateTime.Now.AddMinutes(-10),
                ErrorMessage = null,
                LastUpdated = DateTime.Now,
                Payload = JsonConvert.SerializeObject(payload),
                Status = "In Progress",
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFabricacionOrderModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                mockSapAdapter.Object,
                mockUsers.Object,
                mockSapFile.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.ProductionOrderPdfGenerationAsync(productionOrdersToFinalize);

            // Assert
            Assert.That(response.Success, Is.True);
            Assert.That(response.Code.Equals((int)HttpStatusCode.OK));
        }

        /// <summary>
        /// GetFailedProductionOrders.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task GetFailedProductionOrders()
        {
            var mockRedisService = new Mock<IRedisService>();

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();

            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();

            var mockDao = new Mock<IPedidosDao>();
            mockDao
                .Setup(x => x.GetAllProductionOrderProcessingStatusByStatus(It.IsAny<List<string>>()))
                .Returns(Task.FromResult(this.GetProductionOrderProcessingStatusModelForGetFailedProductionOrders()));

            var mockProductionOrdersService = new ProductionOrdersService(
                mockDao.Object,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                mockSapAdapter.Object,
                mockUsers.Object,
                mockSapFile.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.GetFailedProductionOrders();

            // Assert
            Assert.That(response.Success, Is.True);
            Assert.That(response.Code.Equals((int)HttpStatusCode.OK));
            Assert.That(response.Response, Is.EqualTo(3));
        }

        /// <summary>
        /// RetryFailedProductionOrderFinalization_Primary_Validations.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task RetryFailedProductionOrderFinalization_Primary_Validations()
        {
            var payload = new ProductionOrderProcessingStatusDto
            {
                Id = "9b7b6752-8f91-4c1b-9eda-f41a3e30fdef",
                ProductionOrderId = 100003,
                LastStep = "Primary Validations",
                Status = "Failded",
                ErrorMessage = "error",
                Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"76162c93-7c22-49b2-b3aa-dc3a355f48e1\",\"ProductionOrderId\":100003,\"SourceProcess\":null,\"Batches\":null}}",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            var request = new RetryFailedProductionOrderFinalizationDto
            {
                BatchProcessId = "0436570b-2437-47f7-82ea-e699db8e45a3",
                ProductionOrderProcessingPayload = new List<ProductionOrderProcessingStatusDto> { payload },
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService
                .Setup(mr => mr.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetRedisFinalizeProductionOrderString(true)));
            mockRedisService.Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            var resultMessages = new List<ValidationsToFinalizeProductionOrdersResultModel>
                {
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100003, ErrorMessage = "La orden de produción 100003 no existe." },
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100004, ErrorMessage = "La orden de producción 100004 no esta liberada." },
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100005, ErrorMessage = "La orden de producción 100005 está cerrada." },
                    new ValidationsToFinalizeProductionOrdersResultModel { ProductionOrderId = 100006, ErrorMessage = string.Empty },
                };

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
                .Setup(msl => msl.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompl(resultMessages, true)));

            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                mockSapAdapter.Object,
                mockUsers.Object,
                mockSapFile.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.RetryFailedProductionOrderFinalization(request);

            // Assert
            Assert.That(response.Success, Is.True);
            Assert.That(response.Code.Equals((int)HttpStatusCode.OK));
        }

        /// <summary>
        /// RetryFailedProductionOrderFinalization_Primary_Validations.
        /// </summary>
        /// <returns>Test.</returns>
        [Test]
        public async Task RetryFailedProductionOrderFinalization_PdfGeneration()
        {
            var payload = new ProductionOrderProcessingStatusDto
            {
                Id = "9b7b6752-8f91-4c1b-9eda-f41a3e30fdef",
                ProductionOrderId = 100003,
                LastStep = "Successfully Closed In Postgresql",
                Status = "Failded",
                ErrorMessage = "error",
                Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"76162c93-7c22-49b2-b3aa-dc3a355f48e1\",\"ProductionOrderId\":100003,\"SourceProcess\":null,\"Batches\":null}}",
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            var request = new RetryFailedProductionOrderFinalizationDto
            {
                BatchProcessId = "0436570b-2437-47f7-82ea-e699db8e45a3",
                ProductionOrderProcessingPayload = new List<ProductionOrderProcessingStatusDto> { payload },
            };

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFabricacionOrderModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockProductionOrdersService = new ProductionOrdersService(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                mockRedisService.Object,
                this.mockKafkaConnector.Object,
                mockSapAdapter.Object,
                mockUsers.Object,
                mockSapFile.Object,
                this.logger.Object,
                this.mapper);

            var response = await mockProductionOrdersService.RetryFailedProductionOrderFinalization(request);

            // Assert
            Assert.That(response.Success, Is.True);
            Assert.That(response.Code.Equals((int)HttpStatusCode.OK));
        }
    }
}
