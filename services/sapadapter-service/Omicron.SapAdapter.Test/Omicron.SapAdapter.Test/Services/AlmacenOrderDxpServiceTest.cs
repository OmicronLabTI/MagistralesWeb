// <summary>
// <copyright file="AlmacenOrderDxpServiceTest.cs" company="Axity">
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
    public class AlmacenOrderDxpServiceTest : BaseTest
    {
        private IAlmacenOrderDxpService almacenOrderDxpService;

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
                .UseInMemoryDatabase(databaseName: "TemporalAlmacenDxpService")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.ProductoModel.AddRange(this.GetProductoModelForDoctorOrders());
            this.context.OrderModel.AddRange(this.GetOrderModelForDoctorOrders());
            this.context.DetallePedido.AddRange(this.GetDetallePedidoForDoctorOrders());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModelForDoctorOrders());
            this.context.LblContainerModel.AddRange(this.GetClassificationsCatalog());
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

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

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
            this.almacenOrderDxpService = new AlmacenOrderDxpService(this.sapDao, mockPedidoService.Object, mockAlmacen.Object, this.catalogService.Object, mockRedis.Object, mockProccesspayments.Object, mockDoctor.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="type">the type of orders.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("BE,MG,MX,BQ,LN, mixto, maquila, omigenomics")]
        [TestCase("BE,MG,MX,BQ,LN,linea")]
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
            var response = await this.almacenOrderDxpService.SearchAlmacenOrdersByDxpId(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Comments, Is.Not.Null);
            Assert.That(response.Code.Equals(200));
            Assert.That(response.Response, Is.InstanceOf<AlmacenOrdersByDoctorModel>());
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
                { ServiceConstants.Chips, "#1234A2" },
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
            var response = await this.almacenOrderDxpService.SearchAlmacenOrdersByDxpId(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Comments, Is.Not.Null);
            Assert.That(response.Code.Equals(200));
            Assert.That(response.Response, Is.InstanceOf<AlmacenOrdersByDoctorModel>());
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
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(localNeighBors)));

            // act
            var response = await this.almacenOrderDxpService.SearchAlmacenOrdersDetailsByDxpId(request);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Code.Equals(200));
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response, Is.InstanceOf<SalesByDoctorModel>());

            var data = (SalesByDoctorModel)response.Response;
            Assert.That(data.AlmacenHeaderByDoctor, Is.Not.Null);
            Assert.That(data.Items.Any());
        }
    }
}
