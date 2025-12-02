// <summary>
// <copyright file="CancelPedidosServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class CancelPedidosServiceTest : BaseTest
    {
        private ICancelPedidosService cancelPedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> mockSapAdapter;

        private Mock<ISapServiceLayerAdapterService> serviceLayerAdapterService;

        private Mock<IPedidosDao> pedidosDaoMock;

        private DatabaseContext context;

        private string userId = "abc";

        /// <summary>
        /// Get user order models.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserOrderModelsForCancellationTests()
        {
            return new List<UserOrderModel>
            {
                new UserOrderModel { Id = 50, Productionorderid = "1050", Salesorderid = string.Empty, Status = "Proceso", Userid = "abcd" },

                new UserOrderModel { Id = 51, Productionorderid = null, Salesorderid = "10051", Status = "Finalizado", Userid = "abcd" },
                new UserOrderModel { Id = 52, Productionorderid = "1052", Salesorderid = "10051", Status = "Reasignado", Userid = "abcd" },

                new UserOrderModel { Id = 53, Productionorderid = null, Salesorderid = "10053", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 54, Productionorderid = "1054", Salesorderid = "10053", Status = "Cancelado", Userid = "abcd" },
                new UserOrderModel { Id = 55, Productionorderid = "1055", Salesorderid = "10053", Status = "Proceso", Userid = "abcd" },

                new UserOrderModel { Id = 56, Productionorderid = "1056", Salesorderid = string.Empty, Status = "Cancelado", Userid = "abcd" },

                new UserOrderModel { Id = 57, Productionorderid = null, Salesorderid = "10057", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 58, Productionorderid = "1057", Salesorderid = "10057", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 59, Productionorderid = "1058", Salesorderid = "10057", Status = "Proceso", Userid = "abcd" },

                new UserOrderModel { Id = 100, Productionorderid = null, Salesorderid = "86961", Status = "Finalizado", Userid = "abcd", DeliveryId = 0 },
                new UserOrderModel { Id = 101, Productionorderid = "1064", Salesorderid = "86961", Status = "Almacenado", Userid = "abcd", DeliveryId = 0 },
                new UserOrderModel { Id = 102, Productionorderid = "1065", Salesorderid = "86961", Status = "Proceso", Userid = "abcd", DeliveryId = 0 },
                new UserOrderModel { Id = 103, Productionorderid = "1066", Salesorderid = "86961", Status = "Almacenado", Userid = "abcd", DeliveryId = 1 },
                new UserOrderModel { Id = 104, Productionorderid = "1067", Salesorderid = "86961", Status = "Proceso", Userid = "abcd", DeliveryId = 0 },

                new UserOrderModel { Id = 60, Productionorderid = "1060", Salesorderid = string.Empty, Status = "Finalizado", Userid = "abcd" },

                new UserOrderModel { Id = 61, Productionorderid = null, Salesorderid = "10061", Status = "Proceso", Userid = "abcd" },
                new UserOrderModel { Id = 62, Productionorderid = "1062", Salesorderid = "10061", Status = "Finalizado", Userid = "abcd" },
                new UserOrderModel { Id = 63, Productionorderid = "1063", Salesorderid = "10061", Status = "Proceso", Userid = "abcd" },

                // CancelDelivery Total
                new UserOrderModel { Id = 64, Productionorderid = null, Salesorderid = "84368", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 65, Productionorderid = "122715", Salesorderid = "84368", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74571 },
                new UserOrderModel { Id = 66, Productionorderid = null, Salesorderid = "84369", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 67, Productionorderid = "122715", Salesorderid = "84369", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74573 },
                new UserOrderModel { Id = 68, Productionorderid = null, Salesorderid = "84370", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 69, Productionorderid = "122715", Salesorderid = "84370", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74575 },
                new UserOrderModel { Id = 70, Productionorderid = "122716", Salesorderid = "84370", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74576 },
                new UserOrderModel { Id = 71, Productionorderid = null, Salesorderid = "84371", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 72, Productionorderid = "122715", Salesorderid = "84371", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74577 },
                new UserOrderModel { Id = 73, Productionorderid = "122716", Salesorderid = "84371", Status = "Finalizado", StatusAlmacen = null, DeliveryId = 0 },

                // cancelDeliveryPartial
                new UserOrderModel { Id = 74, Productionorderid = null, Salesorderid = "84372", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 75, Productionorderid = "122715", Salesorderid = "84372", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74578 },
                new UserOrderModel { Id = 76, Productionorderid = "122716", Salesorderid = "84372", Status = "Pendiente", StatusAlmacen = null, DeliveryId = 0 },
                new UserOrderModel { Id = 77, Productionorderid = null, Salesorderid = "84373", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 78, Productionorderid = "122715", Salesorderid = "84373", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74580 },
                new UserOrderModel { Id = 79, Productionorderid = "122716", Salesorderid = "84373", Status = "Pendiente", StatusAlmacen = null, DeliveryId = 0 },
                new UserOrderModel { Id = 80, Productionorderid = null, Salesorderid = "84374", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 81, Productionorderid = "122715", Salesorderid = "84374", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74581 },
                new UserOrderModel { Id = 82, Productionorderid = "122716", Salesorderid = "84374", Status = "Finalizado", StatusAlmacen = null, DeliveryId = 0 },
                new UserOrderModel { Id = 83, Productionorderid = null, Salesorderid = "84375", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 84, Productionorderid = "122715", Salesorderid = "84375", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74583 },
                new UserOrderModel { Id = 85, Productionorderid = "122716", Salesorderid = "84375", Status = "Finalizado", StatusAlmacen = null, DeliveryId = 0 },
                new UserOrderModel { Id = 86, Productionorderid = null, Salesorderid = "84376", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 87, Productionorderid = "122715", Salesorderid = "84376", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74584 },
                new UserOrderModel { Id = 88, Productionorderid = "122716", Salesorderid = "84376", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 0 },
                new UserOrderModel { Id = 89, Productionorderid = null, Salesorderid = "84377", Status = "Almacenado", StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 90, Productionorderid = "122715", Salesorderid = "84377", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 74585 },
                new UserOrderModel { Id = 91, Productionorderid = "122716", Salesorderid = "84377", Status = "Almacenado", StatusAlmacen = "Almacenado", DeliveryId = 0 },

                // clean Delivery
                new UserOrderModel { Id = 92, Productionorderid = "122715", Salesorderid = "84377", Status = "Almacenado", StatusAlmacen = "Almacenado", InvoiceId = 100 },
                new UserOrderModel { Id = 93, Productionorderid = "122716", Salesorderid = "84377", Status = "Almacenado", StatusAlmacen = "Almacenado", InvoiceId = 100 },
            };
        }

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var options = CreateNewContextOptions();
            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModelsForCancellationTests());
            this.context.UserOrderModel.AddRange(this.GetUserModelsForPackangignCancelation());
            this.context.UserOrderModel.AddRange(this.GetUserModelsForTotalCancellationInformation());
            this.context.SaveChanges();
            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosDaoMock = new Mock<IPedidosDao>();
        }

        /// <summary>
        /// Build a service instance.
        /// </summary>
        /// <param name="sapAdapterResponseSalesOrderContent">Sap adapter response.</param>
        /// <param name="diapiResponseContent">Di api response.</param>
        /// <param name="sapAdapteProdOrdersResponseContent">Sap adapter production orders response.</param>
        /// <returns>Service instance.</returns>
        public CancelPedidosService BuildService(object sapAdapterResponseSalesOrderContent, object diapiResponseContent, object sapAdapteProdOrdersResponseContent = null)
        {
            var mockResultDiapi = new ResultModel();
            mockResultDiapi.Success = true;
            mockResultDiapi.Code = 200;
            mockResultDiapi.Response = diapiResponseContent;

            var mockResultSalesOrdersSapAdapter = new ResultModel();
            mockResultSalesOrdersSapAdapter.Success = true;
            mockResultSalesOrdersSapAdapter.Code = 200;
            mockResultSalesOrdersSapAdapter.Response = sapAdapterResponseSalesOrderContent;

            var mockResultProdutionOrdersSapAdapter = new ResultModel();
            mockResultProdutionOrdersSapAdapter.Success = true;
            mockResultProdutionOrdersSapAdapter.Code = 200;
            mockResultProdutionOrdersSapAdapter.Response = sapAdapteProdOrdersResponseContent;

            this.mockSapAdapter = new Mock<ISapAdapter>();
            this.mockSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail))
                .Returns(Task.FromResult(mockResultSalesOrdersSapAdapter));

            this.mockSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetFabOrdersByFilter))
                .Returns(Task.FromResult(mockResultProdutionOrdersSapAdapter));

            this.serviceLayerAdapterService = new Mock<ISapServiceLayerAdapterService>();
            this.serviceLayerAdapterService
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResultDiapi));

            var mockUserService = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();

            return new CancelPedidosService(this.mockSapAdapter.Object, this.pedidosDao, mockSapFile.Object, mockUserService.Object, this.serviceLayerAdapterService.Object);
        }

        /// <summary>
        /// ResetAvailablePiecesForCancelledOrder.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task ResetAvailablePiecesForCancelledOrder()
        {
            // arrange
            var parentOrder = new ProductionOrderSeparationModel
            {
                Id = 1,
                OrderId = 1001,
                AvailablePieces = 10,
            };
            var detailOrder = new ProductionOrderSeparationDetailModel
            {
                Id = 1,
                OrderId = 1001,
                DetailOrderId = 2001,
                AssignedPieces = 5,
            };

            this.context.ProductionOrderSeparationModel.Add(parentOrder);
            this.context.ProductionOrderSeparationDetailModel.Add(detailOrder);
            await this.context.SaveChangesAsync();

            this.pedidosDao = new PedidosDao(this.context);
            this.cancelPedidosService = this.BuildService(null, "Ok");

            var orderToReset = new OrderIdModel
            {
                UserId = this.userId,
                OrderId = 2001, // ID de la orden hija
            };

            // act
            await ((CancelPedidosService)this.cancelPedidosService)
                .ResetAvailablePiecesForCancelledOrder(orderToReset);

            // assert
            var updatedParentOrder = await this.context.ProductionOrderSeparationModel
                .FirstOrDefaultAsync(x => x.OrderId == 1001);
            var childOrder = await this.context.ProductionOrderSeparationDetailModel
                .FirstOrDefaultAsync(x => x.DetailOrderId == 2001);

            Assert.That(updatedParentOrder, Is.Not.Null);
            Assert.That(updatedParentOrder.AvailablePieces, Is.EqualTo(15)); // 10 + 5 piezas devueltas
            Assert.That(childOrder.CancelOrderUserId, Is.Not.Null);
        }

        /// <summary>
        /// ResetAvailablePiecesForCancelledOrder.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task ResetAvailablePiecesForCancelledOrderChildOrder()
        {
            // arrange
            var parentOrder = new ProductionOrderSeparationModel
            {
                Id = 1,
                OrderId = 1001,
                AvailablePieces = 10,
            };
            var detailOrder = new ProductionOrderSeparationDetailModel
            {
                Id = 1,
                OrderId = 1001,
                DetailOrderId = 2001,
                AssignedPieces = 5,
            };

            this.context.ProductionOrderSeparationModel.Add(parentOrder);
            this.context.ProductionOrderSeparationDetailModel.Add(detailOrder);
            await this.context.SaveChangesAsync();

            this.pedidosDao = new PedidosDao(this.context);
            this.cancelPedidosService = this.BuildService(null, "Ok");

            var orderToReset = new OrderIdModel
            {
                UserId = this.userId,
                OrderId = 2002, // ID de la orden hija
            };

            // act
            await ((CancelPedidosService)this.cancelPedidosService)
                .ResetAvailablePiecesForCancelledOrder(orderToReset);

            // assert
            var childOrder = await this.context.ProductionOrderSeparationDetailModel
                .FirstOrDefaultAsync(x => x.DetailOrderId == 2001);

            Assert.That(childOrder.CancelOrderUserId, Is.Null);
        }

        /// <summary>
        /// Cancel a single production order.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1050)]
        [TestCase(1058)]
        [TestCase(1065)]
        public async Task CancelFabricationOrders_AffectSingleOrder_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = orderId == 1065 ? this.BuildService(new List<OrderWithDetailModel>(), "Ok") : this.BuildService(this.GetOrderWithDetailModel(), "Ok");

            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 1, 0, 1));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders already cancelled.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1054)]
        [TestCase(1056)]
        public async Task CancelFabricationOrders_AlreadyCancelled_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetOrderWithDetailModel(), null);
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 1, 0, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel fabrication orders with finished status.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1060)]
        public async Task CancelFabricationOrders_FinishedFabricationOrder_FailedResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, null);
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel fabrication orders not found.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1000)]
        public async Task CancelFabricationOrders_NotFound_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, null, new List<string>());
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel fabrication orders missing local order.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelFabricationOrders_GetFromSap_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrder(), "Ok", this.GetSapAdapterProductionOrder());
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 1, 0, 1));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetFabOrdersByFilter), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders missing local order.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelFabricationOrders_GetIsolatedFromSap_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok", this.GetSapAdapterIsolatedProductionOrder());
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 1, 0, 1));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetFabOrdersByFilter), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders with sales order cancellation.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1055)]
        public async Task CancelFabricationOrders_AffectSalesOrder_SuccessResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetOrderWithDetailModel(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(response.Success);
            Assert.That(this.CheckAction(response, true, 1, 0, 1));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Cancel fabrication orders with sap error.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(1055)]
        public async Task CancelFabricationOrders_SapDiApiError_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetOrderWithDetailModel(), "Fail");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelFabricationOrders(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Cancel sales orders successfuly.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <param name="affectedRecords">Number of affected records.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(10057, 3)]
        public async Task CancelSalesOrder_ValidRelatedProductionOrders_SuccessResults(int orderId, int affectedRecords)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 1, 0, affectedRecords));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(affectedRecords - 1));
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(10061)]
        public async Task CancelSalesOrder_RelatedProductionOrdersFinished_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(10051)]
        public async Task CancelSalesOrder_FinishedSalesOrder_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(null, "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <param name="affectedRecords">Affected records.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997, 3)]
        public async Task CancelSalesOrder_GetFromSap_SuccessResults(int orderId, int affectedRecords)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrder(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 1, 0, affectedRecords));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(affectedRecords - 1));
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelSalesOrder_GetFromSapWithFinishedProductionOrders_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedProductionOrders(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
        }

        /// <summary>
        /// Cancel sales orders failed.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(997)]
        public async Task CancelSalesOrder_GetFromSapWithFinishedSalesOrder_FailResults(int orderId)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");
            var orderToUpdate = new List<OrderIdModel>
            {
                new OrderIdModel { UserId = this.userId, OrderId = orderId },
            };

            // act
            var response = await this.cancelPedidosService.CancelSalesOrder(orderToUpdate);

            // assert
            Assert.That(this.CheckAction(response, true, 0, 1, 0));
            this.serviceLayerAdapterService.Verify(v => v.PostAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            this.mockSapAdapter.Verify(v => v.PostSapAdapter(It.IsAny<object>(), ServiceConstants.GetOrderWithDetail), Times.Once);
        }

        /// <summary>
        /// test the cancel.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("total")]
        public async Task CancelDelivery(string type)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");
            var orderToUpdate = new List<CancelDeliveryPedidoModel>
            {
                new CancelDeliveryPedidoModel { DeliveryId = 74571, SaleOrderId = 84368, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74572, SaleOrderId = 84368, NeedsCancel = false, Status = "O" },

                new CancelDeliveryPedidoModel { DeliveryId = 74573, SaleOrderId = 84369, NeedsCancel = true, Status = "C" },

                new CancelDeliveryPedidoModel { DeliveryId = 74575, SaleOrderId = 84370, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74576, SaleOrderId = 84370, NeedsCancel = false, Status = "O" },

                new CancelDeliveryPedidoModel { DeliveryId = 74577, SaleOrderId = 84371, NeedsCancel = true, Status = "C" },
            };

            var model = new CancelDeliveryPedidoCompleteModel
            {
                CancelDelivery = orderToUpdate,
                DetallePedido = new List<DetallePedidoModel>(),
            };

            // act
            var response = await this.cancelPedidosService.CancelDelivery(type, model);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// test the cancel.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("partial")]
        public async Task CancelDeliveryPartial(string type)
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");
            var orderToUpdate = new List<CancelDeliveryPedidoModel>
            {
                new CancelDeliveryPedidoModel { DeliveryId = 74578, SaleOrderId = 84372, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74579, SaleOrderId = 84372, NeedsCancel = false, Status = "O" },
                new CancelDeliveryPedidoModel { DeliveryId = 74580, SaleOrderId = 84373, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74581, SaleOrderId = 84374, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74582, SaleOrderId = 84375, NeedsCancel = false, Status = "O" },
                new CancelDeliveryPedidoModel { DeliveryId = 74583, SaleOrderId = 84375, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74584, SaleOrderId = 84376, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74585, SaleOrderId = 84377, NeedsCancel = true, Status = "C" },
                new CancelDeliveryPedidoModel { DeliveryId = 74586, SaleOrderId = 84377, NeedsCancel = false, Status = "O" },
            };

            var model = new CancelDeliveryPedidoCompleteModel
            {
                CancelDelivery = orderToUpdate,
                DetallePedido = new List<DetallePedidoModel>(),
            };

            // act
            var response = await this.cancelPedidosService.CancelDelivery(type, model);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// test the cancel.
        /// </summary>
        /// <param name="deliveryId">the delivery id.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(150158)]
        [TestCase(150160)]
        public async Task CancelTotalInfo(int deliveryId)
        {
            List<int> deliverys = new List<int> { deliveryId };

            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");

            // act
            var response = await this.cancelPedidosService.CancelTotalInfo(deliverys);

            // assert
            Assert.That(response, Is.Not.Null);

            if (deliveryId == 150158)
            {
                Assert.That(response.Response, Is.True);
            }
            else
            {
                Assert.That(response.Response, Is.False);
            }
        }

        /// <summary>
        /// Method to verify Get All Almacens.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase("REVE 14")]
        [TestCase("REVE 16")]
        public async Task CancelPackaging(string itemCode)
        {
            // arrange
            var request = new CancelPackagingDto { ItemCode = itemCode, DeliveryId = 15800, InvoiceId = 15700 };

            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");

            // act
            var result = await this.cancelPedidosService.CancelPackaging(request);

            // assert
            if (itemCode == "REVE 14")
            {
                Assert.That(result.Code == 200, Is.True);
            }
            else
            {
                Assert.That(result.Code == 400, Is.True);
                Assert.That(result.Comments as string == "No existe un registro que coincidan con estos datos ItemCode: REVE 16, DeliveryId: 15800, InvoiceId: 15700 o el producto ya se encuentra en distribución", Is.True);
            }
        }

        /// <summary>
        /// test the cancel.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CleanInvoices()
        {
            // arrange
            this.cancelPedidosService = this.BuildService(this.GetSapAdapterOrderWithFinishedSalesOrder(), "Ok");
            var orderToUpdate = new List<int> { 100 };

            // act
            var response = await this.cancelPedidosService.CleanInvoices(orderToUpdate);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        private static DbContextOptions<DatabaseContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh.
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseInMemoryDatabase("TemporalCancelPedidos")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        /// <summary>
        /// Gets sap adapter order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<OrderWithDetailModel> GetSapAdapterOrder()
        {
            var detalle = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { OrdenFabricacionId = 998, Status = "P" },
                new CompleteDetailOrderModel { OrdenFabricacionId = 999, Status = "P" },
            };

            var orders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(detalle),
                    Order = new OrderModel { DocNum = 997, PedidoId = 997, PedidoStatus = "L" },
                },
            };
            return orders;
        }

        /// <summary>
        /// Gets sap adapter production order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<FabricacionOrderModel> GetSapAdapterProductionOrder()
        {
            var orders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel
                {
                    PedidoId = 997,
                    OrdenId = 998,
                    Status = "P",
                },
            };
            return orders;
        }

        /// <summary>
        /// Gets sap adapter isolated production order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<FabricacionOrderModel> GetSapAdapterIsolatedProductionOrder()
        {
            var orders = this.GetSapAdapterProductionOrder();
            orders[0].PedidoId = null;
            return orders;
        }

        /// <summary>
        /// Gets sap adapter order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<OrderWithDetailModel> GetSapAdapterOrderWithFinishedProductionOrders()
        {
            var orders = this.GetSapAdapterOrder();
            orders[0].Detalle[0].Status = "L";
            return orders;
        }

        /// <summary>
        /// Gets sap adapter order.
        /// </summary>
        /// <returns>Sap order as json string.</returns>
        private List<OrderWithDetailModel> GetSapAdapterOrderWithFinishedSalesOrder()
        {
            var orders = this.GetSapAdapterOrder();
            orders[0].Order.PedidoStatus = "C";
            return orders;
        }

        /// <summary>
        /// Check response results.
        /// </summary>
        /// <param name="result">Result.</param>
        /// <param name="success">Expected success.</param>
        /// <param name="numberOfSucceess">Expected success results.</param>
        /// <param name="numberOfFails">Expected fail results.</param>
        /// <param name="numberOfLogs">Expected logs.</param>
        /// <returns>Validation flag.</returns>
        private bool CheckAction(ResultModel result, bool success, int numberOfSucceess, int numberOfFails, int numberOfLogs)
        {
            var content = (SuccessFailResults<OrderIdModel>)result.Response;

            return result.Success.Equals(success) &&
                    numberOfFails.Equals(content.Failed.Count) &&
                    numberOfSucceess.Equals(content.Success.Count);
        }
    }
}
