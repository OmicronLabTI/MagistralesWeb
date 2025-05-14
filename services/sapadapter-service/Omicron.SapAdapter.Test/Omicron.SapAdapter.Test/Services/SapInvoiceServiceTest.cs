// <summary>
// <copyright file="SapInvoiceServiceTest.cs" company="Axity">
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
    public class SapInvoiceServiceTest : BaseTest
    {
        private ISapInvoiceService sapInvoiceService;

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
                .UseInMemoryDatabase(databaseName: "TemporalAlmacenInvoice")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.CatalogProductModel.AddRange(this.GetCatalogProductModel());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.DeliverModel.AddRange(this.DeliveryModel());
            this.context.DeliveryDetailModel.AddRange(this.GetDeliveryDetail());
            this.context.BatchTransacitions.AddRange(this.GetBatchTransacitions());
            this.context.BatchesTransactionQtyModel.AddRange(this.GetBatchesTransactionQtyModel());
            this.context.InvoiceHeaderModel.AddRange(this.GetInvoiceHeader());
            this.context.InvoiceDetailModel.AddRange(this.GetInvoiceDetails());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.SalesPersonModel.AddRange(this.GetSalesPerson());
            this.context.SaveChanges();

            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockDoctors = new Mock<IDoctorService>();
            mockDoctors
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

            this.mockRedis = new Mock<IRedisService>();

            this.mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            this.mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            this.sapDao = new SapDao(this.context, mockLog.Object);
            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };
            var mockProccessPayments = new Mock<IProccessPayments>();
            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));
            this.sapInvoiceService = new SapInvoiceService(this.sapDao, mockPedidoService.Object, mockAlmacenService.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoice()
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();

            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetInvoice(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response, Is.InstanceOf<InvoiceOrderModel>());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chips.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("1")]
        [TestCase("alias")]
        public async Task GetInvoice(string chip)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { "chips", chip },
                { ServiceConstants.Shipping, "Foraneo" },
            };

            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();
            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetInvoice(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response, Is.InstanceOf<InvoiceOrderModel>());
        }

        /// <summary>
        /// Test the method to get the invoice detail.
        /// </summary>
        /// <param name="invoice">the invoice to look for.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetInvoiceDetail(int invoice)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

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

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await service.GetInvoiceDetail(invoice);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response, Is.InstanceOf<InvoicesModel>());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoiceProducts()
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            mockPedidos
               .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
               .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
               .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
               .Returns(Task.FromResult(this.GetLineProductsRemision()));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetInvoiceProducts("1", "Distribucion", Enumerable.Empty<int>().ToList());

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryAcannedData()
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetDeliveryScannedData("46037");

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryScannedDataMg()
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();

            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetMagistralProductInvoice("75000-1000");

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="code">the code to look.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("Linea1-1-0")]
        [TestCase("Linea10-1-0")]
        public async Task GetDeliveryScannedDataLn(string code)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsScannInvoice()));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetLineProductInvoice(code);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="type">the code to look.</param>
        /// <param name="chip">the chip.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("foraneo", "")]
        [TestCase("local", "")]
        [TestCase("local", "1")]
        public async Task GetInvoiceHeaders(string type, string chip)
        {
            // arrange
            var listUserOrder = new List<int>
            {
                2,
                3,
                4,
            };

            var exclusivePartnersIds = new List<string>
            {
                "C1",
                "C2",
                "C3",
            };

            var dataTollok = new InvoicePackageSapLookModel
            {
                InvoiceDocNums = listUserOrder,
                Limit = 10,
                Offset = 0,
                Type = type,
                Chip = chip,
                ExclusivePartnersIds = exclusivePartnersIds,
            };

            // act
            var response = await this.sapInvoiceService.GetInvoiceHeader(dataTollok);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Code == 200);
            Assert.That(response.Success);
            Assert.That(response.Response, Is.InstanceOf<IEnumerable<InvoiceHeaderModel>>());
            Assert.That(response.Comments, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="code">the code to look.</param>
        /// <param name="subcode"> the sub code to look.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("1", "1")]
        public async Task GetInvoiceData(string code, string subcode)
        {
            // arrange
            var packages = new List<PackageModel>();
            var packagesResponse = this.GetResultDto(packages);

            var clients = new List<ExclusivePartnersModel>();
            var clientResponse = this.GetResultDto(clients);

            var processPayments = new List<PaymentsDto>()
            {
                new PaymentsDto { TransactionId = "123456", ShippingCostAccepted = 1 },
            };

            var doctorsData = new List<DoctorAddressModel>
            {
                new DoctorAddressModel { AddressId = "Address1", BetweenStreets = "steets", References = "reference", EtablishmentName = "stabblishment" },
            };

            var userorders = new List<UserOrderDto>
            {
                new UserOrderDto { InvoiceId = 1, InvoiceLineNum = 1 },
                new UserOrderDto { InvoiceId = 1, InvoiceLineNum = 2 },
            };

            var userordersResponse = this.GetResultDto(userorders);

            var mockPedidos = new Mock<IPedidosService>();

            mockPedidos.Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(userordersResponse));

            var lineproducts = new List<LineProductsDto>
            {
                new LineProductsDto { InvoiceId = 1, InvoiceLineNum = 3 },
            };

            var lineproductsResponse = this.GetResultDto(lineproducts);

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(lineproductsResponse))
                .Returns(Task.FromResult(packagesResponse));

            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(clientResponse));

            var mockProccessPayments = new Mock<IProccessPayments>();

            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(processPayments)));

            var mockDoctors = new Mock<IDoctorService>();
            mockDoctors
                .Setup(x => x.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(doctorsData)));

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetInvoiceData(code, subcode);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetSapIds()
        {
            // arrange
            var listIds = new List<int> { 75000 };

            // act
            var response = await this.sapInvoiceService.GetSapIds(listIds);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetCancelledInvoices()
        {
            var days = 30;

            // act
            var response = await this.sapInvoiceService.GetCancelledInvoices(days);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the invoices by ids.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetInvoicesByIds()
        {
            var ids = new List<int> { 2, 3 };

            // act
            var response = await this.sapInvoiceService.GetInvoicesByIds(ids);
            var invoices = response.Response as List<InvoiceHeaderModel>;

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Success);
            Assert.That(invoices.Count > 0);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chips.</param>
        /// <param name="hasChipFilter">hasChipFilter.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("1", true)]
        [TestCase("alias", true)]
        [TestCase(null, false)]
        public async Task GetInvoiceByFilters(string chip, bool hasChipFilter)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Shipping, "Foraneo" },
                { ServiceConstants.StartDateParam, DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy") },
                { ServiceConstants.EndDateParam, DateTime.Now.ToString("dd/MM/yyyy") },
            };

            if (hasChipFilter)
            {
                dictionary.Add("chips", chip);
            }

            var payments = new List<PaymentsDto>()
            {
                new PaymentsDto { CardCode = "C00007", ShippingCostAccepted = 1, TransactionId = "ac901443-c548-4860-9fdc-fa5674847822" },
            };
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();
            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));
            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var response = await service.GetInvoiceByFilters(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response, Is.InstanceOf<InvoiceOrderModel>());
        }

        /// <summary>
        /// Test the method to get the invoices by ids.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetClosedInvoicesByDocNum()
        {
            var ids = new List<int> { 2, 3 };

            // act
            var response = await this.sapInvoiceService.GetClosedInvoicesByDocNum(ids);
            var invoices = response.Response as List<InvoiceHeaderModel>;

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.UserError, Is.Null);
            Assert.That(response.Success);
        }

        /// <summary>
        /// Test the method to get the invoices by ids.
        /// </summary>
        [Test]
        public void CalculateStored()
        {
            var lineProducts = new List<LineProductsModel>
            {
                new LineProductsModel { Id = 51, SaleOrderId = 175528, ItemCode = "REVE 44", StatusAlmacen = "Empaquetado", BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchNumber = "AXB-23", BatchQty = 1, WarehouseCode = "PT" } }), DeliveryId = 135500, InvoiceId = 150199 },
                new LineProductsModel { Id = 52, SaleOrderId = 175528, ItemCode = "REVE 14", StatusAlmacen = "Empaquetado",  BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchNumber = "OMD0122-1", BatchQty = 3, WarehouseCode = "CUA" } }), DeliveryId = 135500, InvoiceId = 150199 },
                new LineProductsModel { Id = 53, SaleOrderId = 175528,  ItemCode = "REVE 44", StatusAlmacen = "Empaquetado", BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchNumber = "AXB-23", BatchQty = 1, WarehouseCode = "PT" } }), DeliveryId = 135501, InvoiceId = 150199 },
            };

            var usersOrder = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 40, InvoiceId = 150199, StatusAlmacen = "Empaquetado", Salesorderid = "175524", Productionorderid = "226236", MagistralQr = JsonConvert.SerializeObject(new PedidosMagistralQrModel { SaleOrder = 175524, ProductionOrder = 226236, Quantity = 2, ItemCode = "567   30 ML" }) },
                new UserOrderModel { Id = 41, InvoiceId = 150199, StatusAlmacen = "Empaquetado", Salesorderid = "175525", Productionorderid = "226237", MagistralQr = JsonConvert.SerializeObject(new PedidosMagistralQrModel { SaleOrder = 175525, ProductionOrder = 226237, Quantity = 5, ItemCode = "BQ 01" }) },
                new UserOrderModel { Id = 42, InvoiceId = 150199, StatusAlmacen = "Empaquetado", Salesorderid = "175526", Productionorderid = "226238", MagistralQr = JsonConvert.SerializeObject(new PedidosMagistralQrModel { SaleOrder = 175526, ProductionOrder = 226238, Quantity = 1, ItemCode = "BQ 01" }) },
                new UserOrderModel { Id = 43, InvoiceId = 150199, StatusAlmacen = "Empaquetado", Salesorderid = "175527", Productionorderid = "226239", MagistralQr = JsonConvert.SerializeObject(new PedidosMagistralQrModel { SaleOrder = 175527, ProductionOrder = 226239, Quantity = 2, ItemCode = "BE 01   30 CAP" }) },
            };

            var mockPedidos = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var (piezas, productos) = service.CalculateStored(lineProducts, usersOrder);

            // assert
            Assert.That(piezas, Is.EqualTo(15));
            Assert.That(productos, Is.EqualTo(7));
        }

        /// <summary>
        /// Test the method to get the invoices by ids.
        /// </summary>
        [Test]
        public void CalculateStoredBatchNameList()
        {
            var lineProducts = new List<LineProductsModel>
            {
                new LineProductsModel
                {
                    Id = 24, SaleOrderId = 175289, ItemCode = "REVE 42", StatusAlmacen = "Empaquetado",
                    BatchName = JsonConvert.SerializeObject(new[]
                    {
                     new AlmacenBatchModel { BatchNumber = "B25-AX6", BatchQty = 5, WarehouseCode = "PT" },
                     new AlmacenBatchModel { BatchNumber = "B25-AX7", BatchQty = 5, WarehouseCode = "PT" },
                     new AlmacenBatchModel { BatchNumber = "B25-AX5", BatchQty = 2, WarehouseCode = "PT" },
                     new AlmacenBatchModel { BatchNumber = "B25-AX4", BatchQty = 3, WarehouseCode = "PT" },
                    }),
                    DeliveryId = 135189, InvoiceId = 150163,
                },
                new LineProductsModel { Id = 25, SaleOrderId = 175289, ItemCode = "REVE 23", StatusAlmacen = "Empaquetado",  BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchNumber = "BAX-25", BatchQty = 1, WarehouseCode = "GENERAL" } }), DeliveryId = 135189, InvoiceId = 150163 },
                new LineProductsModel { Id = 26, SaleOrderId = 175289,  ItemCode = "REVE 16", StatusAlmacen = "Empaquetado", BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchNumber = "AXB-25", BatchQty = 1, WarehouseCode = "CUA" } }), DeliveryId = 135189, InvoiceId = 150163 },
                new LineProductsModel { Id = 27, SaleOrderId = 175289,  ItemCode = "REVE 14", StatusAlmacen = "Empaquetado", BatchName = JsonConvert.SerializeObject(new[] { new AlmacenBatchModel { BatchNumber = "AXB-23", BatchQty = 1, WarehouseCode = "CUA" } }), DeliveryId = 135189, InvoiceId = 150163 },
                new LineProductsModel { Id = 28, SaleOrderId = 175289,  ItemCode = null, StatusAlmacen = "Almacenado", BatchName = null, DeliveryId = 135189, InvoiceId = 0 },
            };

            var usersOrder = new List<UserOrderModel>();

            var mockPedidos = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var mockDoctors = new Mock<IDoctorService>();

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, this.catalogService.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctors.Object);

            // act
            var (piezas, productos) = service.CalculateStored(lineProducts, usersOrder);

            // assert
            Assert.That(piezas, Is.EqualTo(18));
            Assert.That(productos, Is.EqualTo(4));
        }
    }
}
