// <summary>
// <copyright file="SapAlmacenServiceTest.cs" company="Axity">
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
    public class SapAlmacenServiceTest : BaseTest
    {
        private ISapAlmacenService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

        private Mock<IRedisService> mockRedis;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacen")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.CatalogProductModel.AddRange(this.GetCatalogProductModel());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.LblContainerModel.AddRange(this.GetClassificationsCatalog());
            this.context.SaveChanges();

            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockProccessPayments = new Mock<IProccessPayments>();

            this.mockRedis = new Mock<IRedisService>();

            this.mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            this.mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapService = new SapAlmacenService(this.sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogs.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersBasic()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "Apodaca" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProducts()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            var result = new List<ActiveConfigRoutesModel>();
            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, "LN,BQ,MQ,MG,MN,BE,mixto" },
            };

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chips.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("75000")]
        [TestCase("aa")]
        public async Task GetOrders(string chip)
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "Apodaca" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProducts()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

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
            mockProccessPayments
                .Setup(m => m.PostProccessPayments(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(payments)));

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chips.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("#xse123")]
        public async Task GetOrdersbyDocNumDxp(string chip)
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "Apodaca" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProducts()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { "chips", chip },
            };

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
            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersWithoutMaquila()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProducts()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, $"LN,{ServiceConstants.Mixto.ToLower()}" },
            };

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));
            var mockProccessPayments = new Mock<IProccessPayments>();
            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersPaquetes()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            var parametersResponse = this.GetResultDto(parameters);

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
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(resultPedido))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));
            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));
            var mockProccessPayments = new Mock<IProccessPayments>();
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, $"{ServiceConstants.Paquetes.ToLower()}" },
            };

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersDetails()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderDetailModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRecepcionDetail()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

            var colorsResponse = new List<ProductColorsDto>
            {
                new ProductColorsDto() { BackgroundColor = "#f3f3f3", TemaId = string.Empty, LabelText = "tema 1", TextColor = "#ffffff" },
                new ProductColorsDto() { BackgroundColor = "#f3f3f3", TemaId = "tema1", LabelText = "tema 1", TextColor = "#ffffff" },
            };

            mockCatalogos
                .Setup(m => m.PostCatalogs(It.IsAny<object>(), ServiceConstants.GetThemes))
                .Returns(Task.FromResult(this.GetResultDto(colorsResponse)));

            var ids = 75000;

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

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrdersDetails(ids);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersDetailsRemittedPieces()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderDetailModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRecepcionDetail()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

            var colorsResponse = new List<ProductColorsDto>
            {
                new ProductColorsDto() { BackgroundColor = "#f3f3f3", TemaId = string.Empty, LabelText = "tema 1", TextColor = "#ffffff" },
                new ProductColorsDto() { BackgroundColor = "#f3f3f3", TemaId = "tema1", LabelText = "tema 1", TextColor = "#ffffff" },
            };

            mockCatalogos
                .Setup(m => m.PostCatalogs(It.IsAny<object>(), ServiceConstants.GetThemes))
                .Returns(Task.FromResult(this.GetResultDto(colorsResponse)));

            var ids = 75000;

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

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrdersDetails(ids);
            var result = response.Response as ReceipcionPedidosDetailModel;

            TestContext.WriteLine($"Result: {JsonConvert.SerializeObject(result, Formatting.Indented)}");

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(result.AlmacenHeader.RemittedPieces, Is.EqualTo(1));
            var linea1 = result.Items.FirstOrDefault(i => i.ItemCode == "Linea1");
            Assert.That(linea1, Is.Not.Null);
            Assert.That(linea1.RemittedPieces, Is.EqualTo(1));
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetMagistralScannedData()
        {
            // arrange
            var order = "75000-1000";

            // act
            var response = await this.sapService.GetMagistralScannedData(order);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedData()
        {
            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var ptresult = new List<WarehouseDto> { new WarehouseDto { WarehouseCodes = new List<string> { "PT" } }, };
            mockCatalogs
                .Setup(x => x.PostCatalogs(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(ptresult)));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();

            var sapDao = new SapDao(this.context, mockLog.Object);
            var sapService = new SapAlmacenService(sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogs.Object, mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // arrange
            var order = "aa";
            var orderId = 11;

            // act
            var response = await sapService.GetLineScannedData(order, orderId);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedDataBatches()
        {
            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var almacenResult = new RemittedPiecesModel() { AvailablePieces = 0, ItemCode = "Linea1", RemissionPieces = 5, TotalPieces = 0 };
            mockAlmacenService
                .Setup(x => x.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(almacenResult)));
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var ptresult = new List<WarehouseDto> { new WarehouseDto { ItemCode = "Linea1", WarehouseCodes = new List<string> { "PT" } }, };
            mockCatalogs
                .Setup(x => x.PostCatalogs(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(ptresult)));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();

            var sapDao = new SapDao(this.context, mockLog.Object);
            var sapService = new SapAlmacenService(sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogs.Object, mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // arrange
            var order = "Linea1";
            var orderId = 11;

            // act
            var response = await sapService.GetLineScannedData(order, orderId);
            var result = response.Response as LineScannerModel;

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedDataRemittedPieces()
        {
            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var almacenResult = new RemittedPiecesModel() { AvailablePieces = 0, ItemCode = "Linea50", RemissionPieces = 5, TotalPieces = 0 };
            mockAlmacenService
                .Setup(x => x.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(almacenResult)));

            var mockCatalogs = new Mock<ICatalogsService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var ptresult = new List<WarehouseDto> { new WarehouseDto { ItemCode = "Linea50", WarehouseCodes = new List<string> { "PT" } }, };
            mockCatalogs
                .Setup(x => x.PostCatalogs(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(ptresult)));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();

            var sapDao = new SapDao(this.context, mockLog.Object);
            var sapService = new SapAlmacenService(sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogs.Object, mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // arrange
            var order = "Linea50";
            var orderId = 11;

            // act
            var response = await sapService.GetLineScannedData(order, orderId);
            var result = response.Response as LineScannerModel;

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Pieces, Is.Not.Null);

            Assert.That(result.Pieces.ItemCode, Is.EqualTo(order));
            Assert.That(result.Pieces.RemissionPieces, Is.EqualTo(5));
            Assert.That(result.Pieces.AvailablePieces, Is.EqualTo(5));
            Assert.That(result.Pieces.TotalPieces, Is.EqualTo(10));
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedDataWithoutWarehouse()
        {
            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var ptresult = new List<WarehouseDto> { new WarehouseDto() { ItemCode = "Linea1", WarehouseCodes = new List<string>(), }, };
            mockCatalogs
                .Setup(x => x.PostCatalogs(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(ptresult)));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();

            var sapDao = new SapDao(this.context, mockLog.Object);
            var sapService = new SapAlmacenService(sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogs.Object, mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // arrange
            var order = "Linea1";
            var orderId = 11;

            // act
            var response = await sapService.GetLineScannedData(order, orderId);

            // assert
            Assert.That(response.Code == 404);
            Assert.That(response.UserError == ServiceConstants.NoActiveWarehouseError);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedDataWithoutBoxes()
        {
            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockProccessPayments = new Mock<IProccessPayments>();
            var ptresult = new List<WarehouseDto> { new WarehouseDto { ItemCode = "Linea1", WarehouseCodes = new List<string> { "XXXXXXXX" } }, };

            mockCatalogs
                .Setup(x => x.PostCatalogs(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(ptresult)));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();

            var sapDao = new SapDao(this.context, mockLog.Object);
            var sapService = new SapAlmacenService(sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogs.Object, mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // arrange
            var order = "Linea1";
            var orderId = 11;

            // act
            var response = await sapService.GetLineScannedData(order, orderId);

            // assert
            Assert.That(response.Code == 404);
            Assert.That(response.UserError == ServiceConstants.NoAvaiableBoxesError);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetCompleteDetail()
        {
            // arrange
            var order = 1000;

            // act
            var response = await this.sapService.GetCompleteDetail(order);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetProductsWithCodeBars()
        {
            // act
            var response = await this.sapService.GetProductsWithCodeBars();

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders models.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersByIds()
        {
            var ordersToLook = new List<int> { 100, 101, 102 };

            // act
            var response = await this.sapService.GetOrdersByIds(ordersToLook);
            var orders = response.Response as List<OrderModel>;
            var countDxpOrders = response.Comments as IEnumerable<CountDxpOrders>;

            // asserts
            Assert.That(response.Success);
            Assert.That(response.Code == 200);
            Assert.That(response.Response, Is.InstanceOf<List<OrderModel>>());
            Assert.That(response.Comments, Is.InstanceOf<IEnumerable<CountDxpOrders>>());
            Assert.That(orders.Any());
            Assert.That(countDxpOrders.Any());
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryBySaleOrderId()
        {
            // arrange
            var order = new List<int>();

            // act
            var response = await this.sapService.GetDeliveryBySaleOrderId(order);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task AlmacenGraphCount()
        {
            // arrange
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            var dict = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, $"{yesterday}-{today}" },
            };

            // act
            var response = await this.sapService.AlmacenGraphCount(dict);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveryParties()
        {
            // act
            var response = await this.sapService.GetDeliveryParties();

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// the test.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetDeliveries()
        {
            // arrange
            var ids = new List<int> { 100 };

            // act
            var response = await this.sapService.GetDeliveries(ids);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersWithNewValues()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "Apodaca" },
            };

            var parametersResponse = this.GetResultDto(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            mockPedidos
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .SetupSequence(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProducts()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            var result = new List<ActiveConfigRoutesModel>();
            mockCatalogos
                .Setup(m => m.GetParams(ServiceConstants.GetActiveRouteConfigurationsEndPoint))
                .Returns(Task.FromResult(this.GetResultDto(this.GetConfigs(new List<string> { "LN", "BQ", "MQ", "MG", "MN", "BE", "mixto" }))));

            var mockProccessPayments = new Mock<IProccessPayments>();
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, "LN,BQ,MQ,MG,MN,BE,mixto" },
            };

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object, this.mockRedis.Object, mockProccessPayments.Object, mockDoctor.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.That(response, Is.Not.Null);
        }
    }
}
