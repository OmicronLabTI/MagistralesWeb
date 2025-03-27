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

            this.context = new DatabaseContext(options);
            this.context.RoleModel.AddRange(this.GetListRoles());
            this.context.ParametersModel.AddRange(this.GetParameters());
            this.context.ClassificationQfbModel.AddRange(this.GetActiveClassificationQfbModel());
            this.context.WarehousesModel.AddRange(this.GetWarehouses());
            this.context.SaveChanges();

            this.catalogDao = new CatalogDao(this.context);
            this.catalogService = new CatalogService(this.catalogDao, config.Object, azure.Object, sapAdapter.Object, catalogsdxp.Object);
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
        public async Task UploadWarehouseFromExcel()
        {
            var config = new Mock<IConfiguration>();
            var azure = new Mock<IAzureService>();
            var sapadapter = new Mock<ISapAdapterService>();
            var catalogsdxp = new Mock<ICatalogsDxpService>();

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

            var service = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object);

            var result = await service.UploadWarehouseFromExcel();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);
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

            var service = new CatalogService(this.catalogDao, config.Object, azure.Object, sapadapter.Object, catalogsdxp.Object);

            var products = new List<ActiveWarehouseDto>() { new ActiveWarehouseDto { ItemCode = "REVE 42", CatalogName = string.Empty, FirmName = "REVE" }, };
            var result = await service.GetActivesWarehouses(products);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Code == 200, Is.True);
        }

        private static MemoryStream CreateExcel()
        {
            var dataTable = new DataTable();
            dataTable.TableName = "Warehouse";
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("AppliesToProducts");
            dataTable.Columns.Add("AppliesToManufacturers");
            dataTable.Columns.Add("IsActive");

            dataTable.Rows.Add(new List<string> { "be", "bioelite", "1037   60 ml", ServiceConstants.IsActive, }.ToArray());
            dataTable.Rows.Add(new List<string> { "aa", "aa", string.Empty, ServiceConstants.IsActive, }.ToArray());

            var mss = new MemoryStream();
            var wb = new XLWorkbook();
            wb.Worksheets.Add(dataTable);
            wb.SaveAs(mss);
            mss.Position = 0;

            return mss;
        }
    }
}
