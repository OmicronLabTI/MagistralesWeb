// <summary>
// <copyright file="SapInvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class SapInvoiceServiceTest : BaseTest
    {
        private ISapInvoiceService sapInvoiceService;

        private ISapDao sapDao;

        private DatabaseContext context;

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
            this.context.ProductoModel.AddRange(this.GetProductoModel());
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

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapInvoiceService = new SapInvoiceService(this.sapDao, mockPedidoService.Object, mockAlmacenService.Object);
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

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetInvoice(dictionary);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="chip">the chips.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("1")]
        [TestCase("aaa")]
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
            };

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetInvoice(dictionary);

            // assert
            Assert.IsNotNull(response);
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

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetInvoiceProducts(1);

            // assert
            Assert.IsNotNull(response);
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
                .Setup(m => m.GetUserPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderInvoice()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetDeliveryScannedData("75001-1");

            // assert
            Assert.IsNotNull(response);
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

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetMagistralProductInvoice("75000-1000");

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="code">the code to look.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("Linea1-1")]
        [TestCase("Linea10-1")]
        public async Task GetDeliveryScannedDataLn(string code)
        {
            // arrange
            var mockPedidos = new Mock<IPedidosService>();

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsScannInvoice()));

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetLineProductInvoice(code);

            // assert
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="code">the code to look.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("1")]
        public async Task GetInvoiceData(string code)
        {
            // arrange
            var packages = new List<PackageModel>();
            var packagesResponse = this.GetResultDto(packages);

            var mockPedidos = new Mock<IPedidosService>();
            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(packagesResponse));

            var service = new SapInvoiceService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetInvoiceData(code);

            // assert
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetCancelledInvoices()
        {
            // act
            var response = await this.sapInvoiceService.GetCancelledInvoices();

            // assert
            Assert.IsNotNull(response);
        }
    }
}
