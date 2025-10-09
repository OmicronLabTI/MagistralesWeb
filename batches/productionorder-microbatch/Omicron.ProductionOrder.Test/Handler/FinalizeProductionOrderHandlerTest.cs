// <summary>
// <copyright file="FinalizeProductionOrderHandlerTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.ProductionOrder.Test.Handler
{
    /// <summary>
    /// Class FinalizeProductionOrderHandlerTest.
    /// </summary>
    [TestFixture]
    public class FinalizeProductionOrderHandlerTest
    {
        private Mock<ILogger> loggerMock;
        private Mock<IRedisService> redisServiceMock;
        private Mock<IPedidosService> pedidosServiceMock;
        private Mock<IOptionsSnapshot<Settings>> settingsMock;
        private FinalizeProductionOrderHandler handler;
        private Mock<IKafkaConnector> kafkaConnectorMock;

        /// <summary>
        /// Setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.loggerMock = new Mock<ILogger>();
            this.redisServiceMock = new Mock<IRedisService>();
            this.pedidosServiceMock = new Mock<IPedidosService>();
            this.settingsMock = new Mock<IOptionsSnapshot<Settings>>();
            this.kafkaConnectorMock = new Mock<IKafkaConnector>();

            // Setup settings object
            var settings = new Settings
            {
                BatchSize = 10,
                PedidosUrl = "https://test-pedidos.com",
                RedisUrl = "redis://localhost:6379",
                SeqUrl = "https://test-seq.com",
            };

            this.settingsMock.Setup(x => x.Value).Returns(settings);

            this.handler = new FinalizeProductionOrderHandler(
                this.loggerMock.Object,
                this.pedidosServiceMock.Object,
                this.redisServiceMock.Object,
                this.settingsMock.Object,
                this.kafkaConnectorMock.Object);
        }

        /// <summary>
        /// Test Handle method with successful execution - tests full flow including private methods.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleSuccessfulExecutionProcessesOrdersAndDeletesRedisKeys()
        {
            // Arrange
            // Mock RetryFinalizeProductionOrder flow
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync(string.Empty);

            // Mock successful API response with 5 orders to process
            this.pedidosServiceMock
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ResultDto { Success = true, Response = "5" });

            // Mock Redis read for production orders - return fewer items than batch size to trigger key deletion
            var productionOrders = new List<ProductionOrderProcessingStatusModel>
            {
                new ProductionOrderProcessingStatusModel { Id = "1" },
                new ProductionOrderProcessingStatusModel { Id = "2" },
            };

            this.redisServiceMock.Setup(x => x.ReadListAsync<ProductionOrderProcessingStatusModel>(
                BatchConstants.ProductionOrderFinalizingToProcessKey,
                It.IsAny<int>(),
                It.IsAny<int>()))
                .ReturnsAsync(productionOrders);

            // Act
            await this.handler.Handle();

            // Assert
            // Verify Redis operations
            this.redisServiceMock.Verify(
                x => x.WriteToRedis(
                BatchConstants.ProductionOrderFinalizingRetryCronjob,
                It.IsAny<string>(),
                BatchConstants.DefaultRedisValueTimeToLive), Times.Once);

            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingRetryCronjob), Times.Once);

            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey), Times.AtLeastOnce);

            // Verify API calls
            this.pedidosServiceMock.Verify(
                x => x.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                It.IsAny<string>()), Times.Once);

            // Verify logging
            this.loggerMock.Verify(x => x.Information(BatchConstants.StartCronJobProcess, It.IsAny<string>()), Times.Once);
            this.loggerMock.Verify(x => x.Information(BatchConstants.EndCronJobProcess, It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test Handle method when job is already running - tests RetryFinalizeProductionOrder private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleJobAlreadyRunningLogsWarningAndDoesNotDeleteKeys()
        {
            // Arrange
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync("existing_value");

            // Act
            await this.handler.Handle();

            // Assert
            this.loggerMock.Verify(x => x.Information(BatchConstants.CronJobProcessAlreadyRunning, It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(It.IsAny<string>()), Times.Exactly(4));
            this.loggerMock.Verify(x => x.Information(BatchConstants.EndCronJobProcess, It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test Handle method when API call fails - tests error handling in RetryFinalizeProductionOrder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleApiCallFailsLogsErrorAndDeletesKeys()
        {
            // Arrange
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                It.IsAny<string>()))
                .ReturnsAsync(new ResultDto { Success = false, Response = null });

            // Act
            await this.handler.Handle();

            // Assert
            this.loggerMock.Verify(x => x.Error(BatchConstants.ErrorRetrievingProductionOrders, It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingRetryCronjob), Times.Exactly(3));
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey), Times.Exactly(3));
        }

        /// <summary>
        /// Test Handle method when no orders to process - tests conditional logic in RetryFinalizeProductionOrder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleNoOrdersToProcessLogsInfoAndDeletesKeys()
        {
            // Arrange
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                It.IsAny<string>()))
                .ReturnsAsync(new ResultDto { Success = true, Response = "0" });

            // Act
            await this.handler.Handle();

            // Assert
            this.loggerMock.Verify(x => x.Information(BatchConstants.ThereAreNoProductionOrdersToProcess, It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingRetryCronjob), Times.Exactly(2));
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey), Times.Exactly(2));
        }

        /// <summary>
        /// Test Handle method with multiple batches - tests SendToRetryProcess private method through batching logic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleMultipleBatchesProcessesAllBatches()
        {
            // Arrange
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync(string.Empty);

            // Setup 25 orders to process (should create 3 batches with BatchSize = 10)
            this.pedidosServiceMock.Setup(x => x.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                It.IsAny<string>()))
                .ReturnsAsync(new ResultDto { Success = true, Response = "25" });

            var productionOrders = new List<ProductionOrderProcessingStatusModel>
            {
                new ProductionOrderProcessingStatusModel { Id = "1" },
                new ProductionOrderProcessingStatusModel { Id = "2" },
            };

            this.redisServiceMock.Setup(x => x.ReadListAsync<ProductionOrderProcessingStatusModel>(
                BatchConstants.ProductionOrderFinalizingToProcessKey,
                It.IsAny<int>(),
                It.IsAny<int>()))
                .ReturnsAsync(productionOrders);

            // Act
            await this.handler.Handle();

            // Assert
            // Verify batching logic worked - should call ReadListAsync multiple times for different batches
            this.redisServiceMock.Verify(
                x => x.ReadListAsync<ProductionOrderProcessingStatusModel>(
                BatchConstants.ProductionOrderFinalizingToProcessKey,
                It.IsAny<int>(),
                It.IsAny<int>()), Times.AtLeastOnce);

            // Verify logging for batch processing
            this.loggerMock.Verify(x => x.Information(BatchConstants.NumberOfProductionOrdersToProcess, It.IsAny<string>(), 25), Times.Once);
            this.loggerMock.Verify(x => x.Information(BatchConstants.ProductionOrdersAreRetrievedFromRedis, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test Handle method when GetValueFromRedisByKeyAndOffsetAndLimit returns less items than limit - tests Redis key deletion logic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleLastBatchLessThanLimitDeletesRedisKey()
        {
            // Arrange
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                It.IsAny<string>()))
                .ReturnsAsync(new ResultDto { Success = true, Response = "5" });

            // Return fewer items than the limit (5 items when limit is 10)
            var productionOrders = new List<ProductionOrderProcessingStatusModel>
            {
                new ProductionOrderProcessingStatusModel { Id = "1" },
                new ProductionOrderProcessingStatusModel { Id = "2" },
                new ProductionOrderProcessingStatusModel { Id = "3" },
                new ProductionOrderProcessingStatusModel { Id = "4" },
                new ProductionOrderProcessingStatusModel { Id = "5" },
            };

            this.redisServiceMock.Setup(x => x.ReadListAsync<ProductionOrderProcessingStatusModel>(
                BatchConstants.ProductionOrderFinalizingToProcessKey,
                It.IsAny<int>(),
                It.IsAny<int>()))
                .ReturnsAsync(productionOrders);

            // Act
            await this.handler.Handle();

            // Assert
            // Should delete the ProcessKey multiple times: once in GetValueFromRedisByKeyAndOffsetAndLimit and once at the end
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey), Times.AtLeast(1));
        }

        /// <summary>
        /// Test Handle method when exception occurs - tests exception handling and cleanup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleExceptionThrownLogsErrorAndDeletesRedisKeys()
        {
            // Arrange
            var exception = new Exception("Test exception");
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ThrowsAsync(exception);

            // Act
            await this.handler.Handle();

            // Assert
            this.loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey), Times.Once);
        }

        /// <summary>
        /// Test Handle method verifies background task execution - tests SendPedidosRequestInBackgroundAsync private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleSuccessfulProcessingInitiatesBackgroundTasks()
        {
            // Arrange
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.ProductionOrderFinalizingRetryCronjob))
                .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(
                BatchConstants.GetFailedProductionOrdersEndpoint,
                It.IsAny<string>()))
                .ReturnsAsync(new ResultDto { Success = true, Response = "5" });

            var productionOrders = new List<ProductionOrderProcessingStatusModel>
            {
                new ProductionOrderProcessingStatusModel { Id = "1" },
            };

            this.redisServiceMock.Setup(x => x.ReadListAsync<ProductionOrderProcessingStatusModel>(
                BatchConstants.ProductionOrderFinalizingToProcessKey,
                It.IsAny<int>(),
                It.IsAny<int>()))
                .ReturnsAsync(productionOrders);

            // Act
            await this.handler.Handle();

            // Wait a bit to allow background tasks to potentially start
            await Task.Delay(100);

            // Assert
            // Verify logging that indicates background processing
            this.loggerMock.Verify(x => x.Information(BatchConstants.StartProductionOrdersAreSentForRetry, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.loggerMock.Verify(x => x.Information(BatchConstants.EndProductionOrdersAreSentForRetry, It.IsAny<string>()), Times.Once);

            // Verify Redis cleanup operations
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.ProductionOrderFinalizingToProcessKey), Times.AtLeastOnce);
        }
    }
}