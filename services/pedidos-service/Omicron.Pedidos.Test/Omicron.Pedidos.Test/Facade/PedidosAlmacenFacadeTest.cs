// <summary>
// <copyright file="PedidosAlmacenFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Facade
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Facade.Pedidos;
    using Omicron.Pedidos.Services.Mapping;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class PedidosAlmacenFacadeTest : BaseTest
    {
        private PedidosAlmacenFacade almacenFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockServiceAlmacen = new Mock<IPedidosAlmacenService>();
            mockServiceAlmacen.SetReturnsDefault(Task.FromResult(response));

            var mockCancel = new Mock<ICancelPedidosService>();
            mockCancel.SetReturnsDefault(Task.FromResult(response));

            this.almacenFacade = new PedidosAlmacenFacade(
                mockServiceAlmacen.Object,
                mapper,
                mockCancel.Object);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetOrdersForAlmacen()
        {
            // arrange
            // act
            var response = await this.almacenFacade.GetOrdersForAlmacen();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task UpdateUserOrders()
        {
            // arrange
            var listUsers = new List<UserOrderDto>();

            // act
            var response = await this.almacenFacade.UpdateUserOrders(listUsers);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetOrdersForDelivery()
        {
            // arrange
            // act
            var response = await this.almacenFacade.GetOrdersForDelivery();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetOrdersForInvoice()
        {
            // arrange
            // act
            var response = await this.almacenFacade.GetOrdersForInvoice();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task GetOrdersForPackages()
        {
            // arrange
            var type = new Dictionary<string, string>();

            // act
            var response = await this.almacenFacade.GetOrdersForPackages(type);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// test tet.
        /// </summary>
        /// <returns>test.</returns>
        [Test]
        public async Task UpdateSentOrders()
        {
            // arrange
            var type = new List<UserOrderDto>();

            // act
            var response = await this.almacenFacade.UpdateSentOrders(type);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetAlmacenGraphData()
        {
            // arrange
            var type = new Dictionary<string, string>();

            // act
            var response = await this.almacenFacade.GetAlmacenGraphData(type);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task GetUserOrderByDeliveryOrder()
        {
            // arrange
            var type = new List<int>();

            // act
            var response = await this.almacenFacade.GetUserOrderByDeliveryOrder(type);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CreateinvoicePdf()
        {
            // arrange
            var type = new List<int>();

            // act
            var response = await this.almacenFacade.CreatePdf(string.Empty, type);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the test.
        /// </summary>
        /// <returns>returns nothing.</returns>
        [Test]
        public async Task CancelDelivery()
        {
            // arrange
            var type = new CancelDeliveryPedidoCompleteDto();

            // act
            var response = await this.almacenFacade.CancelDelivery(string.Empty, type);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }
    }
}