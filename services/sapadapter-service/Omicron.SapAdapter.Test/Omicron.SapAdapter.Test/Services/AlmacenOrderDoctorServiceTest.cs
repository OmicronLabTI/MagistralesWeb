// <summary>
// <copyright file="AlmacenOrderDoctorServiceTest.cs" company="Axity">
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
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model.AlmacenModels;
    using Omicron.SapAdapter.Services.Almacen;
    using Omicron.SapAdapter.Services.Catalog;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Sap;
    using Serilog;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AlmacenOrderDoctorServiceTest : BaseTest
    {
        private IAlmacenOrderDoctorService almacenOrderDoctorService;

        private ISapDao sapDao;

        private DatabaseContext context;

        private Mock<ICatalogsService> catalogService;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAlmacenOrdersService")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.ProductoModel.AddRange(this.GetProductoModelForDoctorOrders());
            this.context.OrderModel.AddRange(this.GetOrderModelForDoctorOrders());
            this.context.DetallePedido.AddRange(this.GetDetallePedidoForDoctorOrders());
            this.context.ClientCatalogModel.AddRange(this.GetClients());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModelForDoctorOrders());
            this.context.SaveChanges();

            var mockPedidoService = new Mock<IPedidosService>();
            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidosForDoctorOrders()));

            mockPedidoService
                .Setup(m => m.PostPedidos(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidosForDoctorOrders()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetLineProductsForDoctorOrders()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            var parameters = new List<ParametersModel>
            {
                new ParametersModel { Value = "10" },
            };

            this.catalogService = new Mock<ICatalogsService>();
            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModel(parameters)));

            this.sapDao = new SapDao(this.context, mockLog.Object);

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            mockRedis
                .Setup(x => x.IsConnectedRedis())
                .Returns(true);
            this.almacenOrderDoctorService = new AlmacenOrderDoctorService(this.sapDao, mockPedidoService.Object, mockAlmacen.Object, this.catalogService.Object, mockRedis.Object);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <param name="type">the type of orders.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase("magistral, mixto, maquila, linea")]
        [TestCase("magistral, mixto, linea")]
        public async Task SearchAlmacenOrdersByAllsDoctors(string type)
        {
            // arrange
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Type, type },
            };

            // act
            var response = await this.almacenOrderDoctorService.SearchAlmacenOrdersByDoctor(dictionary);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersByDoctor()
        {
            // arrange
            var dictionary = new Dictionary<string, string>
            {
                { ServiceConstants.Offset, "0" },
                { ServiceConstants.Limit, "10" },
                { ServiceConstants.Chips, "alias" },
                { ServiceConstants.Shipping, "Foraneo" },
            };

            var localNeighBors = new List<ParametersModel>
            {
                new ParametersModel { Value = "Nuevo León", Field = "LocalNeighborhood" },
            };

            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModel(localNeighBors)));

            // act
            var response = await this.almacenOrderDoctorService.SearchAlmacenOrdersByDoctor(dictionary);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task SearchAlmacenOrdersDetailsByDoctor()
        {
            // arrange
            var request = new DoctorOrdersSearchDeatilDto
            {
                Address = "CDMX",
                Name = "alias",
                SaleOrders = new List<int>
                {
                    84503,
                    84517,
                    84515,
                },
            };

            var localNeighBors = new List<ParametersModel>
            {
                new ParametersModel { Value = "Nuevo León", Field = "LocalNeighborhood" },
            };

            this.catalogService
                .Setup(m => m.GetParams(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModel(localNeighBors)));

            // act
            var response = await this.almacenOrderDoctorService.SearchAlmacenOrdersDetailsByDoctor(request);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Test the method to get the orders for almacen.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetOrderdetail()
        {
            // arrange
            var salesOrderId = 84515;

            var mockPedido = new Mock<IPedidosService>();
            mockPedido
                .Setup(m => m.GetUserPedidos(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidosForDoctorOrders()));

            var mockAlmacen = new Mock<IAlmacenService>();
            mockAlmacen
                .Setup(m => m.PostAlmacenOrders(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetIncidents()));

            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetRedisKey(It.IsAny<string>()))
                .Returns(Task.FromResult(string.Empty));

            var localService = new AlmacenOrderDoctorService(this.sapDao, mockPedido.Object, mockAlmacen.Object, this.catalogService.Object, mockRedis.Object);

            // act
            var response = await localService.GetOrderdetail(salesOrderId);

            // assert
            Assert.IsNotNull(response);
        }
    }
}
