// <summary>
// <copyright file="ProductionOrderServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services.ProductionOrder
{
    /// <summary>
    /// Class ProductionOrderServiceTest.
    /// </summary>
    [TestFixture]
    public class ProductionOrderServiceTest
    {
        private Mock<ILogger> mockLogger;
        private IProductionOrderService productionOrderService;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            this.mockLogger = new Mock<ILogger>();
            this.productionOrderService = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object);
        }

        /// <summary>
        /// Validate finish order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task FinishOrderTestSuccess()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object);

            mockServiceLayerClient
                .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))))
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
                .Returns(Task.FromResult(GetResult(true, GetProduct("EN-123", "tNo", 100))));

            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            mockServiceLayerClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            var item1 = new CloseProductionOrderDto()
            {
                OrderId = 1022,
            };
            var request = new List<CloseProductionOrderDto>();
            request.Add(item1);

            var result = await service.FinishOrder(request);
            var dictResult = (Dictionary<int, string>)result.Response;
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);
            Assert.AreEqual(1, dictResult.Count);
            Assert.AreEqual("Ok", dictResult[0]);
        }

        /// <summary>
        /// Validate finish order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task FinishOrderTestError()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object);

            mockServiceLayerClient
                .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(false, null))) // fail get- order1
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposClosed", 0, 0)))) // closed -order2
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 1, 0)))) // IssuedQuantity == 0 order3
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // stock order-4
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 0)))) // stock order-4
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // Results order-5
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))) // Results order-5
                .Returns(Task.FromResult(GetResult(true, GetBatchNumber(true)))) // Results order-5

                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 1)))) // IssuedQuantity order-6
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))) // IssuedQuantity order-6
                .Returns(Task.FromResult(GetResult(true, GetBatchNumber(false)))) // IssuedQuantity order-6

                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // SaveInventoryGenExit order-7
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))) // SaveInventoryGenExit order-7
                .Returns(Task.FromResult(GetResult(true, GetBatchNumber(false)))) // SaveInventoryGenExit order-7

                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // SaveInventoryGenEntry order-8
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))) // SaveInventoryGenEntry order-8
                .Returns(Task.FromResult(GetResult(true, GetBatchNumber(false)))) // SaveInventoryGenEntry order-8
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // SaveInventoryGenEntry order-8
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))) // SaveInventoryGenEntry order-8

                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // CloseProductionOrder order-9
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))) // CloseProductionOrder order-9
                .Returns(Task.FromResult(GetResult(true, GetBatchNumber(false)))) // CloseProductionOrder order-9
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0)))) // CloseProductionOrder order-9
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100)))); // CloseProductionOrder order-9

            mockServiceLayerClient
                .SetupSequence(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(false, null))) // SaveInventoryGenExit order-7
                .Returns(Task.FromResult(GetResult(true, null))) // SaveInventoryGenEntry order-8
                .Returns(Task.FromResult(GetResult(false, null))) // SaveInventoryGenEntry order-8
                .Returns(Task.FromResult(GetResult(true, null))) // CloseProductionOrder order-9
                .Returns(Task.FromResult(GetResult(true, null))); // CloseProductionOrder order-9

            mockServiceLayerClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(false, null)));

            var request = new List<CloseProductionOrderDto>();
            request.Add(GetCloseProductionOrderDto(1, true));
            request.Add(GetCloseProductionOrderDto(2, true));
            request.Add(GetCloseProductionOrderDto(3, true));
            request.Add(GetCloseProductionOrderDto(4, true));
            request.Add(GetCloseProductionOrderDto(5, true));
            request.Add(GetCloseProductionOrderDto(6, true));
            request.Add(GetCloseProductionOrderDto(7, true));
            request.Add(GetCloseProductionOrderDto(8, true));
            request.Add(GetCloseProductionOrderDto(9, true));

            var result = await service.FinishOrder(request);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);
            var dictResult = (Dictionary<int, string>)result.Response;
            Assert.AreEqual(8, dictResult.Count);
        }

        /// <summary>
        /// validate update formula.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task UpdateFormulaTest()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))));

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            var deleteComponent = new CompleteDetalleFormulaDto()
            {
                Action = "delete",
                ProductId = "EM-123",
                BaseQuantity = 1,
                RequiredQuantity = 1,
                Warehouse = "MN",
            };
            var newComponent = new CompleteDetalleFormulaDto()
            {
                Action = "insert",
                ProductId = "XML-1234",
                BaseQuantity = 10,
                RequiredQuantity = 10,
                Warehouse = "MN",
            };
            var editedComponent = new CompleteDetalleFormulaDto()
            {
                Action = "edit",
                ProductId = "EN-123",
                BaseQuantity = 10,
                RequiredQuantity = 10,
                Warehouse = "MN",
            };

            var request = new UpdateFormulaDto()
            {
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
                Warehouse = "MN",
                Components = new List<CompleteDetalleFormulaDto>() { deleteComponent, newComponent, editedComponent },
            };
            var result = await service.UpdateFormula(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);
        }

        /// <summary>
        /// validate update formula.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task UpdateFormulaWithErrorTest()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))));

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(false, null)));

            var deleteComponent = new CompleteDetalleFormulaDto()
            {
                Action = "delete",
                ProductId = "EM-123",
                BaseQuantity = 1,
                RequiredQuantity = 1,
                Warehouse = "MN",
            };
            var newComponent = new CompleteDetalleFormulaDto()
            {
                Action = "insert",
                ProductId = "XML-1234",
                BaseQuantity = 10,
                RequiredQuantity = 10,
                Warehouse = "MN",
            };
            var editedComponent = new CompleteDetalleFormulaDto()
            {
                Action = "edit",
                ProductId = "EN-123",
                BaseQuantity = 10,
                RequiredQuantity = 10,
                Warehouse = "MN",
            };

            var request = new UpdateFormulaDto()
            {
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
                Warehouse = "MN",
                Components = new List<CompleteDetalleFormulaDto>() { deleteComponent, newComponent, editedComponent },
            };
            var result = await service.UpdateFormula(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(400, result.Code);
        }

        private static CloseProductionOrderDto GetCloseProductionOrderDto(int orderId, bool batches)
        {
            var batch = new BatchesConfigurationDto()
            {
                Quantity = "1.0",
                BatchCode = "Code",
                ManufacturingDate = DateTime.Now,
                ExpirationDate = DateTime.Now,
            };
            var item = new CloseProductionOrderDto()
            {
                OrderId = orderId,
                Batches = batches ? new List<BatchesConfigurationDto>() { batch } : null,
            };

            return item;
        }

        private static ProductionOrderDto GetProductionOrder(string status, double issuedQuantityB, double issuedQuantityM)
        {
            var firstProduct = new ProductionOrderLineDto()
            {
                ProductionOrderIssueType = "im_Backflush",
                ItemNo = "EM-123",
                Warehouse = "MN",
                PlannedQuantity = 1,
                IssuedQuantity = issuedQuantityB,
            };

            var batch = new ProductionOrderItemBatchDto()
            {
                BatchNumber = "X-111",
                Quantity = 1,
            };

            var secondProduct = new ProductionOrderLineDto()
            {
                ProductionOrderIssueType = "im_Manual",
                ItemNo = "EN-123",
                Warehouse = "MG",
                PlannedQuantity = 1,
                IssuedQuantity = issuedQuantityM,
                BatchNumbers = new List<ProductionOrderItemBatchDto>() { batch },
            };

            var productionOrder = new ProductionOrderDto()
            {
                ProductionOrderStatus = status,
                AbsoluteEntry = 1,
                DocumentNumber = 1,
                Series = 3,
                ProductionOrderLines = new List<ProductionOrderLineDto>() { firstProduct, secondProduct },
                PlannedQuantity = 1,
                Warehouse = "MN",
            };

            return productionOrder;
        }

        private static ItemDto GetProduct(string name, string manageBatchNumbers, double stock)
        {
            var itemWareHouse = new ItemWarehouseInfoDto()
            {
                WarehouseCode = "MN",
                InStock = stock,
            };

            var firstItem = new ItemDto()
            {
                ManageBatchNumbers = manageBatchNumbers,
                ItemCode = name,
                ItemWarehouseInfoCollection = new List<ItemWarehouseInfoDto>() { itemWareHouse },
            };

            return firstItem;
        }

        private static BatchNumberResponseDto GetBatchNumber(bool hasResults)
        {
            return new BatchNumberResponseDto()
            {
                Results = hasResults ? new List<BatchNumberDetailDto>() { new BatchNumberDetailDto() } : new List<BatchNumberDetailDto>(),
            };
        }

        private static ResultModel GetResult(bool success, object data)
        {
            return new ResultModel()
            {
                Code = success ? 200 : 400,
                Success = success,
                Response = JsonConvert.SerializeObject(data),
                UserError = success ? null : "Error",
            };
        }
    }
}