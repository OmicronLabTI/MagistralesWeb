// <summary>
// <copyright file="AssignOrderServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Constants;
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

        private ISapDiApi sapDiApi;

        private Mock<IUsersService> usersService;

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

            // act
            var result = await this.pedidosService.ReassignOrder(reassign);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
