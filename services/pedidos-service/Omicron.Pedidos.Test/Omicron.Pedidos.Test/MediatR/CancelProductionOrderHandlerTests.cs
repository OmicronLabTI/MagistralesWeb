// <summary>
// <copyright file="CancelProductionOrderHandlerTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.OrderHistory;

    /// <summary>
    /// Tests for SeparateProductionOrderHandler.
    /// </summary>
    [TestFixture]
    public class CancelProductionOrderHandlerTests : BaseTest
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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
        /// <param name="closeSapProductionSuccessfully">closeSapProductionSuccessfully.</param>
        /// <param name="exceptionMessage">exceptionMessage.</param>
        /// <returns>Test task.</returns>
        [Test]
        [TestCase(220001, true, null)] // Correcto
        [TestCase(220002, true, null)] // Orden Cancelada en postgres.
        [TestCase(211111, true, null)] // No existe orden.
        [TestCase(220003, false, "Error al cerrar la orden de fabricación en SAP.")] // Error en sap.
        public async Task HandleCancelProductionOrder(int productionOrderId, bool closeSapProductionSuccessfully, string exceptionMessage)
        {
            // Arrange
            this.mockBackgroundTaskQueue.Invocations.Clear();
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

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task HandleCancelProductionOrderFullFlow()
        {
            // Arrange
            var command = new CancelProductionOrderCommand(
                220004,
                5,
                "d7e087e1-9bb8-4198-a617-7a04423f13e8",
                "axity1",
                "xxx-xxx-xxx",
                123,
                10);

            var sapResponde = new CancelProductionOrderDto
            {
                LastStep = "CancelSAP",
                ErrorMessage = string.Empty,
            };

            var sapResultCreateDetail = this.GetResultModelCompl(sapResponde, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Assert.That(result, Is.True);
            Assert.That(command.LastStep, Is.EqualTo("SaveHistory"));
            this.mockLogger.Verify(l => l.Information(It.Is<string>(msg => msg.Contains("End Proccess Successfuly")), It.Is<string>(logBase => logBase.Contains("220004"))), Times.Once);
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task HandleCancelProductionOrderFullFlowWithError()
        {
            // Arrange
            var command = new CancelProductionOrderCommand(
                220005,
                5,
                "5de526c9-5f7c-48dc-a5de-016dd7575199",
                "axity1",
                "xxx-xxx-xxx",
                123,
                10);

            var sapResponde = new CancelProductionOrderDto
            {
                LastStep = "UpdateCancelParentOrderProcess",
                ErrorMessage = "Error al cancelar",
            };

            var sapResultCreateDetail = this.GetResultModelCompl(sapResponde, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Assert.That(command.LastStep, Is.EqualTo("UpdateCancelParentOrderProcess"));
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task HandleCancelProductionOrderCancelSapStep()
        {
            // Arrange
            var command = new CancelProductionOrderCommand(
                220006,
                5,
                "2d93cca4-5d26-4faf-9c74-1971ecf1c7ba",
                "axity1",
                "xxx-xxx-xxx",
                123,
                10);

            command.LastStep = "CancelSAP";

            var sapResponde = new CancelProductionOrderDto
            {
                LastStep = "CancelSAP",
                ErrorMessage = string.Empty,
            };

            var sapResultCreateDetail = this.GetResultModelCompl(sapResponde, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Assert.That(result, Is.True);
            Assert.That(command.LastStep, Is.EqualTo("SaveHistory"));
            this.mockLogger.Verify(l => l.Information(It.Is<string>(msg => msg.Contains("End Proccess Successfuly")), It.Is<string>(logBase => logBase.Contains("220006"))), Times.Once);
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task HandleCancelProductionOrderCancelPostgresStep()
        {
            // Arrange
            var command = new CancelProductionOrderCommand(
                220007,
                5,
                "2d93cca4-5d26-4faf-9c74-1971ecf1c7ba",
                "axity1",
                "xxx-xxx-xxx",
                123,
                10);

            command.LastStep = "CancelPostgres";

            var sapResponde = new CancelProductionOrderDto
            {
                LastStep = "CancelSAP",
                ErrorMessage = string.Empty,
            };

            var sapResultCreateDetail = this.GetResultModelCompl(sapResponde, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Assert.That(result, Is.True);
            Assert.That(command.LastStep, Is.EqualTo("SaveHistory"));
            this.mockLogger.Verify(l => l.Information(It.Is<string>(msg => msg.Contains("End Proccess Successfuly")), It.Is<string>(logBase => logBase.Contains("220007"))), Times.Once);
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task HandleCancelProductionOrder_NotDivisionInPending()
        {
            // Arrange
            var parent = new UserOrderModel
            {
                Id = 99901,
                Userid = "axity1",
                Salesorderid = "SO-TEST-1",
                Productionorderid = "330001",
                Status = ServiceConstants.Cancelled,
                StatusWorkParent = ServiceConstants.Pendiente,
                ReassignmentDate = null,
                TypeOrder = "FAB",
                Quantity = 1,
                StatusForTecnic = ServiceConstants.Asignado,
            };
            this.context.UserOrderModel.Add(parent);
            this.context.SaveChanges();

            var command = new CancelProductionOrderCommand(
                productionOrderId: 330001,
                pieces: 5,
                separationId: Guid.NewGuid().ToString(),
                userId: "axity1",
                dxpOrder: "xxx-xxx-xxx",
                sapOrder: 123,
                totalPieces: 10);

            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            var mockRedisService = new Mock<IRedisService>();

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
            Assert.That(result, Is.False);
            this.mockBackgroundTaskQueue.Verify(
                q => q.QueueBackgroundWorkItem(It.IsAny<Func<IServiceProvider, CancellationToken, Task>>()),
                Times.Never);
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <param name="hasReassignmentDate">exceptionMessage.</param>
        /// <param name="expectedWorkStatus">expectedWorkStatus.</param>
        /// <returns>Test task.</returns>
        [Test]
        [TestCase(false, ServiceConstants.Proceso)] // Proceso
        [TestCase(true, ServiceConstants.Reasignado)] // Reasignado
        public async Task Handle_StatusWorkParent_BasedOnReassignmentDate(bool hasReassignmentDate, string expectedWorkStatus)
        {
            // Arrange
            var parentOrderId = 330100 + (hasReassignmentDate ? 1 : 0);
            var productionOrderId = parentOrderId.ToString();

            this.context.UserOrderModel.Add(new UserOrderModel
            {
                Id = 99001 + parentOrderId,
                Userid = "axity1",
                Salesorderid = "SO-X",
                Productionorderid = productionOrderId,
                Status = ServiceConstants.Cancelled,
                StatusWorkParent = null,
                ReassignmentDate = hasReassignmentDate ? DateTime.UtcNow : (DateTime?)null,
                TypeOrder = "FAB",
                Quantity = 1,
                StatusForTecnic = ServiceConstants.Asignado,
            });

            // Marca como padre
            this.context.Set<ProductionOrderSeparationModel>().Add(new ProductionOrderSeparationModel
            {
                OrderId = parentOrderId,
                ProductionDetailCount = 0,
                TotalPieces = 10,
                AvailablePieces = 10,
                Status = "Init",
            });

            await this.context.SaveChangesAsync();

            var command = new CancelProductionOrderCommand(
                productionOrderId: parentOrderId,
                pieces: 5,
                separationId: Guid.NewGuid().ToString(),
                userId: "axity1",
                dxpOrder: "xxx-xxx-xxx",
                sapOrder: 123,
                totalPieces: 10);

            var sapOk1 = this.GetResultModelCompl(new object(), true, string.Empty, null);
            var sapOk2 = this.GetResultModelCompl(909090, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
                .SetupSequence(s => s.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(sapOk1))
                .Returns(Task.FromResult(sapOk2));

            var mockRedis = new Mock<IRedisService>();
            mockRedis.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var handler = new CancelProductionOrderHandler(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                this.mockBackgroundTaskQueue.Object,
                this.mockLogger.Object,
                mockRedis.Object,
                this.mockOrderHistoryHelper.Object);

            // Act
            var ok = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(ok, Is.True);

            var updated = await this.context.UserOrderModel.FirstAsync(u => u.Productionorderid == productionOrderId);
            Assert.That(updated.StatusWorkParent, Is.EqualTo(expectedWorkStatus));

            this.mockBackgroundTaskQueue.Verify(
                q => q.QueueBackgroundWorkItem(It.IsAny<Func<IServiceProvider, CancellationToken, Task>>()),
                Times.AtLeastOnce());
        }
    }
}
