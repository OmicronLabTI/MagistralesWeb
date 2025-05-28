// <summary>
// <copyright file="PedidosAlmacenServiceTest.cs" company="Axity">
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
    public class PedidosAlmacenServiceTest : BaseTest
    {
        private IPedidosAlmacenService pedidosAlmacen;

        private Mock<IConfiguration> configuration;

        private IPedidosDao pedidosDao;

        private DatabaseContext context;

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
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            var mockSapFile = new Mock<ISapFileService>();

            this.configuration = new Mock<IConfiguration>();
            this.configuration.SetupGet(x => x[It.Is<string>(s => s == "OmicronFilesAddress")]).Returns("http://localhost:5002/");

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosAlmacen = new PedidosAlmacenService(this.pedidosDao, mockSapFile.Object, this.configuration.Object);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForAlmacen()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForAlmacen();

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForAlmacenById()
        {
            var listIds = new List<int> { 207, 206 };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForAlmacen(listIds);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateUserOrders()
        {
            // arrange
            var listorders = new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Status = "Almacenado" },
            };

            // act
            var result = await this.pedidosAlmacen.UpdateUserOrders(listorders);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForDelivery()
        {
            // arrange
            var parameters = new Dictionary<string, string>
            {
                { "startdate", DateTime.Now.ToString("dd/MM/yyyy") },
                { "enddate", DateTime.Now.ToString("dd/MM/yyyy") },
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForDelivery(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForDeliveryList()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForDelivery(new List<int> { 100 });

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForInvoice()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForInvoice();

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForPackages()
        {
            // arrange
            var dict = new Dictionary<string, string>
            {
                { "status", "Empaquetado,Enviado" },
                { "startdate", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy") },
                { "enddate", DateTime.Now.ToString("dd/MM/yyyy") },
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForPackages(dict);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="type">the type.</param>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateSentOrders()
        {
            // arrange
            var listUserOrders = new List<UserOrderModel>
            {
                new UserOrderModel { InvoiceId = 100, StatusInvoice = "Enviado" },
            };

            // act
            var result = await this.pedidosAlmacen.UpdateSentOrders(listUserOrders);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetAlmacenGraphData()
        {
            // arrange
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            var today = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            var dict = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, $"{yesterday}-{today}" },
            };

            // act
            var result = await this.pedidosAlmacen.GetAlmacenGraphData(dict);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetUserOrderByDeliveryOrder()
        {
            // arrange
            var listIds = new List<int> { 100 };

            // act
            var result = await this.pedidosAlmacen.GetUserOrderByDeliveryOrder(listIds);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetUserOrderByInvoiceId()
        {
            // arrange
            var listIds = new List<int> { 4, 5 };

            // act
            var result = await this.pedidosAlmacen.GetUserOrderByInvoiceId(listIds);
            var userorders = result.Response as List<UserOrderModel>;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Success);
            Assert.That(result.Code == 200);
            Assert.That(userorders.Count > 0);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateinvoicePdf()
        {
            var details = new List<int> { 100 };
            var type = "invoice";

            var dictResponse = new Dictionary<string, string>
            {
                { "100", "Ok-C:\\algo" },
            };

            var response = new ResultModel
            {
                Code = 200,
                Success = true,
                Response = JsonConvert.SerializeObject(dictResponse),
            };

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            var pedidoServiceLocal = new PedidosAlmacenService(this.pedidosDao, mockSapFile.Object, this.configuration.Object);

            // act
            var result = await pedidoServiceLocal.CreatePdf(type, details);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task AdvanceLook()
        {
            var details = new List<int> { 100 };
            var mockSapFile = new Mock<ISapFileService>();

            var pedidoServiceLocal = new PedidosAlmacenService(this.pedidosDao, mockSapFile.Object, this.configuration.Object);

            // act
            var result = await pedidoServiceLocal.AdvanceLook(details);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// GetOrdersForAlmacenByRangeDates.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForAlmacenByRangeDates()
        {
            var parameters = new Dictionary<string, string>
            {
                { "startdate", DateTime.Now.ToString("dd/MM/yyyy") },
                { "enddate", DateTime.Now.ToString("dd/MM/yyyy") },
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForAlmacenByRangeDates(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// GetOrdersForAlmacenByOrdersId.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForAlmacenByOrdersId()
        {
            var ordersId = new List<int> { 100 };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForAlmacenByOrdersId(ordersId);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForInvoiceByRangeDates()
        {
            var parameters = new Dictionary<string, string>
            {
                { "startdate", DateTime.Now.ToString("dd/MM/yyyy") },
                { "enddate", DateTime.Now.ToString("dd/MM/yyyy") },
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForInvoiceByRangeDates(parameters);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetUserOrdersForInvoiceByDeliveryIds()
        {
            // arrange
            var deliveryIds = new List<int> { 1, 2, 3, 4, 5, 6 };

            // act
            var result = await this.pedidosAlmacen.GetUserOrdersForInvoiceByDeliveryIds(deliveryIds);

            // assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
