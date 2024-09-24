// <summary>
// <copyright file="AlmacenOrderDoctorServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AlmacenOrderDoctorServiceTest : BaseTest
    {
        private IAlmacenOrderDoctorService almacenOrderDoctorService;

        private ISapDao sapDao;

        private DatabaseContext context;

        private Mock<ICatalogsService> catalogService;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacenOrdersService")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.ProductoModel.AddRange(this.GetProductoModelForDoctorOrders());
            this.context.OrderModel.AddRange(this.GetOrderModelForDoctorOrders());
            this.context.DetallePedido.AddRange(this.GetDetallePedidoForDoctorOrders());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModelForDoctorOrders());
            this.context.SaveChanges();

            var mockPedidoService = new Mock<IPedidosService>();
            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidosForDoctorOrders()));

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidosForDoctorOrders()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsForDoctorOrders()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            this.catalogService = new Mock<ICatalogsService>();
            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(parameters)));

            this.sapDao = new SapDao(this.context, mockLog.Object);

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };
            var mockProccesspayments = new Mock<IProccessPayments>();
            mockProccesspayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));

            this.almacenOrderDoctorService = new AlmacenOrderDoctorService(this.sapDao, mockPedidoService.Object, mockAlmacen.Object, this.catalogService.Object, mockRedis.Object, mockProccesspayments.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="type">the type of orders.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("magistral, mixto, maquila, linea, omigenomics")]
        [TestCase("magistral, mixto, linea")]
        public async Task SearchAlmacenOrdersByAllsDoctors(string type)
        {
            // arrange
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, type },
            };

            // act
            var response = await this.almacenOrderDoctorService.SearchAlmacenOrdersByDoctor(dictionary);

            // assert
            ClassicAssert.IsNotNull(response);
            ClassicAssert.IsNotNull(response.Comments);
            ClassicAssert.AreEqual(200, response.Code);
            ClassicAssert.IsInstanceOf<AlmacenOrdersByDoctorModel>(response.Response);

            var data = (AlmacenOrdersByDoctorModel)response.Response;
            ClassicAssert.IsTrue(data.SalesOrders.Any());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersByDoctor()
        {
            // arrange
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Chips, "alias" },
                { ServiceConstants.Shipping, "Foraneo" },
            };

            var localNeighBors = new List<ParametersModel>
            {
                new ParametersModel { Value = "Nuevo León", Field = "LocalNeighborhood" },
            };

            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(localNeighBors)));

            // act
            var response = await this.almacenOrderDoctorService.SearchAlmacenOrdersByDoctor(dictionary);

            // assert
            ClassicAssert.IsNotNull(response);
            ClassicAssert.IsNotNull(response.Comments);
            ClassicAssert.AreEqual(200, response.Code);
            ClassicAssert.IsInstanceOf<AlmacenOrdersByDoctorModel>(response.Response);

            var data = (AlmacenOrdersByDoctorModel)response.Response;
            ClassicAssert.IsTrue(data.SalesOrders.Any());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersDetailsByDoctor()
        {
            // arrange
            var request = new DoctorOrdersSearchDeatilDto
            {
                Address = "CDMX",
                Name = "alias",
                SaleOrders = new List<int>
                {
                    84503,
                    84517,
                    84515,
                },
            };

            var localNeighBors = new List<ParametersModel>
            {
                new ParametersModel { Value = "Nuevo León", Field = "LocalNeighborhood" },
            };

            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(localNeighBors)));

            // act
            var response = await this.almacenOrderDoctorService.SearchAlmacenOrdersDetailsByDoctor(request);

            // assert
            ClassicAssert.IsNotNull(response);
            ClassicAssert.AreEqual(200, response.Code);
            ClassicAssert.IsNotNull(response.Response);
            ClassicAssert.IsInstanceOf<SalesByDoctorModel>(response.Response);

            var data = (SalesByDoctorModel)response.Response;
            ClassicAssert.IsNotNull(data.AlmacenHeaderByDoctor);
            ClassicAssert.IsTrue(data.Items.Any());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrderdetail()
        {
            // arrange
            var salesOrderId = 84515;

            var mockPedido = new Mock<IPedidosService>();
            mockPedido
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidosForDoctorOrders()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            var mockProccesspayments = new Mock<IProccessPayments>();
            var localService = new AlmacenOrderDoctorService(this.sapDao, mockPedido.Object, mockAlmacen.Object, this.catalogService.Object, mockRedis.Object, mockProccesspayments.Object);

            // act
            var response = await localService.GetOrderdetail(salesOrderId);

            // assert
            ClassicAssert.IsNotNull(response);
        }
    }
}
