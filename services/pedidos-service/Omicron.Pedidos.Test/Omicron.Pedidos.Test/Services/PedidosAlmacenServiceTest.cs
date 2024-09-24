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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrdersForDelivery()
        {
            // act
            var result = await this.pedidosAlmacen.GetOrdersForDelivery();

            // assert
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
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
            };

            // act
            var result = await this.pedidosAlmacen.GetOrdersForPackages(dict);

            // assert
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.Response);
            ClassicAssert.True(result.Success);
            ClassicAssert.True(result.Code == 200);
            ClassicAssert.True(userorders.Count > 0);
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
            ClassicAssert.IsNotNull(result);
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
            ClassicAssert.IsNotNull(result);
        }
    }
}
