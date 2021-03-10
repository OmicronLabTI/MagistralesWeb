// <summary>
// <copyright file="AssignOrderServiceTest.cs" company="Axity">
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
    using Moq;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class AssignOrderServiceTest : BaseTest
    {
        private IAssignPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private Mock<IUsersService> usersService;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal2")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.OrderLogModel.Add(this.GetOrderLogModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
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

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new AssignPedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);
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

            var pedidosServiceLocal = new AssignPedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

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

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var pedidosServiceLocal = new AssignPedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task AutomaticAssign()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 100, 101 },
                UserLogistic = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign));
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task ReassignOrder()
        {
            var reassign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1, 2, 3 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abc",
            };

            // act
            var result = await this.pedidosService.ReassignOrder(reassign);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task ReassignOrderOrden()
        {
            var reassign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abc",
            };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            var pedidosServiceLocal = new AssignPedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object);

            // act
            var result = await pedidosServiceLocal.ReassignOrder(reassign);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
