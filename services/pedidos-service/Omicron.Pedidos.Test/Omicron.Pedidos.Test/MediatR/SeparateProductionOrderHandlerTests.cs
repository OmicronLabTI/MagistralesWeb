// <summary>
// <copyright file="SeparateProductionOrderHandlerTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    using Omicron.Pedidos.Services.OrderHistory;

    /// <summary>
    /// Tests for SeparateProductionOrderHandler.
    /// </summary>
    [TestFixture]
    public class SeparateProductionOrderHandlerTests : BaseTest
    {
        private Mock<ILogger> mockLogger;
        private Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue;
        private DatabaseContext context;
        private IPedidosDao pedidosDao;
        private Mock<IOrderHistoryHelper> mockOrderHistoryHelper;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserModelsForFinalizeProductionOrdersOnPostgresqlAsync());
            this.context.SaveChanges();
            this.pedidosDao = new PedidosDao(this.context);
            this.mockLogger = new Mock<ILogger>();
            this.mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            this.mockOrderHistoryHelper = new Mock<IOrderHistoryHelper>();
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <param name="productionOrderId">productionOrderId.</param>
        /// <param name="retryCount">Retry Count.</param>
        /// <param name="closeSapProductionSuccessfully">closeSapProductionSuccessfully.</param>
        /// <param name="exceptionMessage">exceptionMessage.</param>
        /// <returns>Test task.</returns>
        [Test]
        [TestCase(220001, 0, true, null)] // Correcto
        [TestCase(220002, 0, true, null)] // Orden Cancelada en postgres.
        [TestCase(211111, 0, true, null)] // No existe orden.
        [TestCase(220003, 2, false, "Error al cerrar la orden de fabricación en SAP.")] // Error en sap.
        public async Task HandleCancelProductionOrder(int productionOrderId, int retryCount, bool closeSapProductionSuccessfully, string exceptionMessage)
        {
            // Arrange
            var command = new CancelProductionOrderCommand(
                productionOrderId,
                5,
                "b54659e3-d334-42c4-b91a-f59f5a463125",
                "axity1",
                "xxx-xxx-xxx",
                123,
                10);

            var sapResult = this.GetResultModelCompl(new object(), closeSapProductionSuccessfully, string.Empty, exceptionMessage);
            var sapResultCreateDetail = this.GetResultModelCompl(909090, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .SetupSequence(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.FromResult(sapResult))
               .Returns(Task.FromResult(sapResultCreateDetail));

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var handler = new CancelProductionOrderHandler(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                this.mockBackgroundTaskQueue.Object,
                this.mockLogger.Object,
                mockRedisService.Object,
                this.mockOrderHistoryHelper.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            if (productionOrderId == 220001 || productionOrderId == 220002)
            {
                Assert.That(result, Is.True);
            }
            else
            {
                Assert.That(result, Is.False);
            }
        }
    }
}
