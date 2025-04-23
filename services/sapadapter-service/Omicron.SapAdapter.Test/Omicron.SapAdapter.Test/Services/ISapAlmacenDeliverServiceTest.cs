// <summary>
// <copyright file="ISapAlmacenDeliverServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class ISapAlmacenDeliverServiceTest : BaseTest
    {
        private ISapAlmacenDeliveryService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

        private Mock<ICatalogsService> catalogService;

        private Mock<IRedisService> mockRedis;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacenDelivery")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.CatalogProductModel.AddRange(this.GetCatalogProductModel());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.DeliverModel.AddRange(this.DeliveryModel());
            this.context.DeliveryDetailModel.AddRange(this.GetDeliveryDetail());
            this.context.BatchTransacitions.AddRange(this.GetBatchTransacitions());
            this.context.BatchesTransactionQtyModel.AddRange(this.GetBatchesTransactionQtyModel());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.LblContainerModel.AddRange(this.GetLblContainer());
            this.context.InvoiceHeaderModel.AddRange(this.GetInvoiceHeader());
            this.context.SaveChanges();

            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            this.catalogService = new Mock<ICatalogsService>();
            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(parameters)));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctor = new Mock<IDoctorService>();

            this.mockRedis = new Mock<IRedisService>();

            this.mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            this.mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapService = new SapAlmacenDeliveryService(this.sapDao, mockPedidoService.Object, mockAlmacenService.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDelivery()
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();

            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.StartDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy) },
                { ServiceConstants.EndDateParam, DateTime.Now.ToString(ServiceConstants.DateTimeFormatddMMyyyy) },
            };

            var mockDoctor = new Mock<IDoctorService>();

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryWithoutMaquila()
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, $"{ServiceConstants.Line},{ServiceConstants.Mixto.ToLower()},{ServiceConstants.Magistral.ToLower()}, {ServiceConstants.OmigenomicsGroup.ToLower()}" },
            };

            var mockDoctor = new Mock<IDoctorService>();

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chip.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("1")]
        [TestCase("aa")]
        public async Task GetDelivery(string chip)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));
            var mockProccessPayments = new Mock<IProccessPayments>();

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { "chips", chip },
            };

            var mockDoctor = new Mock<IDoctorService>();

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryPackage()
        {
            // arrange
            var resultPedido = new ResultDto
            {
                Code = 200,
                ExceptionMessage = JsonConvert.SerializeObject(new List<int>()),
                Response = JsonConvert.SerializeObject(new List<UserOrderModel>()),
                Success = true,
                Comments = "15",
            };

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(resultPedido));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemisionPackage()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, ServiceConstants.Paquetes.ToLower() },
            };

            var mockDoctor = new Mock<IDoctorService>();

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chip.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(46040)]
        public async Task GetOrdersDeliveryDetail(int chip)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();

            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };
            var mockProccessPayments = new Mock<IProccessPayments>();
            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetOrdersDeliveryDetail(chip);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.InstanceOf<SalesModel>());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chip.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("75001-46037")]
        public async Task GetProductsDelivery(string chip)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockProccessPayments = new Mock<IProccessPayments>();

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetProductsDelivery(chip);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// GetDeliveryIdsByInvoice.
        /// </summary>
        /// <param name="invoiceId">the invoiceId.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(1)]
        public async Task GetDeliveryIdsByInvoice(int invoiceId)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctor = new Mock<IDoctorService>();
            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetDeliveryIdsByInvoice(invoiceId);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.InstanceOf<IEnumerable<int>>());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        [Test]
        public void CalculateRemittedPieces()
        {
            // arrange
            var itemCode = "REVE 16 - 1003";

            var lineProducts = new List<LineProductsModel>
            {
                new LineProductsModel { ItemCode = "REVE 16", BatchName = "[{\"BatchQty\":5}]", DeliveryId = 123 },
                new LineProductsModel { ItemCode = "REVE 16", BatchName = "[{\"BatchQty\":10}]", DeliveryId = 123 },
                new LineProductsModel { ItemCode = "REVE 15", BatchName = "[{\"BatchQty\":10}]", DeliveryId = 123 },
            };

            var mockPedidos = new Mock<IPedidosService>();

            var mockAlmacen = new Mock<IAlmacenService>();

            var mockProccessPayments = new Mock<IProccessPayments>();

            var mockDoctor = new Mock<IDoctorService>();

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = service.CalculateRemittedPieces(itemCode, lineProducts);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(15));
        }
    }
}
