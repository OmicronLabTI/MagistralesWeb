// <summary>
// <copyright file="PedidosDxpFacadeTest.cs" company="Axity">
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
    public class PedidosDxpFacadeTest : BaseTest
    {
        private PedidosDxpFacade pedidosFacade;

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

            var mockBusquedaService = new Mock<IPedidosDxpService>();

            mockBusquedaService.SetReturnsDefault(Task.FromResult(response));

            this.pedidosFacade = new PedidosDxpFacade(mockBusquedaService.Object, mapper);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetDeliveredPayments()
        {
            // arrange
            var order = new List<int>();

            // act
            var response = await this.pedidosFacade.GetDeliveredPayments(order);

            // arrange
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Get Orders Header Status.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetOrdersHeaderStatus()
        {
            // arrange
            var orders = new List<string>();

            // act
            var response = await this.pedidosFacade.GetOrdersHeaderStatus(orders);

            // arrange
            this.AssertResponse(response);
        }

        /// <summary>
        /// Assert response.
        /// </summary>
        /// <param name="response">Response to validate.</param>
        public void AssertResponse(ResultDto response)
        {
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Response);
            Assert.IsEmpty(response.ExceptionMessage);
            Assert.IsEmpty(response.UserError);
            Assert.AreEqual(200, response.Code);
        }
    }
}
