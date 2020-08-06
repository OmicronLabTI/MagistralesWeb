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

        private ISapAdapter sapAdapter;

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
            this.context.UserOrderModel.Add(this.GetUserOrderModel());
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
            this.context.SaveChanges();

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.CreateFabOrder(It.IsAny<object>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object);
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
    }
}
