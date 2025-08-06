// <summary>
// <copyright file="PedidoServiceTest.cs" company="Axity">
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
    public class PedidoServiceTest : BaseTest
    {
        private IPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private Mock<IUsersService> usersService;

        private DatabaseContext context;

        private Mock<IConfiguration> configuration;

        private Mock<IReportingService> reportingService;

        private Mock<IRedisService> redisService;

        private Mock<IKafkaConnector> kafkaConnector;

        private Mock<ISapServiceLayerAdapterService> sapServiceLayerService;

        private Mock<IProductionOrdersService> productionOrdersService;

        private Mock<ILogger> logger;

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
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.ProductionOrderSeparationModel.AddRange(this.GetProductionOrderSeparation());
            this.context.SaveChanges();

            this.reportingService = new Mock<IReportingService>();
            this.sapAdapter = new Mock<ISapAdapter>();
            this.sapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            this.kafkaConnector = new Mock<IKafkaConnector>();
            this.kafkaConnector
                .Setup(m => m.PushMessage(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var mockSapFile = new Mock<ISapFileService>();
            this.redisService = new Mock<IRedisService>();

            this.sapServiceLayerService = new Mock<ISapServiceLayerAdapterService>();
            this.sapServiceLayerService
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            this.productionOrdersService = new Mock<IProductionOrdersService>();

            this.logger = new Mock<ILogger>();

            this.configuration = new Mock<IConfiguration>();
            this.configuration.SetupGet(x => x[It.Is<string>(s => s == "OmicronFilesAddress")]).Returns("http://localhost:5002/");

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderBySalesOrder()
        {
            // arrange
            var listIds = new List<int> { 1, 2, 3 };

            // act
            var response = await this.pedidosService.GetUserOrderBySalesOrder(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderByFabOrder()
        {
            // arrange
            var listIds = new List<int> { 100, 101, 102 };

            // act
            var response = await this.pedidosService.GetUserOrderByFabOrder(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetFabOrderByUserID()
        {
            // arrange
            var id = "abc";

            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListFormulaDetalle()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserId(id);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return fabOrder.</returns>
        [Test]
        public async Task GetFabOrderByUserIdOrdersPaternt()
        {
            // arrange
            var id = "1a663b91-fffa-4298-80c3-aaae35586dc6";

            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListFormulaDetalleForOrdersParent()));

            var mockUsuarios = new Mock<IUsersService>();

            mockUsuarios
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUser()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsuarios.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserId(id);
            var result = response.Response as QfbOrderModel;

            var procesoStatus = result.Status.FirstOrDefault(s => s.StatusName == "En Proceso");
            var orderInEnProceso = procesoStatus.Orders.Any(order => order.ProductionOrderId == 225306);
            var reasignadoStatus = result.Status.FirstOrDefault(s => s.StatusName == "Reasignado");
            var orderInReasignado = reasignadoStatus.Orders.Any(order => order.ProductionOrderId == 225305);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(procesoStatus, Is.Not.Null);
            Assert.That(reasignadoStatus, Is.Not.Null);
            Assert.That(procesoStatus.Orders.Count, Is.EqualTo(1));
            Assert.That(reasignadoStatus.Orders.Count, Is.EqualTo(1));
            Assert.That(orderInEnProceso, Is.True);
            Assert.That(orderInReasignado, Is.True);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return fabOrder.</returns>
        [Test]
        public async Task GetFabOrderByUserIdOrdersPaterntOrder()
        {
            // arrange
            var id = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a";

            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListFormulaDetalleForOrdersParent()));

            var mockUsuarios = new Mock<IUsersService>();

            mockUsuarios
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUser()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsuarios.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserId(id);
            var result = response.Response as QfbOrderModel;

            var procesoStatus = result.Status.FirstOrDefault(s => s.StatusName == "En Proceso");

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(procesoStatus, Is.Not.Null);
            Assert.That(procesoStatus.Orders.Count, Is.EqualTo(5));
            Assert.That(procesoStatus.Orders[0].ProductionOrderId, Is.EqualTo(225309));
            Assert.That(procesoStatus.Orders[1].ProductionOrderId, Is.EqualTo(225310));
            Assert.That(procesoStatus.Orders[2].ProductionOrderId, Is.EqualTo(225311));
            Assert.That(procesoStatus.Orders[3].ProductionOrderId, Is.EqualTo(225312));
            Assert.That(procesoStatus.Orders[4].ProductionOrderId, Is.EqualTo(225313));
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetFabOrderByTcnicID()
        {
            // arrange
            var id = "tecnic";

            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListFormulaDetalle()));

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel(true)));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var mockSapFile = new Mock<ISapFileService>();

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserId(id);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrdersByUserId()
        {
            // arrange
            var id = new List<string> { "abc" };

            // act
            var response = await this.pedidosService.GetUserOrdersByUserId(id);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="iduser">User Id.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase("abcquimico")]
        [TestCase("abcquimicocd")]
        [TestCase("tecnicoqfb")]
        [TestCase("tecnicoqfb2")]
        public async Task GetQfbOrdersByStatus(string iduser)
        {
            // arrange
            var status = "Asignado";

            // act
            var response = await this.pedidosService.GetQfbOrdersByStatus(status, iduser);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateComponents()
        {
            // arrange
            var components = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { Available = 1, BaseQuantity = 1, Consumed = 1, Description = "Des", OrderFabId = 2, PendingQuantity = 1, ProductId = "Aspirina", RequiredQuantity = 1, Stock = 1, Unit = "Unit", Warehouse = "wh", WarehouseQuantity = 1 },
            };

            var asignar = new UpdateFormulaModel
            {
                Comments = "Comments",
                Components = components,
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
            };

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();

            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var listResult = new List<ProductUnitDto> { new ProductUnitDto() { ProductoId = "Aspirina", Id = 1 } };
            var result = new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listResult),
                Success = true,
                UserError = string.Empty,
            };
            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), ServiceConstants.ProductUnit))
                .Returns(Task.FromResult(result));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();

            var pedidosServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, serviceLayer.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosServiceLocal.UpdateComponents(asignar);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="userRoleType">User role type.</param>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase(2, false)]
        [TestCase(2, true)]
        [TestCase(9, true)]
        public async Task UpdateUserOrderStatus(int userRoleType, bool isValidtecnic)
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 100, Status = "Proceso", UserRoleType = userRoleType },
            };

            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));
            var mockSaDiApi = new Mock<ISapDiApi>();

            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Comments, Is.Null);

            if (isValidtecnic)
            {
                Assert.That(response.UserError, Is.Null);
                Assert.That(response.Success);
                Assert.That(response.Code.Equals(200));
                Assert.That(response.Response, Is.Not.Null);
            }
            else
            {
                Assert.That(response.UserError, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Code.Equals(400));
                Assert.That(response.Response, Is.Null);
            }
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="userRoleType">User role type.</param>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase(2, false)]
        [TestCase(2, true)]
        [TestCase(9, true)]
        public async Task UpdateUserOrderStatusEntregado(int userRoleType, bool isValidtecnic)
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 301, Status = "Entregado", UserRoleType = userRoleType },
            };

            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Comments, Is.Null);

            if (isValidtecnic)
            {
                Assert.That(response.UserError, Is.Null);
                Assert.That(response.Success);
                Assert.That(response.Code.Equals(200));
                Assert.That(response.Response, Is.Not.Null);
            }
            else
            {
                Assert.That(response.UserError, Is.Not.Null);
                Assert.That(response.Success, Is.False);
                Assert.That(response.Code.Equals(400));
                Assert.That(response.Response, Is.Null);
            }
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateUserOrderStatusEntregadoAndAddNotCompletSalesLogs()
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 501, Status = "Entregado" },
            };

            // act
            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateUserOrderStatusEntregadoAndAddCompletSalesLogs()
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 501, Status = "Entregado" },
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 502, Status = "Entregado" },
            };

            // act
            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Update order signatures.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task UpdateOrderSignature()
        {
            // arrange
            var imageData = File.ReadAllText("SignatureBase64.txt");

            var signatures = new UpdateOrderSignatureModel
            {
                UserId = "abc",
                FabricationOrderId = 101,
                Signature = imageData,
            };

            // act
            var response = await this.pedidosService.UpdateOrderSignature(SignatureType.LOGISTICS, signatures);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// Update fabrication order comments.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(103)]
        public async Task UpdateFabOrderComments(int orderId)
        {
            // arrange
            var userId = "abc";

            var orderToUpdate = new List<UpdateOrderCommentsModel>
            {
                new UpdateOrderCommentsModel { UserId = userId, OrderId = orderId, Comments = "Hello" },
            };

            // act
            var response = await this.pedidosService.UpdateFabOrderComments(orderToUpdate);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// Get order signatures.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetOrderSignatures()
        {
            // arrange
            int productionOrderId = 102;

            // act
            var response = await this.pedidosService.GetOrderSignatures(productionOrderId);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateBatches()
        {
            // arrange
            var update = new AssignBatchModel
            {
                Action = "insert",
                AssignedQty = 10,
                BatchNumber = "ABC",
                ItemCode = "102",
                OrderId = 10,
            };

            var mockSapServiceLayer = new Mock<ISapServiceLayerAdapterService>();

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultAssignBatch()));

            mockSapServiceLayer
                .Setup(m => m.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultAssignBatch()));

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, mockSapServiceLayer.Object, mockSaDiApiLocal.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.UpdateBatches(new List<AssignBatchModel> { update });

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="qfbSignature">Qfb signature.</param>
        /// <param name="technicalSignature">Technical Signature.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase("QXhpdHkyMDIw", null)]
        [TestCase("QXhpdHkyMDIw", "QXhpdHkyMDIw")]
        public async Task FinishOrder(string qfbSignature, string technicalSignature)
        {
            // arrange
            var update = new FinishOrderModel
            {
                FabricationOrderId = new List<int> { 100 },
                TechnicalSignature = technicalSignature,
                QfbSignature = qfbSignature,
                UserId = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.FinishOrder(update);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task FinishOrderNewSignature()
        {
            // arrange
            var update = new FinishOrderModel
            {
                FabricationOrderId = new List<int> { 101 },
                TechnicalSignature = "QXhpdHkyMDIw",
                QfbSignature = "QXhpdHkyMDIw",
                UserId = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.FinishOrder(update);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        [Test]
        public void CompletedBatchesError()
        {
            // arrange
            var orderId = 101;
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetMissingBatches()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.CompletedBatches(orderId));
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CloseSalesOrders()
        {
            // arrange
            var salesOrders = new List<OrderIdModel>
            {
                new OrderIdModel { OrderId = 104, UserId = "abc", },
            };
            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockServiceLayerLocal = new Mock<ISapServiceLayerAdapterService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var componentsInvalids = new List<CompleteDetalleFormulaModel>()
            {
                new CompleteDetalleFormulaModel { OrderFabId = 107, Consumed = 2, RequiredQuantity = 2 },
            };
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GenerateResultModel(componentsInvalids)))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockServiceLayerLocal
                .Setup(m => m.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var mockProductionOrders = new Mock<IProductionOrdersService>();

            mockProductionOrders
                .Setup(x => x.FinalizeProductionOrdersAsync(It.IsAny<List<FinalizeProductionOrderModel>>()))
                .Returns(Task.FromResult(this.GetResultFinalizeProductionOrdersAsync()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, mockServiceLayerLocal.Object, mockSaDiApi.Object, mockProductionOrders.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.CloseSalesOrders(salesOrders);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.InstanceOf<FinalizeProductionOrdersResult>());
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task RejectSalesOrders()
        {
            // arrange
            var salesOrders = new RejectOrdersModel();
            salesOrders.Comments = "comentatios";
            salesOrders.OrdersId = new List<int>
            {
                123,
                456,
            };

            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockSapAdapter = new Mock<ISapAdapter>();

            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.RejectSalesOrders(salesOrders);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CloseSalesOrders_SalesOrderWithPreProductionOrders_FailedResult()
        {
            // arrange
            var salesOrders = new List<OrderIdModel>
            {
                new OrderIdModel { OrderId = 104, UserId = "abc", },
            };

            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockUsers = new Mock<IUsersService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            var mockServiceLayerLocal = new Mock<ISapServiceLayerAdapterService>();
            mockServiceLayerLocal
                .Setup(m => m.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var result = this.GetResultModelCompleteDetailModel();
            var responseOrders = (List<OrderWithDetailModel>)result.Response;
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 101, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = null });
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 102, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = string.Empty });
            result.Response = responseOrders;

            var componentsInvalids = new List<CompleteDetalleFormulaModel>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GenerateResultModel(componentsInvalids)))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockProductionOrders = new Mock<IProductionOrdersService>();

            mockProductionOrders
                .Setup(x => x.FinalizeProductionOrdersAsync(It.IsAny<List<FinalizeProductionOrderModel>>()))
                .Returns(Task.FromResult(this.GetResultFinalizeProductionOrdersAsync()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, mockServiceLayerLocal.Object, mockSaDiApi.Object, mockProductionOrders.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.CloseSalesOrders(salesOrders);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.InstanceOf<FinalizeProductionOrdersResult>());
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CloseFabOrders()
        {
            // arrange
            var salesOrders = new List<CloseProductionOrderModel>
            {
                new CloseProductionOrderModel { OrderId = 107, UserId = "abc", Batches = new List<BatchesConfigurationModel>() },
            };

            var mockSapServiceLayer = new Mock<ISapServiceLayerAdapterService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockSapServiceLayer
                .Setup(m => m.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var result = this.GetResultModelCompleteDetailModel();
            var responseOrders = (List<OrderWithDetailModel>)result.Response;
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 101, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = null });
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 102, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = string.Empty });
            result.Response = responseOrders;

            var componentsInvalids = new List<CompleteDetalleFormulaModel>()
            {
                new CompleteDetalleFormulaModel { OrderFabId = 107, Consumed = 2, RequiredQuantity = 2 },
            };
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(result))
                .Returns(Task.FromResult(this.GenerateResultModel(componentsInvalids)))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockProductionOrders = new Mock<IProductionOrdersService>();

            mockProductionOrders
                .Setup(x => x.FinalizeProductionOrdersAsync(It.IsAny<List<FinalizeProductionOrderModel>>()))
                .Returns(Task.FromResult(this.GetResultFinalizeProductionOrdersAsync()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, mockSapServiceLayer.Object, mockSaDiApi.Object, mockProductionOrders.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.CloseFabOrders(salesOrders);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.InstanceOf<FinalizeProductionOrdersResult>());
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateIsolatedProductionOrder()
        {
            // arrange
            var order = new CreateIsolatedFabOrderModel { ProductCode = "EN-1234", UserId = "abc", };

            var mockContent = new KeyValuePair<string, string>("token", "Ok");
            var serviceLayerLocal = new Mock<ISapServiceLayerAdapterService>();
            var mockUsers = new Mock<IUsersService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockResultDiApi = new ResultModel();
            mockResultDiApi.Success = true;
            mockResultDiApi.Code = 200;
            mockResultDiApi.Response = JsonConvert.SerializeObject(mockContent);

            var mockResultSapAdapter = new ResultModel();
            mockResultSapAdapter.Success = true;
            mockResultSapAdapter.Code = 200;
            mockResultSapAdapter.Response = "12345";

            serviceLayerLocal
                .Setup(m => m.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResultDiApi));

            mockSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(mockResultSapAdapter));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, serviceLayerLocal.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.CreateIsolatedProductionOrder(order);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByDocNum()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, "100-100" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dic);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByQfb()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Qfb, "abc" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dic);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByStatus()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Status, "Asignado" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dic);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByDate()
        {
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dateFinal) },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dicParams);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersAllFilters()
        {
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dateFinal) },
                { ServiceConstants.Status, "Asignado" },
                { ServiceConstants.Qfb, "abc" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dicParams);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersAllClassifications()
        {
            var dates = new DateTime(2025, 05, 28).ToString("dd/MM/yyyy");
            var dateFinal = new DateTime(2025, 05, 30).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                { ServiceConstants.Classifications, "Todas" },
            };

            var localSapAdapter = new Mock<ISapAdapter>();
            var mockUsers = new Mock<IUsersService>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFabricacionOrderModelClassifications()));

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserModel()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var localService = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await localService.GetFabOrders(dicParams);
            var response = result.Response as List<CompleteOrderModel>;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(response.Count, Is.EqualTo(4));
            Assert.That(response.Any(x => x.FabOrderId == 226274), Is.True);
            Assert.That(response.Any(x => x.FabOrderId == 226275), Is.True);
            Assert.That(response.Any(x => x.FabOrderId == 226276), Is.True);
            Assert.That(response.Any(x => x.FabOrderId == 226277), Is.True);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersSomeClassifications()
        {
            var dates = new DateTime(2025, 05, 28).ToString("dd/MM/yyyy");
            var dateFinal = new DateTime(2025, 05, 30).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                { ServiceConstants.Classifications, "MG" },
            };

            var localSapAdapter = new Mock<ISapAdapter>();
            var mockUsers = new Mock<IUsersService>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFabricacionOrderModelClassifications()));

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserModel()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var localService = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await localService.GetFabOrders(dicParams);
            var response = result.Response as List<CompleteOrderModel>;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(response.Count, Is.EqualTo(2));
            Assert.That(response.Any(x => x.FabOrderId == 226274), Is.True);
            Assert.That(response.Any(x => x.FabOrderId == 226277), Is.True);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CompletedBatches()
        {
            var orderId = 200;
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));

            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await pedidoServiceLocal.CompletedBatches(orderId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task PrintOrders()
        {
            var orderId = new List<int> { 100 };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await pedidoServiceLocal.PrintOrders(orderId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task PrintOrdersFabOrders()
        {
            var orderId = new List<int> { 100 };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFabricacionOrderModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            // act
            var result = await SendToGeneratePdfUtils.CreateModelGeneratePdf(new List<int>(), orderId, mockSapAdapter.Object, this.pedidosDao, mockSapFile.Object, mockUsers.Object, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateSaleOrders()
        {
            var orderId = new UpdateOrderCommentsModel
            {
                OrderId = 100,
                Comments = "Comments",
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await pedidoServiceLocal.UpdateSaleOrders(orderId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateDesignerLabel()
        {
            var details = new List<UpdateDesignerLabelDetailModel>
            {
                new UpdateDesignerLabelDetailModel { OrderId = 100, Checked = true },
                new UpdateDesignerLabelDetailModel { OrderId = 200, Checked = true },
            };

            var orderId = new UpdateDesignerLabelModel
            {
                DesignerSignature = "aG9sYQ==",
                UserId = "abc",
                Details = details,
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await pedidoServiceLocal.UpdateDesignerLabel(orderId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateSaleOrderPdf()
        {
            var order = new CreateOrderPdfDto { OrderId = 100, ClientType = "general" };
            var details = new List<CreateOrderPdfDto> { order };

            var dictResponse = new Dictionary<string, string>
            {
                { "100", "Ok-C:\\algo" },
            };

            var response = new ResultModel
            {
                Code = 200,
                Success = true,
                Response = JsonConvert.SerializeObject(dictResponse),
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var result = await pedidoServiceLocal.CreateSaleOrderPdf(details);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task SignOrdersByTecnic()
        {
            // arrange
            var update = new FinishOrderModel
            {
                FabricationOrderId = new List<int> { 100, 200 },
                TechnicalSignature = "QXhpdHkyMDIw",
                UserId = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.SignOrdersByTecnic(update);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="productionOrderId">Production Order Id.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase("223740")]
        [TestCase("224212")]
        [TestCase("224211")]
        [TestCase("224159")]
        public async Task GetInvalidOrdersByMissingTecnicSign(string productionOrderId)
        {
            // arrange
            var productionOrderIds = new List<string>
            {
                productionOrderId,
            };

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();

            var mockSaDiApi = new Mock<ISapDiApi>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, mockSaDiApi.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedidoServiceLocal.GetInvalidOrdersByMissingTecnicSign(productionOrderIds);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Success);
            Assert.That(response.Code.Equals(200));

            if (productionOrderId.Equals("224212") || productionOrderId.Equals("224159"))
            {
                Assert.That(response.Response, Is.Not.Null);
            }
            else
            {
                Assert.That(response.Response, Is.EqualTo(new List<string>()));
            }
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrdersByInvoiceId()
        {
            // arrange
            List<int> invoicesid = new List<int> { 1, 2 };
            string type = "local";

            var users = new Mock<IUsersService>();
            var sapAdapter = new Mock<ISapAdapter>();
            var sapFile = new Mock<ISapFileService>();

            var sap = new Mock<ISapDiApi>();
            var pedido = new PedidosService(sapAdapter.Object, this.pedidosDao, users.Object, sapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object, this.sapServiceLayerService.Object, sap.Object, this.productionOrdersService.Object, this.logger.Object);

            // act
            var response = await pedido.GetUserOrdersByInvoiceId(invoicesid, type);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Success);
            Assert.That(response.Code.Equals(200));
        }
    }
}
