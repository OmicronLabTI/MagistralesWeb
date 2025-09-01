// <summary>
// <copyright file="CreateChildOrdersSapHandlerTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    using Omicron.Pedidos.Services.OrderHistory;

    /// <summary>
    /// Tests for CreateChildOrdersSapHandlerTest.
    /// </summary>
    [TestFixture]
    public class CreateChildOrdersSapHandlerTest : BaseTest
    {
        private Mock<ILogger> mockLogger;
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
            this.mockOrderHistoryHelper = new Mock<IOrderHistoryHelper>();
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task CreateChildOrdersSapHandlerFullFlow()
        {
            // Arrange
            var command = new CreateChildOrdersSapCommand(
                226744,
                5,
                "8a61faf7-61b8-4556-bc3c-02bf4240d830",
                "axity1",
                "xxx-xxx-xxx",
                null,
                10,
                "SaveHistory");

            var result = new CreateChildOrderResultDto()
            {
                ProductionOrderChildId = 226729,
                LastStep = "CreateChildOrderSapWithComponents",
                ErrorMessage = null,
            };

            var sapResultCreateDetail = this.GetResultModelCompl(result, true, string.Empty, null);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .SetupSequence(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.FromResult(sapResultCreateDetail));

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var handler = new CreateChildOrdersSapHandler(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                this.mockLogger.Object,
                mockRedisService.Object,
                this.mockOrderHistoryHelper.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            Assert.That(command.LastStep, Is.EqualTo("SaveChildOrderHistory")); // comprueba el ultimo paso exitoso
            this.mockLogger.Verify(l => l.Information(It.Is<string>(msg => msg.Contains("ejecutado correctamente") && msg.Contains("226729"))), Times.Once); // verifica el flujo correcto
            this.mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.Is<string>(msg => msg.Contains("Error en el proceso de division"))), Times.Never); // verifica que nunca entro al catch
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task CreateChildOrdersSapHandlerFullFlowWithError()
        {
            // Arrange
            var command = new CreateChildOrdersSapCommand(
                226744,
                5,
                "3dbc7213-5118-4c92-9619-ebdcc5c1e45a",
                "axity1",
                "xxx-xxx-xxx",
                null,
                10,
                ServiceConstants.SaveHistoryStep);

            var result = new CreateChildOrderResultDto()
            {
                ProductionOrderChildId = 0,
                LastStep = "CreateChildOrderSap",
                ErrorMessage = "Error al Agregar componentes",
            };

            var sapResultCreateDetail = this.GetResultModelCompl(result);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.FromResult(sapResultCreateDetail));

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var handler = new CreateChildOrdersSapHandler(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                this.mockLogger.Object,
                mockRedisService.Object,
                this.mockOrderHistoryHelper.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            Assert.That(command.LastStep, Is.EqualTo("CreateChildOrderSap")); // ultimo paso antes del error
            this.mockLogger.Verify(l => l.Error(It.Is<string>(msg => msg.Contains("Error service layer."))), Times.Once); // error en uno de los procesos de sl
            this.mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.Is<string>(msg => msg.Contains("Error en el proceso de division"))), Times.Once); // entro a la logica del catch
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task CreateChildOrdersSapHandlerCreationPostgres()
        {
            // Arrange
            var command = new CreateChildOrdersSapCommand(
                226744,
                5,
                "9f8f3abf-1a74-4560-a876-91405e772a4b",
                "axity1",
                "xxx-xxx-xxx",
                null,
                10,
                ServiceConstants.StepCreateChildOrderWithComponentsSap);

            command.ProductionOrderChildId = 226532;

            var result = new CreateChildOrderResultDto()
            {
                ProductionOrderChildId = 226532,
                LastStep = "CreateChildOrderSap",
                ErrorMessage = "Error al Agregar componentes",
            };

            var sapResultCreateDetail = this.GetResultModelCompl(result);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.FromResult(sapResultCreateDetail));

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var handler = new CreateChildOrdersSapHandler(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                this.mockLogger.Object,
                mockRedisService.Object,
                this.mockOrderHistoryHelper.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            Assert.That(command.LastStep, Is.EqualTo("SaveChildOrderHistory")); // comprueba el ultimo paso exitoso
            this.mockLogger.Verify(l => l.Information(It.Is<string>(msg => msg.Contains("ejecutado correctamente") && msg.Contains("226532"))), Times.Once); // verifica el flujo correcto
            this.mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.Is<string>(msg => msg.Contains("Error en el proceso de division") && msg.Contains("9f8f3abf-1a74-4560-a876-91405e772a4b"))), Times.Never); // verifica que nunca entro al catch
        }

        /// <summary>
        /// Test Handle.
        /// </summary>
        /// <returns>Test task.</returns>
        [Test]
        public async Task CreateChildOrdersSapHandlerSaveHistory()
        {
            // Arrange
            var command = new CreateChildOrdersSapCommand(
                226744,
                5,
                "03c5e153-363c-462c-a904-8d08dc3e83d2",
                "axity1",
                "xxx-xxx-xxx",
                null,
                10,
                ServiceConstants.StepCreateChildOrderPostgres);

            command.ProductionOrderChildId = 226280;

            var result = new CreateChildOrderResultDto()
            {
                ProductionOrderChildId = 226280,
                LastStep = "CreateChildOrderSap",
                ErrorMessage = "Error al Agregar componentes",
            };

            var sapResultCreateDetail = this.GetResultModelCompl(result);
            var mockServiceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerAdapterService
               .Setup(sla => sla.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.FromResult(sapResultCreateDetail));

            var mockRedisService = new Mock<IRedisService>();
            mockRedisService.Setup(m => m.DeleteKey(It.IsAny<string>()));

            var handler = new CreateChildOrdersSapHandler(
                this.pedidosDao,
                mockServiceLayerAdapterService.Object,
                this.mockLogger.Object,
                mockRedisService.Object,
                this.mockOrderHistoryHelper.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            Assert.That(command.LastStep, Is.EqualTo("SaveChildOrderHistory")); // comprueba el ultimo paso exitoso
            this.mockLogger.Verify(l => l.Information(It.Is<string>(msg => msg.Contains("ejecutado correctamente") && msg.Contains("226280"))), Times.Once); // verifica el flujo correcto
            this.mockLogger.Verify(l => l.Error(It.IsAny<Exception>(), It.Is<string>(msg => msg.Contains("Error en el proceso de division") && msg.Contains("03c5e153-363c-462c-a904-8d08dc3e83d2"))), Times.Never); // verifica que nunca entro al catch
        }
    }
}
