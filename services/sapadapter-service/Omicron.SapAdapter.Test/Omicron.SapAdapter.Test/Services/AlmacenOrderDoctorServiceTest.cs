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
        private ISapDao sapDao;

        private DatabaseContext context;

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

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsForDoctorOrders()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
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
            var mockAlmacen = new Mock<IAlmacenService>();
            var mockRedis = new Mock<IRedisService>();
            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));
            var localService = new AlmacenOrderDoctorService(this.sapDao, mockAlmacen.Object, mockPedidos.Object, mockCatalogos.Object, mockRedis.Object);

            // act
            var response = await localService.GetOrderdetail(salesOrderId);

            // assert
            Assert.That(response, Is.Not.Null);
        }
    }
}
