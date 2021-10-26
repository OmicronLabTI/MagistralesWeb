// <summary>
// <copyright file="ISapAlmacenDeliverServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class ISapAlmacenDeliverServiceTest : BaseTest
    {
        private ISapAlmacenDeliveryService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

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
            this.context.SaveChanges();

            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapService = new SapAlmacenDeliveryService(this.sapDao, mockPedidoService.Object, mockAlmacenService.Object);
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
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.IsNotNull(response);
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

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, $"{ServiceConstants.Line},{ServiceConstants.Mixto.ToLower()}" },
            };

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.IsNotNull(response);
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

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { "chips", chip },
            };

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetDelivery(dictionary);

            // assert
            Assert.IsNotNull(response);
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
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderRemision()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProductsRemision()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var service = new SapAlmacenDeliveryService(this.sapDao, mockPedidos.Object, mockAlmacen.Object);

            // act
            var response = await service.GetProductsDelivery(chip);

            // assert
            Assert.IsNotNull(response);
        }
    }
}
