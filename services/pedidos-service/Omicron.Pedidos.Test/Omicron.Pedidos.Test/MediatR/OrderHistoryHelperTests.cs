// <summary>
// <copyright file="OrderHistoryHelperTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.MediatR
{
    /// <summary>
    /// Tests for OrderHistoryHelperTests.
    /// </summary>
    public class OrderHistoryHelperTests : BaseTest
    {
        private OrderHistoryHelper orderHistoryHelper;
        private Mock<IPedidosDao> mockPedidosDao;
        private Mock<ILogger> mockLogger;
        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalOrderHistory")
                .Options;
            this.context = new DatabaseContext(options);

            this.mockPedidosDao = new Mock<IPedidosDao>();
            this.mockLogger = new Mock<ILogger>();

            this.orderHistoryHelper = new OrderHistoryHelper(
                this.mockPedidosDao.Object,
                this.mockLogger.Object);
        }

        /// <summary>
        /// Tests for OrderHistory.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockPedidosDao.Reset();
            this.mockLogger.Reset();
        }

        /// <summary>
        /// successful insertion of child.
        /// </summary>
        /// <returns>Task.</returns>
        [Test]
        public async Task RegisterOrdersDetail()
        {
            // Arrange
            int detailOrderId = 1001;
            var request = new CreateChildOrdersSapCommand(
                productionOrderId: 1000,
                pieces: 5,
                separationId: "99c7c928-2afa",
                userId: "testUser",
                dxpOrder: "99c7c928-2afa-4a84-8a3e-13ce38bfd98f",
                sapOrder: 789456,
                totalPieces: 10,
                lastStep: string.Empty);

            this.mockPedidosDao
                .Setup(x => x.GetMaxDivision(request.ProductionOrderId))
                .ReturnsAsync(0);

            this.mockPedidosDao
                .Setup(x => x.InsertDetailOrder(It.IsAny<ProductionOrderSeparationDetailModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.orderHistoryHelper.RegisterSeparatedOrdersDetail(
                detailOrderId, request);

            // Assert
            Assert.That(result, Is.True);
            this.mockPedidosDao.Verify(
                x => x.GetMaxDivision(request.ProductionOrderId),
                Times.Once);
            this.mockPedidosDao.Verify(
                x => x.InsertDetailOrder(It.IsAny<ProductionOrderSeparationDetailModel>()),
                Times.Once);
        }

        /// <summary>
        /// successful insertion of parent.
        /// </summary>
        /// <returns>Task.</returns>
        [Test]
        public async Task UpsertOrderParent()
        {
            // Arrange
            int orderId = 1000;
            int totalPieces = 10;
            int assignedPieces = 3;

            this.mockPedidosDao
                .Setup(x => x.GetParentOrderId(orderId))
                .ReturnsAsync((ProductionOrderSeparationModel)null);

            this.mockPedidosDao
                .Setup(x => x.InsertOrder(It.IsAny<ProductionOrderSeparationModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.orderHistoryHelper.UpsertOrderSeparation(
                orderId,
                totalPieces,
                assignedPieces);

            // Assert
            Assert.That(result, Is.True);
            this.mockPedidosDao.Verify(
                x => x.GetParentOrderId(orderId),
                Times.Once);
            this.mockPedidosDao.Verify(
                x => x.InsertOrder(It.Is<ProductionOrderSeparationModel>(
                p => p.OrderId == orderId
                && p.TotalPieces == totalPieces
                && p.AvailablePieces == (totalPieces - assignedPieces)
                && p.ProductionDetailCount == 1
                && p.Status == ServiceConstants.PartiallyDivided)),
                Times.Once);
        }

        /// <summary>
        /// successful update of parent.
        /// </summary>
        /// <returns>Task.</returns>
        [Test]
        public async Task UpsertOrderExistingParent()
        {
            // Arrange
            int orderId = 1000;
            int totalPieces = 10;
            int assignedPieces = 3;

            var existingParent = new ProductionOrderSeparationModel
            {
                OrderId = orderId,
                ProductionDetailCount = 1,
                TotalPieces = totalPieces,
                AvailablePieces = 7, // Ya había 3 asignadas
                Status = ServiceConstants.PartiallyDivided,
                CompletedAt = null,
            };

            this.mockPedidosDao
                .Setup(x => x.GetParentOrderId(orderId))
                .ReturnsAsync(existingParent);

            this.mockPedidosDao
                .Setup(x => x.UpdateOrder(It.IsAny<ProductionOrderSeparationModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.orderHistoryHelper.UpsertOrderSeparation(
                orderId,
                totalPieces,
                assignedPieces);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(existingParent.ProductionDetailCount, Is.EqualTo(2));
            Assert.That(existingParent.AvailablePieces, Is.EqualTo(4)); // 7 - 3
            Assert.That(existingParent.Status, Is.EqualTo(ServiceConstants.PartiallyDivided));
            Assert.That(existingParent.CompletedAt, Is.Null);

            this.mockPedidosDao.Verify(
                x => x.UpdateOrder(existingParent),
                Times.Once);
        }

        /// <summary>
        /// Validation of duplicates child.
        /// </summary>
        /// <returns>Task.</returns>
        [Test]
        public async Task SaveHistoryOrdersExistingChild()
        {
            // Arrange
            int detailOrderId = 1001;
            var request = new CreateChildOrdersSapCommand(
                productionOrderId: 1000,
                pieces: 5,
                separationId: "some-separation-id",
                userId: "testUser",
                dxpOrder: "some-dxporder",
                sapOrder: 789456,
                totalPieces: 10,
                lastStep: string.Empty);

            var existingOrder = new ProductionOrderSeparationDetailModel
            {
                DetailOrderId = detailOrderId,
                OrderId = request.ProductionOrderId,
            };

            this.mockPedidosDao
                .Setup(x => x.GetDetailOrderById(detailOrderId))
                .ReturnsAsync(existingOrder);

            // Act
            await this.orderHistoryHelper.SaveHistoryOrdersFab(
                detailOrderId, request);

            // Assert
            this.mockPedidosDao.Verify(
                x => x.GetDetailOrderById(detailOrderId),
                Times.Once);
            this.mockPedidosDao.Verify(
                x => x.GetMaxDivision(It.IsAny<int>()),
                Times.Never);
            this.mockPedidosDao.Verify(
                x => x.InsertDetailOrder(It.IsAny<ProductionOrderSeparationDetailModel>()),
                Times.Never);
            this.mockPedidosDao.Verify(
                x => x.GetParentOrderId(It.IsAny<int>()),
                Times.Never);
            this.mockPedidosDao.Verify(
                x => x.InsertOrder(It.IsAny<ProductionOrderSeparationModel>()),
                Times.Never);
        }
    }
}
