// <summary>
// <copyright file="CatalogServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test.Services
{
    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class CatalogServiceTest : BaseTest
    {
        private ICatalogService catalogService;

        private IMapper mapper;

        private ICatalogDao catalogDao;

        private DatabaseContext context;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            this.mapper = mapperConfiguration.CreateMapper();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapAdapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();
            var mapper = new Mock<IMapper>();

            this.context = new DatabaseContext(options);
            this.context.RoleModel.AddRange(this.GetListRoles());
            this.context.ParametersModel.AddRange(this.GetParameters());
            this.context.ClassificationQfbModel.AddRange(this.GetActiveClassificationQfbModel());
            this.context.WarehousesModel.AddRange(this.GetWarehouses());
            this.context.ConfigRoutesModel.AddRange(this.GetConfigRoutesModel());
            this.context.ProductTypeColorsModel.AddRange(this.GetProductsColors());
            this.context.SaveChanges();

            this.catalogDao = new CatalogDao(this.context);
            this.catalogService = new CatalogService(this.catalogDao, config.Object, azure.Object, sapAdapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAllRolesTest()
        {
            var result = await this.catalogService.GetRoles();

            Assert.That(result != null);
            Assert.That(result.Response, Is.Not.Null);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetParams()
        {
            var dictValues = new Dictionary<string, string>
            {
                { "Email", "Email" },
            };

            var result = await this.catalogService.GetParamsContains(dictValues);

            Assert.That(result != null);
            Assert.That(result.Response, Is.Not.Null);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActiveClassificationQfb()
        {
            var result = await this.catalogService.GetActiveClassificationQfb();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code == 200, Is.True);
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Response, Is.InstanceOf<object>());
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActiveAllClassificationQfb()
        {
            var result = await this.catalogService.GetActiveAllClassificationQfb();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code == 200, Is.True);
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Response, Is.InstanceOf<object>());
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task UploadWarehouseFromExcel()
        {
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountKey")]).Returns("AzureAccountKey");
            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountName")]).Returns("AzureAccountName");
            config.SetupGet(x => x[It.Is<string>(s => s == "WarehousesFileUrl")]).Returns("WarehousesFileUrl");

            sapadapter.SetupSequence(x => x.Post(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(new List<WarehousesDto>() { new () { WarehouseCode = "CUA" }, new () { WarehouseCode = "FARMACIA" } })));

            catalogsdxp.SetupSequence(x => x.Post(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(new List<string>() { "bioelite" })))
                .Returns(Task.FromResult(this.GetResultDto(new List<string>() { "1037   60 ml" })));

            using var memoryStream = new MemoryStream();
            var workbook = CreateExcel();

            azure
                .Setup(x => x.GetElementsFromAzure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((param1, param2, param3, stream) =>
                {
                    workbook.CopyTo(stream);
                });
            var mapper = new Mock<IMapper>();

            var service = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            var result = await service.UploadWarehouseFromExcel();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);
        }

        /// <summary>
        /// Method to verify upload product type colors from excel.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task UploadProductTypeColorsFromExcel()
        {
            // Arrange
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();
            var catalogDao = new Mock<ICatalogDao>();
            var mapper = new Mock<IMapper>();

            // Setup configuration for Azure
            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountKey")]).Returns("AzureAccountKey");
            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountName")]).Returns("AzureAccountName");
            config.SetupGet(x => x[It.Is<string>(s => s == "ProductTypeColorsFileUrl")]).Returns("ProductTypeColorsFileUrl");

            var productTypeColorsFromExcel = new List<ProductTypeColorsModel>
            {
                new ProductTypeColorsModel { TemaId = "TEMA-1", BackgroundColor = "#FF0000", LabelText = "LINEA", TextColor = "#0000FF" },
                new ProductTypeColorsModel { TemaId = "TEMA-2", BackgroundColor = "#00FF00", LabelText = "LINEA" },
                new ProductTypeColorsModel { TemaId = "TEMA-3", BackgroundColor = "#0000FF", LabelText = "MAGISTRAL" },
                new ProductTypeColorsModel { TemaId = "TEMA-4", BackgroundColor = "#FFFF00", LabelText = "MAGISTRAL" },
            };

            var existingTemaIds = new List<string> { "TEMA-1", "TEMA-2" };

            using var memoryStream = new MemoryStream();
            var workbook = CreateExcelProductTypeColors();

            azure
                .Setup(x => x.GetElementsFromAzure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((param1, param2, param3, stream) =>
                {
                    workbook.CopyTo(stream);
                });

            catalogDao.Setup(x => x.GetExistingTemaIds(It.IsAny<List<string>>()))
              .ReturnsAsync(existingTemaIds);

            catalogDao.Setup(x => x.UpdateProductTypecolors(It.IsAny<List<ProductTypeColorsModel>>()))
                      .ReturnsAsync(true);

            catalogDao.Setup(x => x.InsertProductTypecolors(It.IsAny<List<ProductTypeColorsModel>>()))
                      .ReturnsAsync(true);

            redis.Setup(x => x.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                      .ReturnsAsync(true);

            var service = new CatalogService(
                catalogDao.Object, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            var result = await service.UploadProductTypeColorsFromExcel();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);

            catalogDao.Verify(x => x.GetExistingTemaIds(It.IsAny<List<string>>()), Times.Once);
            catalogDao.Verify(x => x.UpdateProductTypecolors(It.IsAny<List<ProductTypeColorsModel>>()), Times.Once);
            catalogDao.Verify(x => x.InsertProductTypecolors(It.IsAny<List<ProductTypeColorsModel>>()), Times.Once);

            redis.Verify(
                x => x.WriteToRedis(
            ServiceConstants.ProductTypeColors,
            It.IsAny<string>(),
            It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(12))), Times.Once);
        }

        /// <summary>
        /// Method to verify Get All Users.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActivesWarehouses()
        {
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();
            var mapper = new Mock<IMapper>();

            var service = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            var products = new List<ActiveWarehouseDto>() { new ActiveWarehouseDto { ItemCode = "REVE 42", CatalogName = string.Empty, FirmName = "REVE" }, };
            var result = await service.GetActivesWarehouses(products);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);
        }

        /// <summary>
        /// Test to verify method get classifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetClassifications()
        {
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();

            var manufacturers = new List<ManufacturersDto>() { new ManufacturersDto { Id = 1, Classification = "Classification unique administration", ClassificationCode = "CUA" }, };

            catalogsdxp.SetupSequence(x => x.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(manufacturers)));
            var mapper = new Mock<IMapper>();

            var service = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            var result = await service.GetClassifications();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);
        }

        /// <summary>
        /// Method to verify upload sorting from excel.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task UploadConfigRouteFromExcel()
        {
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();

            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountKey")]).Returns("AzureAccountKey");
            config.SetupGet(x => x[It.Is<string>(s => s == "AzureAccountName")]).Returns("AzureAccountName");
            config.SetupGet(x => x[It.Is<string>(s => s == "ManufacturersFileUrl")]).Returns("ManufacturersFileUrl");

            sapadapter.SetupSequence(x => x.Post(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(new List<ClassificationsDto>() { new () { Description = "MAGISTRALES", Value = "MG" }, new () { Description = "DE LINEA", Value = "LN" } })));

            catalogsdxp.SetupSequence(x => x.Post(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDto(new List<string>() { "DZ 21", "DZ 44", "OMI 02" })))
                .Returns(Task.FromResult(this.GetResultDto(new List<string>() { "DZ 21", "DZ 49" })));

            using var memoryStream = new MemoryStream();
            var workbook = CreateExcelSortingRoute();

            azure
                .Setup(x => x.GetElementsFromAzure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((param1, param2, param3, stream) =>
                {
                    workbook.CopyTo(stream);
                });
            var mapper = new Mock<IMapper>();

            var service = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            var result = await service.UploadConfigurationRouteFromExcel();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);
        }

        /// <summary>
        /// Get product and container catalog without parameters.
        /// </summary>
        /// <param name="isRedisConnected">If redis is conected.</param>
        /// <returns>representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task TestGetActiveRouteConfigurationsForProducts(bool isRedisConnected)
        {
            // Arrange
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();

            redis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetConfigRoutesModelFromRedis()));

            redis.Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            redis.Setup(m => m.IsConnectedRedis()).Returns(isRedisConnected);
            var mapper = new Mock<IMapper>();
            var catalogServiceMock = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            // Act
            var result = await catalogServiceMock.GetActiveRouteConfigurationsForProducts();
            var response = (List<ConfigRoutesModel>)result.Response;

            // Assets
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Response, Is.InstanceOf<List<ConfigRoutesModel>>());
            Assert.That(response.Count == 3, Is.True);
        }

        /// <summary>
        /// Get product and container catalog without parameters.
        /// </summary>
        /// <param name="isRedisConnected">If redis is conected.</param>
        /// <returns>representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetProductsColors(bool isRedisConnected)
        {
            // Arrange
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();
            var redis = new Mock<IRedisService>();

            redis
                .Setup(m => m.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetConfigRoutesModelFromRedis()));

            redis.Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            redis.Setup(m => m.IsConnectedRedis()).Returns(isRedisConnected);
            var mapper = new Mock<IMapper>();
            var catalogServiceMock = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object, redis.Object, mapper.Object);

            // Act
            var result = await catalogServiceMock.GetProductsColors(new List<string> { "linea", "magistral" });

            // Assets
            Assert.That(result.Success, Is.True);
            Assert.That(result.Code == 200, Is.True);
        }

        private static MemoryStream CreateExcelSortingRoute()
        {
            var dataTable = new DataTable();
            dataTable.TableName = "sortingroute";
            dataTable.Columns.Add("Clasificacion");
            dataTable.Columns.Add("Excepciones");
            dataTable.Columns.Add("ItemCode");
            dataTable.Columns.Add("Color");
            dataTable.Columns.Add("Status");
            dataTable.Columns.Add("Ruta");

            var models = new List<ConfigRoutesModel>
            {
                new ConfigRoutesModel { Classification = "de LiNea", ItemCode = "omI 02", Route = "Almacén" },
                new ConfigRoutesModel { Classification = "de linea", ItemCode = "DZ 22" },
                new ConfigRoutesModel { Classification = null, ItemCode = string.Empty },
                new ConfigRoutesModel { Classification = "magistraLEs", ItemCode = string.Empty, Color = "#ABCDEF", Route = "Almacén" },
                new ConfigRoutesModel { Classification = string.Empty, ItemCode = "DZ 21", Exceptions = "DZ 21" },
                new ConfigRoutesModel { Classification = string.Empty, ItemCode = "DZ 44", Exceptions = "DZ 49", Color = "FFF" },
            };

            models.ForEach(model =>
            {
                dataTable.Rows.Add(
                    model.Classification ?? string.Empty,
                    model.Exceptions ?? string.Empty,
                    model.ItemCode ?? string.Empty,
                    model.Color ?? string.Empty,
                    ServiceConstants.IsActive,
                    model.Route ?? string.Empty);
            });

            var mss = new MemoryStream();
            var wb = new XLWorkbook();

            wb.AddWorksheet("Hoja1");

            wb.Worksheets.Add(dataTable);
            wb.SaveAs(mss);
            mss.Position = 0;

            return mss;
        }

        private static MemoryStream CreateExcel()
        {
            var dataTable = new DataTable();
            dataTable.TableName = "Warehouse";
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("AppliesToProducts");
            dataTable.Columns.Add("AppliesToManufacturers");
            dataTable.Columns.Add("IsActive");
            dataTable.Columns.Add("Exceptions");

            dataTable.Rows.Add(new List<string> { "be", "bioelite", "1037   60 ml", ServiceConstants.IsActive, string.Empty }.ToArray());
            dataTable.Rows.Add(new List<string> { "aa", "aa", string.Empty, ServiceConstants.IsActive, string.Empty }.ToArray());

            var mss = new MemoryStream();
            var wb = new XLWorkbook();
            wb.Worksheets.Add(dataTable);
            wb.SaveAs(mss);
            mss.Position = 0;

            return mss;
        }

        private static MemoryStream CreateExcelProductTypeColors()
        {
            var dataTable = new DataTable();
            dataTable.TableName = "ProductTypeColors";
            dataTable.Columns.Add("IdTema");
            dataTable.Columns.Add("Background");
            dataTable.Columns.Add("ProductType");
            dataTable.Columns.Add("TextColor");
            dataTable.Columns.Add("IsActive");

            var models = new List<object[]>
            {
                new object[] { "TEMA-1", "#FF0000", "LINEA", "#FF0000", ServiceConstants.IsActive },
                new object[] { "TEMA-2", "#FF0000", "LINEA", "#FF0000", ServiceConstants.IsActive },
                new object[] { "TEMA-3", "#FF0000", "MAGISTRAL", "#FF0000", ServiceConstants.IsActive },
                new object[] { "TEMA-4", "#FF0000", "MAGISTRAL", "#FF0000", ServiceConstants.IsActive },
            };

            models.ForEach(model =>
            {
                dataTable.Rows.Add(model);
            });

            var mss = new MemoryStream();
            var wb = new XLWorkbook();

            wb.Worksheets.Add(dataTable);
            wb.SaveAs(mss);
            mss.Position = 0;

            return mss;
        }
    }
}
