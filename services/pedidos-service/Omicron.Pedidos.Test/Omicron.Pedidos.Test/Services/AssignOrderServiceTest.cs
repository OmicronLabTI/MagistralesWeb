// <summary>
// <copyright file="AssignOrderServiceTest.cs" company="Axity">
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
    public class AssignOrderServiceTest : BaseTest
    {
        private IAssignPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private Mock<IUsersService> usersService;

        private Mock<IKafkaConnector> kafkaConnector;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal2")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            this.sapAdapter = new Mock<ISapAdapter>();
            this.sapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            this.kafkaConnector = new Mock<IKafkaConnector>();
            this.kafkaConnector
                .Setup(m => m.PushMessage(It.IsAny<object>()))
                .Returns(Task.FromResult(true));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();

            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new AssignPedidosService(this.sapAdapter.Object, this.pedidosDao, this.usersService.Object, this.kafkaConnector.Object, serviceLayer.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task AssignOrderPedido(bool isValidtecnic)
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1502 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockUsers = new Mock<IUsersService>();
            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();

            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListCompleteDetailOrderModel()));

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            var pedidosServiceLocal = new AssignPedidosService(this.sapAdapter.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.Comments, Is.Null);

            if (isValidtecnic)
            {
                Assert.That(response.UserError, Is.Not.Null);
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
        public async Task AssignOrder()
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var pedidosServiceLocal = new AssignPedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            // ssert.IsNotNull(response);
            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.UserError, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code.Equals(200));
            Assert.That(response.Response, Is.Not.Null);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        [Test]
        public void AutomaticAssign()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 100, 101 },
                UserLogistic = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign));
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        /// <returns>the reutn.</returns>
        [Test]
        public async Task AutomaticAssignDZ()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 902 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRoleWithDZ(false)));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 1", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle901 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 2", IsOmigenomics = true, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 901, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle902 = new CompleteDetailOrderModel { CodigoProducto = "567 120 ML", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 902, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "P", OrderType = "MG", DocNumDxp = "A1" };
            var order902 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 902, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 902, PedidoStatus = "L", OrderType = "MG", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900, detalle902 },
                    Order = order900,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 902, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);
            var result = await pedidoServiceLocal.AutomaticAssign(assign);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success);
            Assert.That(result.Code.Equals(200));
            Assert.That(result.ExceptionMessage, Is.Null);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        [Test]
        public void AutomaticAssignTecnicError()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 902 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRoleWithDZ(true, null)));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 1", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle901 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 2", IsOmigenomics = true, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 901, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle902 = new CompleteDetailOrderModel { CodigoProducto = "567 120 ML", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 902, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "P", OrderType = "MG", DocNumDxp = "A1" };
            var order902 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 902, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 902, PedidoStatus = "L", OrderType = "MG", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900, detalle902 },
                    Order = order900,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 902, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign));
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        /// <returns>the reutn.</returns>
        [Test]
        public async Task AutomaticAssignOnlyDZ()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 901 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRoleWithDZ()));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 1", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "P", OrderType = "MG", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900 },
                    Order = order900,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 901, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);
            var result = await pedidoServiceLocal.AutomaticAssign(assign);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success);
            Assert.That(result.Code.Equals(200));
            Assert.That(result.ExceptionMessage, Is.Null);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        [Test]
        public void AutomaticAssignDZError()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 100, 101 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailDZModel()));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign), ServiceConstants.ErrorUsersDZAutomatico);
        }

        /// <summary>
        ///  The rest.
        /// </summary>
        /// <returns>the reutn.</returns>
        [Test]
        public async Task AutomaticAssignDXP()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 901 },
                UserLogistic = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "L", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle901 = new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 901, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "L", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "L", OrderType = "MN", DocNumDxp = "A1" };
            var order901 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 901, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 901, PedidoStatus = "L", OrderType = "MN", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900 },
                    Order = order900,
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle901 },
                    Order = order901,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 901, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);

            // act
            var result = await pedidoServiceLocal.AutomaticAssign(assign);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success);
            Assert.That(result.Code.Equals(200));
            Assert.That(result.ExceptionMessage, Is.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task ReassignOrder(bool isValidtecnic)
        {
            var reassign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1, 2, 3 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abc",
            };

            var sapAdapterLocal = new Mock<ISapAdapter>();
            var mockUsers = new Mock<IUsersService>();
            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            // act
            var assignPedidosService = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);
            var result = await assignPedidosService.ReassignOrder(reassign);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
            Assert.That(result.Response, Is.Null);
            Assert.That(result.Comments, Is.Null);

            if (isValidtecnic)
            {
                Assert.That(result.UserError, Is.Null);
                Assert.That(result.Success);
                Assert.That(result.Code.Equals(200));
            }
            else
            {
                Assert.That(result.UserError, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code.Equals(400));
            }
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task ReassignOrderOrden()
        {
            var reassign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1502 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abc",
            };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var serviceLayer = new Mock<ISapServiceLayerAdapterService>();
            serviceLayer
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var pedidosServiceLocal = new AssignPedidosService(mockSapAdapter.Object, this.pedidosDao, mockUsers.Object, this.kafkaConnector.Object, serviceLayer.Object);

            // act
            var result = await pedidosServiceLocal.ReassignOrder(reassign);

            // assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
