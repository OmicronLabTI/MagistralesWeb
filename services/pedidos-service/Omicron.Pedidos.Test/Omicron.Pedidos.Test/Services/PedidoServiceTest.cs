// <summary>
// <copyright file="PedidoServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidoServiceTest : BaseTest
    {
        private IPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private ISapDiApi sapDiApi;

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
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
            this.context.SaveChanges();

            this.sapAdapter = new Mock<ISapAdapter>();
            this.sapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ProcessOrders()
        {
            // arrange
            var process = new ProcessOrderModel
            {
                ListIds = new List<int> { 100 },
                User = "abc",
            };

            // act
            var response = await this.pedidosService.ProcessOrders(process);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderBySalesOrder()
        {
            // arrange
            var listIds = new List<int> { 1, 2, 3 };

            // act
            var response = await this.pedidosService.GetUserOrderBySalesOrder(listIds);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetFabOrderByUserID()
        {
            // arrange
            var id = "abc";

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserID(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrdersByUserId()
        {
            // arrange
            var id = new List<string> { "abc" };

            // act
            var response = await this.pedidosService.GetUserOrdersByUserId(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task AssignOrderPedido()
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListCompleteDetailOrderModel()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task AssignOrder()
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateComponents()
        {
            // arrange
            var components = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { Available = 1, BaseQuantity = 1, Consumed = 1, Description = "Des", OrderFabId = 2, PendingQuantity = 1, ProductId = "Aspirina", RequiredQuantity = 1, Stock = 1, Unit = "Unit", Warehouse = "wh", WarehouseQuantity = 1 },
            };

            var asignar = new UpdateFormulaModel
            {
                Comments = "Comments",
                Components = components,
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object);

            // act
            var response = await pedidosServiceLocal.UpdateComponents(asignar);

            // assert
            Assert.IsNotNull(response);
        }
    }
}
