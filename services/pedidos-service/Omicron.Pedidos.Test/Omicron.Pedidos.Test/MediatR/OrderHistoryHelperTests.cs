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
        private Mock<IOrderHistoryDao> mockOrderHistoryDao;
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

            this.mockOrderHistoryDao = new Mock<IOrderHistoryDao>();
            this.mockLogger = new Mock<ILogger>();

            this.orderHistoryHelper = new OrderHistoryHelper(
                this.mockOrderHistoryDao.Object,
                this.mockLogger.Object);
        }

        /// <summary>
        /// Tests for OrderHistory.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.mockOrderHistoryDao.Reset();
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
            int orderId = 1000;
            string userId = "testUser";
            string dxpOrder = "99c7c928-2afa-4a84-8a3e-13ce38bfd98f";
            int sapOrder = 789456;
            int assignedPieces = 5;

            this.mockOrderHistoryDao
                .Setup(x => x.GetMaxDivision(orderId))
                .ReturnsAsync(0);

            this.mockOrderHistoryDao
                .Setup(x => x.InsertDetailOrder(It.IsAny<ProductionOrderSeparationDetailModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.orderHistoryHelper.RegisterSeparatedOrdersDetail(
                detailOrderId,
                orderId,
                userId,
                dxpOrder,
                sapOrder,
                assignedPieces);

            // Assert
            Assert.That(result, Is.True);
            this.mockOrderHistoryDao.Verify(
                x => x.GetMaxDivision(orderId),
                Times.Once);
            this.mockOrderHistoryDao.Verify(
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

            this.mockOrderHistoryDao
                .Setup(x => x.GetParentOrderId(orderId))
                .ReturnsAsync((ProductionOrderSeparationModel)null);

            this.mockOrderHistoryDao
                .Setup(x => x.InsertOrder(It.IsAny<ProductionOrderSeparationModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.orderHistoryHelper.UpsertOrderSeparation(
                orderId,
                totalPieces,
                assignedPieces);

            // Assert
            Assert.That(result, Is.True);
            this.mockOrderHistoryDao.Verify(
                x => x.GetParentOrderId(orderId),
                Times.Once);
            this.mockOrderHistoryDao.Verify(
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

            this.mockOrderHistoryDao
                .Setup(x => x.GetParentOrderId(orderId))
                .ReturnsAsync(existingParent);

            this.mockOrderHistoryDao
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

            this.mockOrderHistoryDao.Verify(
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
            int orderId = 1000;
            string userId = "testUser";
            string dxpOrder = "99c7c928-2afa-4a84-8a3e-13ce38bfd98f";
            int sapOrder = 789456;
            int assignedPieces = 5;
            int totalPieces = 10;

            var existingOrder = new ProductionOrderSeparationDetailModel
            {
                DetailOrderId = detailOrderId,
                OrderId = orderId,
            };

            this.mockOrderHistoryDao
                .Setup(x => x.GetDetailOrderById(detailOrderId))
                .ReturnsAsync(existingOrder);

            // Act
            await this.orderHistoryHelper.SaveHistoryOrdersFab(
                detailOrderId,
                orderId,
                userId,
                dxpOrder,
                sapOrder,
                assignedPieces,
                totalPieces);

            // Assert
            this.mockOrderHistoryDao.Verify(
                x => x.GetDetailOrderById(detailOrderId),
                Times.Once);
            this.mockOrderHistoryDao.Verify(
                x => x.GetMaxDivision(It.IsAny<int>()),
                Times.Never);
            this.mockOrderHistoryDao.Verify(
                x => x.InsertDetailOrder(It.IsAny<ProductionOrderSeparationDetailModel>()),
                Times.Never);
            this.mockOrderHistoryDao.Verify(
                x => x.GetParentOrderId(It.IsAny<int>()),
                Times.Never);
            this.mockOrderHistoryDao.Verify(
                x => x.InsertOrder(It.IsAny<ProductionOrderSeparationModel>()),
                Times.Never);
        }
    }
}
