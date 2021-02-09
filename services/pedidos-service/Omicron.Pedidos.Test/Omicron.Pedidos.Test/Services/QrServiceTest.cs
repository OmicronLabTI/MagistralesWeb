// <summary>
// <copyright file="QrServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.AlmacenService;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class QrServiceTest : BaseTest
    {
        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        private QrService qrsService;

        private Mock<IConfiguration> configuration;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalQr")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.SaveChanges();

            this.configuration = new Mock<IConfiguration>();
            this.configuration.SetupGet(x => x[It.Is<string>(s => s == "QrImagesBaseRoute")]).Returns("http://localhost:5002/");

            var mockSapAdapter = new Mock<ISapAdapter>();
            var mockAlmacen = new Mock<IAlmacenService>();

            this.pedidosDao = new PedidosDao(this.context);
            this.qrsService = new QrService(this.pedidosDao, this.configuration.Object, mockSapAdapter.Object, mockAlmacen.Object);
        }

        /// <summary>
        /// Test the creation of the Qr.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateQrMagistral()
        {
            var listOrdersId = new List<int> { 300, 301 };

            var response = await this.qrsService.CreateMagistralQr(listOrdersId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test the creation of the Qr.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateRemisionQr()
        {
            // arrange
            var mockAlmacen = new Mock<IAlmacenService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var service = new QrService(this.pedidosDao, this.configuration.Object, mockSapAdapter.Object, mockAlmacen.Object);
            var listOrdersId = new List<int> { 105 };

            var response = await service.CreateRemisionQr(listOrdersId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test the creation of the Qr.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateRemisionQrLinea()
        {
            // arrange
            var remisionQr = new RemisionQrModel
            {
                RemisionId = 100,
                NeedsCooling = true,
                PedidoId = 300,
                TotalPieces = 5,
            };

            var lineResult = new List<LineProductsModel>
            {
                new LineProductsModel { SaleOrderId = 106, RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 106 },
            };

            var responseAlmacen = this.GenerateResultModel(lineResult);

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenData(It.IsAny<string>()))
                .Returns(Task.FromResult(responseAlmacen));

            var mockSapAdapter = new Mock<ISapAdapter>();

            var service = new QrService(this.pedidosDao, this.configuration.Object, mockSapAdapter.Object, mockAlmacen.Object);

            var listOrdersId = new List<int> { 106 };

            var response = await service.CreateRemisionQr(listOrdersId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test the creation of the Qr.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateInvoiceQrMagistral()
        {
            var mockAlmacen = new Mock<IAlmacenService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var service = new QrService(this.pedidosDao, this.configuration.Object, mockSapAdapter.Object, mockAlmacen.Object);

            var listOrdersId = new List<int> { 100 };

            var response = await service.CreateInvoiceQr(listOrdersId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test the creation of the Qr.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateInvoiceQrLinea()
        {
            // arrange
            var remisionQr = new InvoiceQrModel
            {
                InvoiceId = 100,
                NeedsCooling = true,
            };

            var lineResult = new List<LineProductsModel>
            {
                new LineProductsModel { SaleOrderId = 106, InvoiceQr = JsonConvert.SerializeObject(remisionQr) },
            };

            var responseAlmacen = this.GenerateResultModel(lineResult);

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenData(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(responseAlmacen));

            var mockSapAdapter = new Mock<ISapAdapter>();

            var service = new QrService(this.pedidosDao, this.configuration.Object, mockSapAdapter.Object, mockAlmacen.Object);

            var listOrdersId = new List<int> { 107 };

            var response = await service.CreateInvoiceQr(listOrdersId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }
    }
}
