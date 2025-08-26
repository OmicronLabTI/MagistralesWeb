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
    public class ProductionOrderServiceTest : BaseTest
    {
        private Mock<ILogger> mockLogger;
        private IProductionOrderService productionOrderService;

        private IMapper mapper;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            this.mockLogger = new Mock<ILogger>();
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();
            this.productionOrderService = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object, this.mapper);
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
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

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
                ProductionOrderId = 1022,
            };
            var request = new List<CloseProductionOrderDto>();
            request.Add(item1);

            var result = await service.FinishOrder(request);
            var dictResult = (Dictionary<int, string>)result.Response;
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(dictResult.Count, Is.EqualTo(1));
            Assert.That(dictResult[0], Is.EqualTo("Ok"));
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
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

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
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            var dictResult = (Dictionary<int, string>)result.Response;
            Assert.That(dictResult.Count, Is.EqualTo(8));
        }

        /// <summary>
        /// validate update formula.
        /// </summary>
        /// <param name="hasAssignedBatches">HasAssignedBatches.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task UpdateFormulaTest(bool hasAssignedBatches)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))));

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            var assignedBatches = new List<AssignBatchDto>
            {
                new AssignBatchDto { AssignedQty = 5, BatchNumber = "BATCH-001", SysNumber = 1 },
                new AssignBatchDto { AssignedQty = 5, BatchNumber = "BATCH-002", SysNumber = 1 },
            };

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
                AssignedBatches = hasAssignedBatches ? assignedBatches : null,
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

            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// validate update formula.
        /// </summary>
        /// <param name="success">success.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CreateChildFabOrders(bool success)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            var productionOrder = GetProductionOrder("boposReleased", 0, 0);
            mockServiceLayerClient
                .Setup(x => x.GetAsync("ProductionOrders(123456)"))
                .Returns(Task.FromResult(GetResult(true, productionOrder)));

            var secondProductionOrder = GetProductionOrder("boposReleased", 0, 0);
            secondProductionOrder.ProductionOrderLines[0].ItemNo = "TEST";

            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(success, secondProductionOrder)));

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, secondProductionOrder)));
            var request = new CreateChildProductionOrdersDto() { OrderId = 123456, Pieces = 1 };
            var result = await service.CreateChildFabOrders(request);
            var response = result.Response as CreateChildOrderResultDto;

            if (success)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
                Assert.That(string.IsNullOrEmpty(response.ErrorMessage), Is.True);
            }
            else
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
                Assert.That(string.IsNullOrEmpty(response.ErrorMessage), Is.False);
            }
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
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

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

            Assert.That(result.Success, Is.False);
            Assert.That(result.Code, Is.EqualTo(400));
        }

        /// <summary>
        /// validate CreateFabOrder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task CreateFabOrderTest()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .SetupSequence(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)))
                .Returns(Task.FromResult(GetResult(false, null)));

            var order = new CreateOrderRequestDto()
            {
                PedidoId = 1,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
            };

            var detailItem = new CompleteDetailDto()
            {
                QtyPlannedDetalle = 10,
                CodigoProducto = "REVE 14",
                DescripcionProducto = "REVE",
            };

            var detailItem2 = new CompleteDetailDto()
            {
                QtyPlannedDetalle = 1,
                CodigoProducto = "REVE 11",
                DescripcionProducto = "REVE",
            };

            var request = new OrderWithDetailDto()
            {
                Order = order,
                Detalle = new List<CompleteDetailDto>() { detailItem, detailItem2 },
            };

            var result = await service.CreateFabOrder(new List<OrderWithDetailDto>() { request });

            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// validate UpdateFabOrder.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task UpdateFabOrderTest()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))));

            mockServiceLayerClient
                .SetupSequence(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)))
                .Returns(Task.FromResult(GetResult(false, null)));

            var order1 = new UpdateFabOrderDto()
            {
                Status = "R",
                OrderFabId = 1,
            };

            var order2 = new UpdateFabOrderDto()
            {
                Status = "R",
                OrderFabId = 2,
            };

            var result = await service.UpdateFabOrders(new List<UpdateFabOrderDto>() { order1, order2 });

            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// validate CancelFabOrderTest.
        /// </summary>
        /// <param name="orderId">The orderId.</param>
        /// <param name="successGet">The successGet.</param>
        /// <param name="successUpdated">The successUpdated.</param>
        /// <param name="status">The status.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(1, true, true, "boposReleased")]
        [TestCase(2, true, false, "boposReleased")]
        [TestCase(3, true, true, "boposCancelled")]
        public async Task CancelFabOrderTest(int orderId, bool successGet, bool successUpdated, string status)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(successGet, GetProductionOrder(status, 0, 0))));

            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(successUpdated, null)));

            var request = new CancelOrderDto()
            {
                OrderId = orderId,
            };
            var result = await service.CancelProductionOrder(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// validate CancelFabOrderTest.
        /// </summary>
        /// <param name="successGet">The successGet.</param>
        /// <param name="successPut">The successPut.</param>
        /// <param name="successPost">The successPost.</param>
        /// <param name="status">The status.</param>
        /// <param name="responseServiceLayerHasError">responseServiceLayerHasError.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(true, true, true, "boposReleased", false)]
        [TestCase(true, true, true, "boposCancelled", false)]
        [TestCase(true, false, true, "boposReleased", false)]
        [TestCase(true, true, false, "boposReleased", false)]
        [TestCase(true, true, true, "boposReleased", true)]
        public async Task CancelProductionOrderForSeparationProcess(
            bool successGet,
            bool successPut,
            bool successPost,
            string status,
            bool responseServiceLayerHasError)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(successGet, GetProductionOrder(status, 0, 0))));

            mockServiceLayerClient
               .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.FromResult(GetResult(successPut, null)));

            if (!responseServiceLayerHasError)
            {
                mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(successPost, null)));
            }
            else
            {
                mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Deadlock Error."));
            }

            var request = new CancelProductionOrderDto()
            {
                ProductionOrderId = 100001,
                SeparationId = Guid.NewGuid().ToString(),
            };
            var result = await service.CancelProductionOrderForSeparationProcess(request);

            if (result.Success && !responseServiceLayerHasError)
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));

                // Assert.That(result.Response, Is.Null);
                Assert.That(result.UserError, Is.Null);
                Assert.That(result.Comments, Is.Null);
                Assert.That(result.ExceptionMessage, Is.Null);
            }
            else
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(500));
                Assert.That(result.UserError, Is.Null);
                Assert.That(result.Comments, Is.Null);

                // Assert.That(result.Response, Is.Null);
                // Assert.That(result.ExceptionMessage, Is.Not.Null);
            }
        }

        /// <summary>
        /// validate CancelFabOrderTest.
        /// </summary>
        /// <param name="success">The success.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CreateIsolatedProductionOrderTest(bool success)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))));

            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(success, null)));

            var request = new CreateIsolatedFabOrderDto()
            {
                ProductCode = "EM-123",
            };
            var result = await service.CreateIsolatedProductionOrder(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// validate UpdateProductionOrdersBatches.
        /// </summary>
        /// <param name="successGet">The successGet.</param>
        /// <param name="successPut">The successPut.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        public async Task UpdateProductionOrdersBatchesTest(bool successGet, bool successPut)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(successGet, GetProductionOrder("boposReleased", 0, 0))));

            mockServiceLayerClient
                .Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(successPut, null)));

            var firstBatch = new AssignBatchDto()
            {
                Action = "insert",
                ItemCode = "EN-123",
                OrderId = 1,
                BatchNumber = "Axity-1",
                AssignedQty = 1,
            };

            var secondBatch = new AssignBatchDto()
            {
                Action = "delete",
                ItemCode = "EN-123",
                OrderId = 1,
                BatchNumber = "X-111",
                AssignedQty = 1,
            };

            var result = await service.UpdateProductionOrdersBatches(new List<AssignBatchDto> { firstBatch, secondBatch });

            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
        }

        /// <summary>
        /// PrimaryValidationForProductionOrderFinalizationInSap.
        /// </summary>
        /// <param name="isOk">Is Ok.</param>
        /// <param name="statusSap">Status Sap.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(false, "boposReleased")]
        [TestCase(true, "boposClosed")]
        [TestCase(true, "boposPlanned")]
        [TestCase(true, "boposReleased")]
        public async Task PrimaryValidationForProductionOrderFinalizationInSap(bool isOk, string statusSap)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object, this.mapper);

            var productionOrderRespone = GetProductionOrder(statusSap, 0, 0);

            mockServiceLayerClient
                .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(isOk, productionOrderRespone)))
                .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))));

            var request = new List<CloseProductionOrderDto>
            {
                new CloseProductionOrderDto()
                {
                    ProductionOrderId = 1022,
                },
            };

            var result = await service.PrimaryValidationForProductionOrderFinalizationInSap(request);
            var resultTest = (List<ValidationsToFinalizeProductionOrdersResultDto>)result.Response;
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(resultTest.Any(), Is.True);

            if (isOk && statusSap.Equals("boposReleased"))
            {
                Assert.That(resultTest.All(x => string.IsNullOrEmpty(x.ErrorMessage)), Is.True);
            }
            else
            {
                Assert.That(resultTest.All(x => !string.IsNullOrEmpty(x.ErrorMessage)), Is.True);
            }
        }

        /// <summary>
        /// PrimaryValidationForProductionOrderFinalizationInSapError.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task PrimaryValidationForProductionOrderFinalizationInSapError()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var service = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object, this.mapper);

            mockServiceLayerClient
                .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Connection Refused."));

            var request = new List<CloseProductionOrderDto>
            {
                new CloseProductionOrderDto()
                {
                    ProductionOrderId = 1022,
                },
            };

            var result = await service.PrimaryValidationForProductionOrderFinalizationInSap(request);
            var resultTest = (List<ValidationsToFinalizeProductionOrdersResultDto>)result.Response;
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(resultTest.Any(), Is.True);
            Assert.That(resultTest.All(x => !string.IsNullOrEmpty(x.ErrorMessage)), Is.True);
        }

        /// <summary>
        /// FinalizeProductionOrderInSap.
        /// </summary>
        /// <param name="lastStep">Last Step.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase("Primary Validations")]
        [TestCase("Create Inventory")]
        [TestCase("Create Receipt")]
        [TestCase("Invalid Step")]
        public async Task FinalizeProductionOrderInSap(string lastStep)
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            if (lastStep == "Primary Validations")
            {
                mockServiceLayerClient
                   .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                   .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
                   .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
                   .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))));
            }
            else
            {
                mockServiceLayerClient
                   .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                   .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
                   .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))));
            }

            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            mockServiceLayerClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            var service = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object, this.mapper);

            var request = new List<CloseProductionOrderDto>
            {
                new CloseProductionOrderDto()
                {
                    ProductionOrderId = 1022,
                    LastStep = lastStep,
                    ProcessId = "4c1e8273-0312-4514-b985-d0a53403d1bb",
                    Batches = new List<BatchesConfigurationDto>
                    {
                        new BatchesConfigurationDto { BatchCode = "Batch Code", ExpirationDate = DateTime.Now.AddYears(2), ManufacturingDate = DateTime.Now, Quantity = "2" },
                    },
                },
            };

            var result = await service.FinalizeProductionOrderInSap(request);
            var response = (List<ValidationsToFinalizeProductionOrdersResultDto>)result.Response;
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(response.Count, Is.EqualTo(1));
            Assert.That(response.All(x => string.IsNullOrEmpty(x.ErrorMessage)), Is.True);

            if (lastStep != "Invalid Step")
            {
                Assert.That(response.All(x => x.LastStep == "Successfully Closed In SAP"), Is.True);
            }
            else
            {
                Assert.That(response.All(x => x.LastStep == lastStep), Is.True);
            }
        }

        /// <summary>
        /// FinalizeProductionOrderInSap.
        /// </summary>
        /// <param name="lastStep">Last Step.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task FinalizeProductionOrderInSapConnectionError()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            mockServiceLayerClient
               .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
               .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
               .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))));

            mockServiceLayerClient
                .SetupSequence(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)))
                .ThrowsAsync(new Exception("Connection Refused."));

            mockServiceLayerClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            var service = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object, this.mapper);

            var request = new List<CloseProductionOrderDto>
            {
                new CloseProductionOrderDto()
                {
                    ProductionOrderId = 1022,
                    LastStep = "Primary Validations",
                    ProcessId = "4c1e8273-0312-4514-b985-d0a53403d1dd",
                    Batches = new List<BatchesConfigurationDto>
                    {
                        new BatchesConfigurationDto { BatchCode = "Batch Code", ExpirationDate = DateTime.Now.AddYears(2), ManufacturingDate = DateTime.Now, Quantity = "2" },
                    },
                },
            };

            var result = await service.FinalizeProductionOrderInSap(request);
            var response = (List<ValidationsToFinalizeProductionOrdersResultDto>)result.Response;
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(response.Count, Is.EqualTo(1));
            Assert.That(response.All(x => !string.IsNullOrEmpty(x.ErrorMessage)), Is.True);
            Assert.That(response.All(x => x.LastStep == "Create Inventory"), Is.True);
        }

        /// <summary>
        /// FinalizeProductionOrderInSap.
        /// </summary>
        /// <param name="lastStep">Last Step.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        public async Task FinalizeProductionOrderInSapServiceLayerError()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            mockServiceLayerClient
               .SetupSequence(x => x.GetAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
               .Returns(Task.FromResult(GetResult(true, GetProductionOrder("boposReleased", 0, 0))))
               .Returns(Task.FromResult(GetResult(true, GetProduct("EM-123", "tYES", 100))));

            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(true, null)));

            var deadLockResult = new ServiceLayerErrorResponseDto
            {
                Error = new ServiceLayerErrorDetailDto
                {
                    Code = -2038,
                    Message = new ServiceLayerErrorMessageDto
                    {
                        Lang = "en-us",
                        Value = "Could not commit transaction: Deadlock (-2038) detected during transaction",
                    },
                },
            };

            mockServiceLayerClient
                .Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(GetResult(false, deadLockResult)));

            var service = new ProductionOrderService(mockServiceLayerClient.Object, this.mockLogger.Object, this.mapper);

            var request = new List<CloseProductionOrderDto>
            {
                new CloseProductionOrderDto()
                {
                    ProductionOrderId = 1022,
                    LastStep = "Primary Validations",
                    ProcessId = "4c1e8273-0312-4514-b985-d0a53403d1dd",
                    Batches = new List<BatchesConfigurationDto>
                    {
                        new BatchesConfigurationDto { BatchCode = "Batch Code", ExpirationDate = DateTime.Now.AddYears(2), ManufacturingDate = DateTime.Now, Quantity = "2" },
                    },
                },
            };

            var result = await service.FinalizeProductionOrderInSap(request);
            var response = (List<ValidationsToFinalizeProductionOrdersResultDto>)result.Response;
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(response.Count, Is.EqualTo(1));
            Assert.That(response.All(x => !string.IsNullOrEmpty(x.ErrorMessage)), Is.True);
            Assert.That(response.All(x => x.LastStep == "Create Receipt"), Is.True);
        }
    }
}