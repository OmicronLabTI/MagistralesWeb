// <summary>
// <copyright file="SapServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class SapServiceTest : BaseTest
    {
        private ISapService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

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
            this.context.AsesorModel.Add(this.GetAsesorModel());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.CatalogProductModel.AddRange(this.GetCatalogProductModel());
            this.context.Users.AddRange(this.GetSapUsers());
            this.context.DetalleFormulaModel.AddRange(this.GetDetalleFormula());
            this.context.ItemWarehouseModel.AddRange(this.GetItemWareHouse());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.BatchTransacitions.AddRange(this.GetBatchTransacitions());
            this.context.BatchesTransactionQtyModel.AddRange(this.GetBatchesTransactionQtyModel());
            this.context.AttachmentModel.AddRange(this.GetAttachmentModel());
            this.context.RawMaterialRequestModel.AddRange(this.GetRawMaterialRequestModel());
            this.context.RawMaterialRequestDetailModel.AddRange(this.GetRawMaterialRequestDetailModel());
            this.context.WarehouseModel.AddRange(this.GetWarehouse());
            this.context.LblContainerModel.AddRange(this.GetLblContainer());

            this.context.SaveChanges();
            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();
            var mockCatalogs = new Mock<ICatalogsService>();

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Value = "A1", Field = "Medic" },
                new ParametersModel { Id = 2, Value = "Codigo", Field = "CardCodeResponsibleMedic" },
            };

            mockCatalogs
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(parameters)));

            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:prefix")]).Returns("L-");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:numberPositions")]).Returns("7");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "OmicronRecipeAddress")]).Returns("http://localhost:5002/");

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetPedidosService()));

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetUsersById()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            mockRedis
                .Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            this.sapService = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object, mockDoctor.Object, mockCatalogs.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersFechaIni()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersFechaFin()
        {
            // arrange
            var dates = new DateTime(2020, 1, 20).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dates) },
                { ServiceConstants.Status, "O" },
                { ServiceConstants.Qfb, "abc" },
            };

            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();
            var mockCatalogs = new Mock<ICatalogsService>();

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Value = "A1", Field = "Medic" },
            };

            mockCatalogs
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(parameters)));

            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:prefix")]).Returns("L-");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:numberPositions")]).Returns("7");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "OmicronRecipeAddress")]).Returns("http://localhost:5002/");

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetUsersById()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            mockRedis
                .Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            var localSap = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object, mockDoctor.Object, mockCatalogs.Object);

            // act
            var result = await localSap.GetOrders(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersId()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, "100-100" },
                { ServiceConstants.Status, "O" },
                { ServiceConstants.Qfb, "abc" },
                { ServiceConstants.OrderType, "MN" },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersStatus()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                { ServiceConstants.Status, "O" },
                { ServiceConstants.Qfb, "abc" },
                { ServiceConstants.Cliente, "cliente" },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetAsesorsByOrderId()
        {
            // arrange
            var salesOrders = new List<int>
            {
                12,
                13,
            };

            // act
            var result = await this.sapService.GetAsesorsByOrderId(salesOrders);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersTodayById()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, "100-100" },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// the order detail.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrderDetail()
        {
            // arrange
            var docId = 100;

            // act
            var result = await this.sapService.GetOrderDetails(docId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetPedidoWithDetail()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetPedidoWithDetail(listIds);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetPedidoWithDetailAndDxp()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetPedidoWithDetailAndDxp(listIds);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetProdOrderByOrderItem()
        {
            var listIds = new List<string> { "100-Buscapina", "100-Omigenomics" };
            var result = await this.sapService.GetProdOrderByOrderItem(listIds);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Response, Is.Not.Null);
            var response = JsonConvert.DeserializeObject<List<OrdenFabricacionModel>>(result.Response.ToString());
            Assert.That(response.Count.Equals(2));
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(103)]
        [TestCase(100)]
        public async Task GetOrderFormula(int id)
        {
            // arrange
            var listIds = new List<int> { id };

            // act
            var result = await this.sapService.GetOrderFormula(listIds, true, true);
            var formulaDeatil = result.Response as CompleteFormulaWithDetalle;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Code == 200);
            Assert.That(formulaDeatil.Details.Any());
            Assert.That(result.Response, Is.InstanceOf<CompleteFormulaWithDetalle>());
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
            Assert.That(result.Comments, Is.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrderFormulaList()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetOrderFormula(listIds, false, true);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <param name="typeClient">Type client.</param>
        [Test]
        [TestCase("institucional")]
        [TestCase("clinica")]
        [TestCase("general")]
        public void GetClientDxpTest(string typeClient)
        {
            // arrange
            var pedidoLocal = new OrderModel
            {
                PedidoId = 1,
                ClientType = typeClient,
                Codigo = "DOC123",
                ProffesionalLicense = "LIC456",
                Medico = "Dr. House",
                ShippingAddressName = "4. ELDY VILLAGOMEZ LLANOS C.7731057",
            };

            var listDoctors = new List<DoctorPrescriptionInfoModel>
             {
                new DoctorPrescriptionInfoModel { CardCode = "DOC123", License = "LIC456", DoctorName = "Dr. Wilson" },
                new DoctorPrescriptionInfoModel { CardCode = "DOC789", License = "LIC999", DoctorName = "Dr. Cameron" },
             };

            var specialCardCodes = new List<string> { "SPECIAL001", "SPECIAL002" };

            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockLog = new Mock<ILogger>();
            var mockDoctor = new Mock<IDoctorService>();

            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            var localSap = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object, mockDoctor.Object, mockCatalogs.Object);

            // act
            var result = localSap.GetClientDxp(pedidoLocal, listDoctors, specialCardCodes);

            // assert
            if (typeClient == "institucional" || typeClient == "clinica")
            {
                Assert.That(result, Is.EqualTo("ELDY VILLAGOMEZ LLANOS"));
            }
            else
            {
                Assert.That(result, Is.EqualTo("Dr. Wilson"));
            }
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <param name="typeClient">Type client.</param>
        /// <param name="shippingAddressName">shipping Address Name.</param>
        [Test]
        [TestCase(null, "4. ELDY VILLAGOMEZ LLANOS C.7731057")]
        [TestCase(null, "1. ABEL ESCOBEDO GONZALEZ C.3393579")]
        [TestCase(null, "6. ALAN GILBERTO RAMIREZ VALVERDE C.11535444")]
        [TestCase(null, "ALAN GILBERTO RAMIREZ VALVERDE")]
        public void GetClientDxpTestTypeClientNull(string typeClient, string shippingAddressName)
        {
            // arrange
            var pedidoLocal = new OrderModel
            {
                PedidoId = 1,
                ClientType = typeClient,
                Codigo = "DOC123",
                ProffesionalLicense = "LIC456",
                Medico = "Dr. House",
                ShippingAddressName = shippingAddressName,
            };

            var listDoctors = new List<DoctorPrescriptionInfoModel>
             {
                new DoctorPrescriptionInfoModel { CardCode = "DOC123", License = "LIC456", DoctorName = "Dr. Wilson" },
                new DoctorPrescriptionInfoModel { CardCode = "DOC789", License = "LIC999", DoctorName = "Dr. Cameron" },
             };

            var specialCardCodes = new List<string> { "SPECIAL001", "SPECIAL002" };

            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockLog = new Mock<ILogger>();
            var mockDoctor = new Mock<IDoctorService>();

            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            var localSap = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object, mockDoctor.Object, mockCatalogs.Object);

            // act
            var result = localSap.GetClientDxp(pedidoLocal, listDoctors, specialCardCodes);

            // assert
            if (shippingAddressName == "4. ELDY VILLAGOMEZ LLANOS C.7731057")
            {
                Assert.That(result, Is.EqualTo("ELDY VILLAGOMEZ LLANOS"));
            }
            else if (shippingAddressName == "1. ABEL ESCOBEDO GONZALEZ C.3393579")
            {
                Assert.That(result, Is.EqualTo("ABEL ESCOBEDO GONZALEZ"));
            }
            else if (shippingAddressName == "6. ALAN GILBERTO RAMIREZ VALVERDE C.11535444")
            {
                Assert.That(result, Is.EqualTo("ALAN GILBERTO RAMIREZ VALVERDE"));
            }
            else
            {
                Assert.That(result, Is.EqualTo("Dr. Wilson"));
            }
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetValidationQuatitiesOrdersFormula()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.sapService.GetValidationQuatitiesOrdersFormula(listIds);
            var formulaDeatil = result.Response as List<CompleteDetalleFormulaModel>;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Code == 200);
            Assert.That(formulaDeatil.Any());
            Assert.That(result.Response, Is.InstanceOf<List<CompleteDetalleFormulaModel>>());
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
            Assert.That(result.Comments, Is.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("35642b3a-9471-4b89-9862-8bee6d98c361")]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7")]
        [TestCase("")]
        public async Task GetComponents(string userId)
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", "Abc,pirina" },
            };

            if (!string.IsNullOrEmpty(userId))
            {
                paramsDict["userId"] = userId;
            }

            // act
            var result = await this.sapService.GetComponents(paramsDict);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetComponentsChipDescription()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", "pirina" },
            };

            // act
            var result = await this.sapService.GetComponents(paramsDict);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetComponentsNoData()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", "qwerty" },
            };

            // act
            var result = await this.sapService.GetComponents(paramsDict);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        [Test]
        public void GetComponentsNoChips()
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
            };

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await this.sapService.GetComponents(paramsDict));
        }

        /// <summary>
        /// Get the order with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetBatchesComponents()
        {
            // arrange
            var ordenId = 100;

            // act
            var result = await this.sapService.GetBatchesComponents(ordenId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetlLastIsolatedProductionOrderId()
        {
            // arrange
            var productId = "Abc Aspirina";
            var uniqueId = "token";

            // act
            var result = await this.sapService.GetlLastIsolatedProductionOrderId(productId, uniqueId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetNextBatchCode()
        {
            // arrange
            var productId = "Abc Aspirina";

            // act
            var result = await this.sapService.GetNextBatchCode(productId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get next batch code.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task ValidateIfExistsBatchCodeByItemCode()
        {
            // arrange
            var productId = "Abc Aspirina";
            var batchCode = "Lote1";

            // act
            var result = await this.sapService.ValidateIfExistsBatchCodeByItemCode(productId, batchCode);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="needLargeDescription">need large descr.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetFabOrdersOnlyLocals(bool needLargeDescription)
        {
            // arrange
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 100 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dateFinal) },
                    { ServiceConstants.Status, "Asignado" },
                    { ServiceConstants.Qfb, "abc" },
                },
            };

            if (needLargeDescription)
            {
                parameters.Filters.Add(ServiceConstants.NeedsLargeDsc, "true");
            }

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersOnlyLocalsFechaIni()
        {
            // arrange
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                    { ServiceConstants.Status, "Asignado" },
                    { ServiceConstants.Qfb, "abc" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersOnlyLocalsItemCode()
        {
            // arrange
            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.ItemCode, "Abc" },
                    { ServiceConstants.Status, "Asignado" },
                    { ServiceConstants.Qfb, "abc" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersOnlyLocalsItemCodeFechaIni()
        {
            // arrange
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                    { ServiceConstants.ItemCode, "Abc" },
                    { ServiceConstants.Status, "Asignado" },
                    { ServiceConstants.Qfb, "abc" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersOnlyLocalsDocNum()
        {
            // arrange
            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.DocNum, "100-100" },
                    { ServiceConstants.Qfb, "abc" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersDbFechaIni()
        {
            // arrange
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersDbItemCode()
        {
            // arrange
            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.ItemCode, "Abc" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersDbItemCodeDateIniOffset()
        {
            // arrange
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.ItemCode, "Abc" },
                    { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                    { ServiceConstants.Offset, "1" },
                    { ServiceConstants.Limit, "1" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersDbItemCodeDateIni()
        {
            // arrange
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.ItemCode, "Abc" },
                    { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersDocNum()
        {
            // arrange
            var parameters = new GetOrderFabModel
            {
                OrdersId = new List<int> { 120 },
                Filters = new Dictionary<string, string>
                {
                    { ServiceConstants.DocNum, "100-100" },
                },
            };

            // act
            var result = await this.sapService.GetFabOrders(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersById()
        {
            // arrange
            var parameters = new List<int> { 100 };

            // act
            var result = await this.sapService.GetFabOrdersById(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get products management by batches with criterials.
        /// </summary>
        /// <param name="criterials">Filters.</param>
        /// <param name="expectedResults">Number of expected results.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("Abc,pirina", 1)]
        [TestCase("ento", 1)]
        [TestCase("ung", 1)]
        [TestCase("10 GR", 1)]
        [TestCase("10 GR,enTo", 1)]
        [TestCase("psula", 1)]
        public async Task GetProductsManagmentByBatch(string criterials, int expectedResults)
        {
            // arrange
            var paramsDict = new Dictionary<string, string>
            {
                { "offset", "0" },
                { "limit", "10" },
                { "chips", criterials },
            };

            // act
            var result = await this.sapService.GetProductsManagmentByBatch(paramsDict);

            // assert
            var responseAsJson = JsonConvert.SerializeObject(result.Response);
            var returnItems = JsonConvert.DeserializeObject<List<ProductoModel>>(responseAsJson);
            Assert.That(result, Is.Not.Null);
            Assert.That(returnItems.Count.Equals(expectedResults));
        }

        /// <summary>
        /// Get production orders with details.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFormulaBySalesOrdersOrProductionOrders()
        {
            // arrange
            var productionOrders = new List<int> { 100 };
            var salesOrders = new List<int> { 100 };

            // act
            var result = await this.sapService.GetFabricationOrdersByCriterial(salesOrders, productionOrders, false);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Test to get recipes.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(100)]
        [TestCase(101)]
        [TestCase(102)]
        public async Task GetRecipe(int orderId)
        {
            // act
            var result = await this.sapService.GetRecipe(orderId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Test to get recipes.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetRecipse()
        {
            // arrange
            var listOrders = new List<int> { 100 };

            // act
            var result = await this.sapService.GetOriginalRouteRecipes(listOrders);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Test to get recipes.
        /// </summary>
        /// <param name="pedidosReturnsError">Pedidos returns error.</param>
        /// <param name="pedidoId">Pedido id.</param>
        /// <returns>The data.</returns>
        [Test]
        [TestCase(true, 200)]
        [TestCase(false, 2000)]
        public async Task ValidateOrder(bool pedidosReturnsError, int pedidoId)
        {
            // arrange
            var order = new List<int> { 2000 };

            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();
            var mockCatalogs = new Mock<ICatalogsService>();
            var mockLog = new Mock<ILogger>();
            var mockDoctor = new Mock<IDoctorService>();

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetInvalidOrdersByMissingTecnicSign(pedidosReturnsError, pedidoId.ToString())));

            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            this.sapService = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object, mockDoctor.Object, mockCatalogs.Object);

            // act
            var response = await this.sapService.ValidateOrder(order);

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ExceptionMessage, Is.Null);
            Assert.That(response.Comments, Is.Null);
            Assert.That(response.UserError, Is.Null);

            if (pedidosReturnsError)
            {
                Assert.That(response.Success, Is.False);
                Assert.That(response.Code.Equals(400));
                Assert.That(response.Response, Is.Not.Null);
                Assert.That(response.Response, Is.InstanceOf<List<OrderValidationResponse>>());
            }
            else
            {
                Assert.That(response.Success);
                Assert.That(response.Code.Equals(200));
                Assert.That(response.Response, Is.Null);
            }
        }

        /// <summary>
        /// Test to get recipes.
        /// </summary>
        /// <returns>The data.</returns>
        [Test]
        public async Task GetDetails()
        {
            // arrange
            var dict = new Dictionary<string, string>();

            // act
            var response = await this.sapService.GetDetails(dict, "ped");

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test to get recipes.
        /// </summary>
        /// <returns>The data.</returns>
        [Test]
        public async Task GetDetailsWithRedis()
        {
            // arrange
            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();
            var mockLog = new Mock<ILogger>();
            var mockCatalogs = new Mock<ICatalogsService>();

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Value = "A1", Field = "Medic" },
            };

            mockCatalogs
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(parameters)));

            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:prefix")]).Returns("L-");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:numberPositions")]).Returns("7");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "OmicronRecipeAddress")]).Returns("http://localhost:5002/");

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetUsersById()));

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            mockRedis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult("[100]"));

            this.sapDao = new SapDao(this.context, mockLog.Object);

            var mockDoctor = new Mock<IDoctorService>();
            mockDoctor
                .Setup(m => m.PostDoctors(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetDoctorsInfo()));

            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            var sapService = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object, mockDoctor.Object, mockCatalogs.Object);

            var dict = new Dictionary<string, string>();

            // act
            var response = await sapService.GetDetails(dict, "ped");

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Get packing required for order in assigned status.
        /// </summary>
        /// <returns>the detail.</returns>
        [Test]
        public async Task GetPackingRequiredForOrderInAssignedStatus()
        {
            // arrange
            var userId = "123-abc";

            // act
            var result = await this.sapService.GetPackingRequiredForOrderInAssignedStatus(userId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersDocNumDxp()
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.DocNumDxp, "A1" },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get Raw Material Request.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="status">Status.</param>
        /// <param name="sendDates">Send dates.</param>
        /// <returns>the detail.</returns>
        [Test]
        [TestCase(null, "Abierto,Cerrado,Cancelado", true)]
        [TestCase(null, "Abierto,Cancelado", true)]
        [TestCase(null, "Abierto", true)]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", "Abierto,Cerrado,Cancelado", true)]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", "Abierto,Cancelado", true)]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", "Abierto", true)]
        [TestCase("db106faf-ef03-4c2e-9b7c-be0c7da8c0b7", "Abierto", false)]
        [TestCase("4c2e983e-87db-5864-ae16-108b666bc19d", "Abierto", true)]
        public async Task GetRawMaterialRequest(string userId, string status, bool sendDates)
        {
            // arrange
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "5" },
                { ServiceConstants.Status, status },
            };

            if (userId != null)
            {
                dicParams.Add(ServiceConstants.ParameterUserId, userId);
            }

            if (sendDates)
            {
                dicParams.Add(ServiceConstants.FechaInicio, DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy"));
                dicParams.Add(ServiceConstants.FechaFin, DateTime.Today.ToString("dd/MM/yyyy"));
            }

            // act
            var result = await this.sapService.GetRawMaterialRequest(dicParams);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success);
            Assert.That(result.Code.Equals(200));
            Assert.That(result.Response, Is.InstanceOf<List<CompleteRawMaterialRequestModel>>());

            var rawMaterial = (List<CompleteRawMaterialRequestModel>)result.Response;

            if (userId == "4c2e983e-87db-5864-ae16-108b666bc19d")
            {
                Assert.That(result.Comments.Equals(0));
                Assert.That(rawMaterial.Count.Equals(0));
            }
            else if (string.IsNullOrEmpty(userId) && status == "Abierto,Cerrado,Cancelado")
            {
                Assert.That(result.Comments.Equals(6));
                Assert.That(rawMaterial.Count.Equals(5));
            }
            else if (string.IsNullOrEmpty(userId) && status == "Abierto,Cancelado")
            {
                Assert.That(result.Comments.Equals(4));
                Assert.That(rawMaterial.Count.Equals(4));
            }
            else if (string.IsNullOrEmpty(userId) && status == "Abierto")
            {
                Assert.That(result.Comments.Equals(2));
                Assert.That(rawMaterial.Count.Equals(2));
            }
            else if (status == "Abierto,Cerrado,Cancelado")
            {
                Assert.That(result.Comments.Equals(3));
                Assert.That(rawMaterial.Count.Equals(3));
            }
            else if (status == "Abierto,Cancelado")
            {
                Assert.That(result.Comments.Equals(2));
                Assert.That(rawMaterial.Count.Equals(2));
            }
            else
            {
                Assert.That(result.Comments.Equals(1));
                Assert.That(rawMaterial.Count.Equals(1));
            }
        }

        /// <summary>
        /// Test to get recipes.
        /// </summary>
        /// <returns>The data.</returns>
        [Test]
        public async Task GetOrderInformationByTransaction()
        {
            // arrange
            var dict = new Dictionary<string, string>()
            { { "idtransaction", "123" } };

            // act
            var response = await this.sapService.GetOrderInformationByTransaction(dict);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test to get warehouses.
        /// </summary>
        /// <returns> The data. </returns>
        [Test]
        public async Task GetWarehouses()
        {
            // arrange
            var dict = new List<string>()
            {
                "BE",
                "AMP",
            };

            // act
            var response = await this.sapService.GetWarehouses(dict);

            // assert
            Assert.That(response, Is.Not.Null);
        }

        /// <summary>
        /// Test to get classifications.
        /// </summary>
        /// <returns> The data. </returns>
        [Test]
        public async Task GetClassificationsByDescription()
        {
            // arrange
            var dict = new List<string>()
            {
                "BIOEQUAL",
                "BIOELITE",
            };

            // act
            var response = await this.sapService.GetClassificationsByDescription(dict);

            // assert
            Assert.That(response, Is.Not.Null);
        }
    }
}
