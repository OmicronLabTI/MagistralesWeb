// <summary>
// <copyright file="SapAlmacenServiceTest.cs" company="Axity">
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
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class SapAlmacenServiceTest : BaseTest
    {
        private ISapAlmacenService sapService;

        private ISapDao sapDao;

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
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.SaveChanges();

            var mockLog = new Mock<ILogger>();
            mockLog.Setup(m => m.Information(It.IsAny<string>()));

            var mockPedidoService = new Mock<IPedidosService>();
            var mockAlmacenService = new Mock<IAlmacenService>();
            var mockCatalogos = new Mock<ICatalogsService>();

            this.sapDao = new SapDao(this.context, mockLog.Object);
            this.sapService = new SapAlmacenService(this.sapDao, mockPedidoService.Object, mockAlmacenService.Object, mockCatalogos.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrders()
        {
            // arrange
            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            var parametersResponse = this.GetResultModel(parameters);

            var mockPedidos = new Mock<IPedidosService>();
            mockPedidos
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUserOrderModelAlmacen()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.GetAlmacenOrders(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetLineProducts()));

            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockCatalogos = new Mock<ICatalogsService>();
            mockCatalogos
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(parametersResponse));

            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
            };

            var localService = new SapAlmacenService(this.sapDao, mockPedidos.Object, mockAlmacen.Object, mockCatalogos.Object);

            // act
            var response = await localService.GetOrders(dictionary);

            // assert
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedData()
        {
            // arrange
            var order = "aa";

            // act
            var response = await this.sapService.GetLineScannedData(order);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetLineScannedDataBatches()
        {
            // arrange
            var order = "Linea1";

            // act
            var response = await this.sapService.GetLineScannedData(order);

            // assert
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
        }
    }
}
