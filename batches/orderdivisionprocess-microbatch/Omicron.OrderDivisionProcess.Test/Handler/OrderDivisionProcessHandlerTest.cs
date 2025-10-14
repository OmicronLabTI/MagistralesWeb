// <summary>
// <copyright file="OrderDivisionProcessHandlerTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.OrderDivisionProcess.Test.Handler
{
    /// <summary>
    /// Class OrderDivisionProcessHandlerTest.
    /// </summary>
    [TestFixture]
    public class OrderDivisionProcessHandlerTest
    {
        private Mock<ILogger> loggerMock;
        private Mock<IRedisService> redisServiceMock;
        private Mock<IPedidosService> pedidosServiceMock;
        private Mock<IServiceScopeFactory> serviceScopeFactoryMock;
        private Mock<IServiceScope> serviceScopeMock;
        private Mock<IServiceProvider> serviceProviderMock;
        private Mock<IOptionsSnapshot<Settings>> settingsMock;
        private OrderDivisionProcessHandler handler;

        /// <summary>
        /// Setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.loggerMock = new Mock<ILogger>();
            this.redisServiceMock = new Mock<IRedisService>();
            this.pedidosServiceMock = new Mock<IPedidosService>();
            this.serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            this.serviceScopeMock = new Mock<IServiceScope>();
            this.serviceProviderMock = new Mock<IServiceProvider>();
            this.settingsMock = new Mock<IOptionsSnapshot<Settings>>();

            // scope para background POST (si tu handler lo usa)
            this.serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(this.serviceScopeMock.Object);
            this.serviceScopeMock.Setup(x => x.ServiceProvider).Returns(this.serviceProviderMock.Object);
            this.serviceProviderMock.Setup(x => x.GetService(typeof(IPedidosService)))
                               .Returns(this.pedidosServiceMock.Object);

            var settings = new Settings
            {
                BatchSize = 10,
                PedidosUrl = "https://test-pedidos.com",
                RedisUrl = "redis://localhost:6379",
                SeqUrl = "https://test-seq.com",
            };
            this.settingsMock.Setup(x => x.Value).Returns(settings);

            this.handler = new OrderDivisionProcessHandler(
                this.loggerMock.Object,
                this.pedidosServiceMock.Object,
                this.redisServiceMock.Object,
                this.settingsMock.Object,
                this.serviceScopeFactoryMock.Object); // quítalo si tu ctor no lo usa
        }

        /// <summary>
        /// Test Handle method with successful execution - tests full flow including private methods.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleSuccessfulExecutionProcessesOrdersAndDeletesRedisKeys()
        {
            // Cron no está corriendo
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob))
                            .ReturnsAsync(string.Empty);

            // GET devuelve 5
            this.pedidosServiceMock.Setup(x => x.GetAsync(BatchConstants.GetFailedDivisionOrdersEndpoint, It.IsAny<string>()))
                              .ReturnsAsync(new ResultDto { Success = true, Response = "5" });

            var divisionOrders = new List<ProductionOrderSeparationDetailLogModel>
            {
            new ProductionOrderSeparationDetailLogModel { Id = "sep-1" },
            new ProductionOrderSeparationDetailLogModel { Id = "sep-2" },
            };

            this.redisServiceMock.Setup(x => x.ReadListAsync<ProductionOrderSeparationDetailLogModel>(
                BatchConstants.DivisionOrdersToProcessKey, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(divisionOrders);

            await this.handler.Handle();

            this.redisServiceMock.Verify(
                x => x.WriteToRedis(
                BatchConstants.DivisionOrdersRetryCronjob,
                It.IsAny<string>(),
                BatchConstants.DefaultRedisValueTimeToLive), Times.Once);

            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersToProcessKey), Times.AtLeastOnce);

            this.pedidosServiceMock.Verify(x => x.GetAsync(BatchConstants.GetFailedDivisionOrdersEndpoint, It.IsAny<string>()), Times.Once);
            this.loggerMock.Verify(x => x.Information(BatchConstants.StartCronJobProcess, It.IsAny<string>()), Times.Once);
            this.loggerMock.Verify(x => x.Information(BatchConstants.EndCronJobProcess, It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test Handle method when job is already running - tests RetryDivisionOrder private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleJobAlreadyRunningLogsAndDoesNotDeleteKeys()
        {
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob))
                            .ReturnsAsync("running");

            await this.handler.Handle();

            this.loggerMock.Verify(x => x.Information(BatchConstants.CronJobProcessAlreadyRunning, It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(It.IsAny<string>()), Times.Never);
            this.loggerMock.Verify(x => x.Information(BatchConstants.EndCronJobProcess, It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Test Handle the service responds with an error - tests RetryDivisionOrder private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleGetFailedLogsErrorAndDeletesKeys()
        {
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob))
                            .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(BatchConstants.GetFailedDivisionOrdersEndpoint, It.IsAny<string>()))
                              .ReturnsAsync(new ResultDto { Success = false, Response = null });

            await this.handler.Handle();

            this.loggerMock.Verify(x => x.Error(BatchConstants.ErrorRetrievingDivisionOrders, It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersToProcessKey), Times.Once);
        }

        /// <summary>
        /// Test Handle there is nothing to process - tests RetryDivisionOrder private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleNoOrdersLogsInfoAndDeletesKeys()
        {
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob))
                            .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(BatchConstants.GetFailedDivisionOrdersEndpoint, It.IsAny<string>()))
                              .ReturnsAsync(new ResultDto { Success = true, Response = "0" });

            await this.handler.Handle();

            this.loggerMock.Verify(x => x.Information(BatchConstants.ThereAreNoDivisionOrdersToProcess, It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersToProcessKey), Times.Once);
        }

        /// <summary>
        /// Test Handle any unexpected failure - tests RetryDivisionOrder private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleExceptionThrownLogsErrorAndDeletesKeys()
        {
            var ex = new Exception("boom");
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob))
                            .ThrowsAsync(ex);

            await this.handler.Handle();

            this.loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersToProcessKey), Times.Once);
        }

        /// <summary>
        /// Test Handle tests RetryDivisionOrder private method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task HandleSendsBatchesInBackgroundLogsStartEnd()
        {
            this.redisServiceMock.Setup(x => x.GetRedisKey(BatchConstants.DivisionOrdersRetryCronjob))
                            .ReturnsAsync(string.Empty);

            this.pedidosServiceMock.Setup(x => x.GetAsync(BatchConstants.GetFailedDivisionOrdersEndpoint, It.IsAny<string>()))
                              .ReturnsAsync(new ResultDto { Success = true, Response = "5" });

            this.redisServiceMock.Setup(x => x.ReadListAsync<ProductionOrderSeparationDetailLogModel>(
                BatchConstants.DivisionOrdersToProcessKey, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<ProductionOrderSeparationDetailLogModel>
                {
                new ProductionOrderSeparationDetailLogModel { Id = "sep-1" },
                });

            await this.handler.Handle();
            await Task.Delay(100);

            this.loggerMock.Verify(x => x.Information(BatchConstants.StartDivisionOrdersAreSentForRetry, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.loggerMock.Verify(x => x.Information(BatchConstants.EndDivisionOrdersAreSentForRetry, It.IsAny<string>()), Times.Once);

            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersRetryCronjob), Times.Once);
            this.redisServiceMock.Verify(x => x.DeleteKey(BatchConstants.DivisionOrdersToProcessKey), Times.AtLeastOnce);
        }
    }
}
